using Konscious.Security.Cryptography;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace RW5jcnlwdA
{
    public class FileEnCryptor
    {
        // === Constants / defaults ===
        const int SaltSize = 32;               // bytes
        const int NoncePrefixSize = 4;         // bytes; nonce = prefix(4) || counter(8) => 12 bytes total
        const int NonceSize = 12;              // AES-GCM nonce size (96 bits)
        const int KeySizeBytes = 32;           // AES-256 per subkey
        const int CombinedKeyBytes = 64;       // Argon2 output bytes to split into enc/mac
        const int TagSize = 16;                // GCM tag size (128 bits)
        const int DefaultChunkSize = 64 * 1024; // 64 KiB chunk plaintext size

        // Argon2 defaults (adjust with care). MemorySize is in KB.
        const int Argon2MemoryKB_Default = 262_144; // 256 MB
        const int Argon2Iterations_Default = 3;     // passes
        const int Argon2Parallelism_Default = 2;
        const int MaxArgon2MemoryKB = 1_048_576;    // 1 GB hard cap to avoid abusive allocations

        // Magic & version to detect format
        static readonly byte[] FileMagic = "FENC"u8.ToArray(); // "FENC"
        const byte FormatVersion = 2; // bumped to v2: header authenticated with AAD, key splitting, atomic write

        // ==================================================

        /// <summary>
        /// Encrypt a file with AES-GCM + Argon2id key derivation (streaming by chunks).
        /// Produces file format (v2):
        /// [MAGIC(4)] [VERSION(1)]
        /// [salt(32)]
        /// [argon2: memoryKB(int32)][iterations(int32)][parallelism(int32)]
        /// [noncePrefix(4)]
        /// [chunkSize(int32 little-endian)]
        /// [ciphertext chunk0][tag0][ciphertext chunk1][tag1]...
        /// Header is authenticated as AAD for each chunk.
        /// </summary>
        public static void EncryptFile(
            string inputFile,
            string outputFile,
            string password,
            ArgumentOutOfRangeException argumentOutOfRangeException,
            int chunkSize = DefaultChunkSize,
            int argon2MemoryKB = Argon2MemoryKB_Default,
            int argon2Iterations = Argon2Iterations_Default,
            int argon2Parallelism = Argon2Parallelism_Default)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be empty.", nameof(password));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
            if (!File.Exists(inputFile)) throw new FileNotFoundException("Input file not found.", inputFile);
            if (argon2MemoryKB <= 0 || argon2Iterations <= 0 || argon2Parallelism <= 0) throw argumentOutOfRangeException;
            if (argon2MemoryKB > MaxArgon2MemoryKB) throw new ArgumentOutOfRangeException(nameof(argon2MemoryKB), $"Argon2 memory too large. Max {MaxArgon2MemoryKB} KB");

            // Prepare header values
            byte[] salt = new byte[SaltSize];
            byte[] noncePrefix = new byte[NoncePrefixSize];
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(noncePrefix);

            // Derive combined key via Argon2id (64 bytes) then split
            byte[]? combinedKey = null;
            byte[]? encKey = null;
            try
            {
                combinedKey = DeriveKeyArgon2id(password, salt, CombinedKeyBytes, argon2MemoryKB, argon2Iterations, argon2Parallelism, argumentOutOfRangeException);
                encKey = new byte[KeySizeBytes];
                Buffer.BlockCopy(combinedKey, 0, encKey, 0, KeySizeBytes);
                // macKey would be combinedKey[32..64] if needed for HMAC; not used here because we use AAD

                // Build header in-memory (authenticated as AAD)
                using var headerMs = new MemoryStream();
                headerMs.Write(FileMagic, 0, FileMagic.Length);
                headerMs.WriteByte(FormatVersion);
                headerMs.Write(salt, 0, salt.Length);

                Span<byte> intBuf = stackalloc byte[4];
                BinaryPrimitives.WriteInt32LittleEndian(intBuf, argon2MemoryKB);
                headerMs.Write(intBuf);
                BinaryPrimitives.WriteInt32LittleEndian(intBuf, argon2Iterations);
                headerMs.Write(intBuf);
                BinaryPrimitives.WriteInt32LittleEndian(intBuf, argon2Parallelism);
                headerMs.Write(intBuf);

                headerMs.Write(noncePrefix, 0, noncePrefix.Length);
                BinaryPrimitives.WriteInt32LittleEndian(intBuf, chunkSize);
                headerMs.Write(intBuf);

                byte[] headerBytes = headerMs.ToArray();

                // Prepare temp output for atomic write
                string tempPath = outputFile + ".tmp";
                using FileStream inFs = new(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                using FileStream outFs = new(tempPath, FileMode.Create, FileAccess.Write);

                // Write header to output
                outFs.Write(headerBytes, 0, headerBytes.Length);

                // Setup AES-GCM with encryption key and explicit tag size
                using var aesGcm = new AesGcm(encKey, TagSize);

                byte[] plainBuf = new byte[chunkSize];
                byte[] cipherBuf = new byte[chunkSize];
                byte[] tag = new byte[TagSize];

                Span<byte> nonce = stackalloc byte[NonceSize];

                ulong counter = 0;
                int read;
                while ((read = inFs.Read(plainBuf, 0, plainBuf.Length)) > 0)
                {
                    if (counter == ulong.MaxValue) throw new InvalidOperationException("Chunk counter exhausted — cannot encrypt more chunks with the chosen nonce scheme.");

                    // Build nonce: prefix(4) || counter(8 little-endian)
                    noncePrefix.CopyTo(nonce[..NoncePrefixSize]);
                    BinaryPrimitives.WriteUInt64LittleEndian(nonce[NoncePrefixSize..], counter);

                    // Encrypt chunk with headerBytes as AAD
                    aesGcm.Encrypt(nonce, plainBuf.AsSpan(0, read), cipherBuf.AsSpan(0, read), tag, headerBytes);

                    // Write ciphertext then tag
                    outFs.Write(cipherBuf, 0, read);
                    outFs.Write(tag, 0, tag.Length);

                    // Zero sensitive plaintext portion and tag
                    CryptographicOperations.ZeroMemory(plainBuf.AsSpan(0, read));
                    CryptographicOperations.ZeroMemory(tag);

                    counter++;
                }

                outFs.Flush(true);
                outFs.Dispose();

                // Atomic move
                File.Move(tempPath, outputFile, overwrite: true);
            }
            finally
            {
                if (encKey != null) CryptographicOperations.ZeroMemory(encKey);
                if (combinedKey != null) CryptographicOperations.ZeroMemory(combinedKey);
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(noncePrefix);
            }
        }

        public static ArgumentOutOfRangeException GetArgumentOutOfRangeException2()
        {
            return new("Argon2 parameters must be positive.");
        }

        /// <summary>
        /// Decrypt a file produced by EncryptFile (v2).
        /// Throws CryptographicException if authentication fails.
        /// </summary>
        public static void DecryptFile(string inputFile, string outputFile, string password, ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be empty.", nameof(password));
            if (!File.Exists(inputFile)) throw new FileNotFoundException("Input file not found.", inputFile);

            using FileStream inFs = new(inputFile, FileMode.Open, FileAccess.Read);

            // Read header fully into memory so it can be used as AAD
            byte[] magic = new byte[FileMagic.Length];
            ReadFull(inFs, magic, 0, magic.Length);
            if (!FixedTimeEquals(magic, FileMagic)) throw new InvalidDataException("File magic mismatch or not an encrypted file.");

            int version = inFs.ReadByte();
            if (version != FormatVersion) throw new InvalidDataException($"Unsupported format version: {version}");

            byte[] salt = new byte[SaltSize];
            ReadFull(inFs, salt, 0, salt.Length);

            int argon2MemoryKB = ReadInt32Little(inFs);
            int argon2Iterations = ReadInt32Little(inFs);
            int argon2Parallelism = ReadInt32Little(inFs);

            if (argon2MemoryKB <= 0 || argon2Iterations <= 0 || argon2Parallelism <= 0) throw argumentOutOfRangeException;
            if (argon2MemoryKB > MaxArgon2MemoryKB) throw new InvalidDataException("Argon2 memory parameter in header is unreasonable.");

            byte[] noncePrefix = new byte[NoncePrefixSize];
            ReadFull(inFs, noncePrefix, 0, noncePrefix.Length);

            int chunkSize = ReadInt32Little(inFs);
            if (chunkSize <= 0) throw new InvalidDataException("Invalid chunk size in header.");

            // Reconstruct header bytes that were written (MAGIC..chunkSize) to use as AAD
            long headerLen = FileMagic.Length + 1 + SaltSize + (4 * 3) + NoncePrefixSize + 4;
            inFs.Position = 0;
            byte[] headerBytes = new byte[headerLen];
            ReadFull(inFs, headerBytes, 0, (int)headerLen);
            // Advance stream position to after header
            inFs.Position = headerLen;

            // Derive combined key and split
            byte[]? combinedKey = null;
            byte[]? encKey = null;
            try
            {
                combinedKey = DeriveKeyArgon2id(password, salt, CombinedKeyBytes, argon2MemoryKB, argon2Iterations, argon2Parallelism, argumentOutOfRangeException);
                encKey = new byte[KeySizeBytes];
                Buffer.BlockCopy(combinedKey, 0, encKey, 0, KeySizeBytes);

                using var aesGcm = new AesGcm(encKey, TagSize);
                using FileStream outFs = new(outputFile + ".tmp", FileMode.Create, FileAccess.Write);

                byte[] cipherBuf = new byte[chunkSize];
                byte[] plainBuf = new byte[chunkSize];
                byte[] tag = new byte[TagSize];

                Span<byte> nonce = stackalloc byte[NonceSize];

                ulong counter = 0;
                while (inFs.Position < inFs.Length)
                {
                    long remaining = inFs.Length - inFs.Position;
                    if (remaining <= TagSize) throw new InvalidDataException("Corrupted file: not enough bytes for tag.");

                    int cipherBytesThisChunk = (int)Math.Min(chunkSize, remaining - TagSize);

                    ReadFull(inFs, cipherBuf, 0, cipherBytesThisChunk);
                    ReadFull(inFs, tag, 0, TagSize);

                    // Build nonce
                    noncePrefix.CopyTo(nonce[..NoncePrefixSize]);
                    BinaryPrimitives.WriteUInt64LittleEndian(nonce[NoncePrefixSize..], counter);

                    try
                    {
                        // Use headerBytes as AAD
                        aesGcm.Decrypt(nonce, cipherBuf.AsSpan(0, cipherBytesThisChunk), tag, plainBuf.AsSpan(0, cipherBytesThisChunk), headerBytes);
                    }
                    catch (CryptographicException)
                    {
                        // authentication failed
                        throw new CryptographicException("GCM authentication failed — wrong password or file tampered.");
                    }

                    outFs.Write(plainBuf, 0, cipherBytesThisChunk);

                    // Zero sensitive plaintext portion and tag
                    CryptographicOperations.ZeroMemory(plainBuf.AsSpan(0, cipherBytesThisChunk));
                    CryptographicOperations.ZeroMemory(tag);

                    if (counter == ulong.MaxValue) throw new InvalidOperationException("Chunk counter exhausted — cannot decrypt more chunks with the chosen nonce scheme.");
                    counter++;
                }

                outFs.Flush(true);
                outFs.Dispose();

                // Atomic replace
                File.Move(outFs.Name, outputFile, overwrite: true);
            }
            finally
            {
                if (encKey != null) CryptographicOperations.ZeroMemory(encKey);
                if (combinedKey != null) CryptographicOperations.ZeroMemory(combinedKey);
                try { CryptographicOperations.ZeroMemory(salt); } catch { }
                try { CryptographicOperations.ZeroMemory(noncePrefix); } catch { }
            }
        }

        // ==================================================

        static byte[] DeriveKeyArgon2id(
            string password,
            byte[] salt,
            int keyBytes,
            int memoryKb,
            int iterations,
            int parallelism,
            ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(salt);
            if (memoryKb <= 0 || iterations <= 0 || parallelism <= 0)
            {
                throw argumentOutOfRangeException;
            }

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            try
            {
                var argon = new Argon2id(passwordBytes)
                {
                    Salt = salt,
                    DegreeOfParallelism = Math.Min(parallelism, Math.Max(1, Environment.ProcessorCount)),
                    Iterations = iterations,
                    MemorySize = memoryKb
                };

                return argon.GetBytes(keyBytes);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(passwordBytes);
            }
        }

        static void ReadFull(Stream s, byte[] buffer, int offset, int count)
        {
            int read;
            while (count > 0 && (read = s.Read(buffer, offset, count)) > 0)
            {
                offset += read;
                count -= read;
            }
            if (count != 0) throw new EndOfStreamException("Failed to read expected number of bytes.");
        }

        static int ReadInt32Little(Stream s)
        {
            Span<byte> b = stackalloc byte[4];
            int read = 0;
            while (read < 4)
            {
                int r = s.Read(b[read..]);
                if (r <= 0) throw new EndOfStreamException("Failed to read expected number of bytes.");
                read += r;
            }
            return BinaryPrimitives.ReadInt32LittleEndian(b);
        }

        static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
