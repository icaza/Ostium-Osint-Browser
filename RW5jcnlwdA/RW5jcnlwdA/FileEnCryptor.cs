using Konscious.Security.Cryptography;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace RW5jcnlwdA
{
    public class FileEnCryptor
    {
        // === Constants / defaults ===
        const int SaltSize = 32;               // bytes
        const int NonceSize = 12;              // AES-GCM nonce size (96 bits) - now fully random
        const int KeySizeBytes = 32;           // AES-256 per subkey
        const int CombinedKeyBytes = 64;       // Argon2 output bytes to split into enc/mac
        const int TagSize = 16;                // GCM tag size (128 bits)
        const int DefaultChunkSize = 64 * 1024; // 64 KiB chunk plaintext size
        const int MaxChunkSize = 1024 * 1024;  // 1 MB maximum chunk size to prevent DoS

        // Argon2 defaults (adjust with care). MemorySize is in KB.
        const int Argon2MemoryKB_Default = 262_144; // 256 MB
        const int Argon2Iterations_Default = 3;     // passes
        const int Argon2Parallelism_Default = 2;
        const int MaxArgon2MemoryKB = 1_048_576;    // 1 GB hard cap to avoid abusive allocations
        const int MinArgon2MemoryKB = 1024;         // 1 MB minimum to ensure reasonable security

        // Magic & version to detect format
        static readonly byte[] FileMagic = "FENC"u8.ToArray(); // "FENC"
        const byte FormatVersion = 3; // bumped to v3: improved nonce generation, better validation

        // ==================================================

        /// <summary>
        /// Encrypt a file with AES-GCM + Argon2id key derivation (streaming by chunks).
        /// Produces file format (v3):
        /// [MAGIC(4)] [VERSION(1)]
        /// [salt(32)]
        /// [argon2: memoryKB(int32)][iterations(int32)][parallelism(int32)]
        /// [chunkSize(int32 little-endian)]
        /// [ciphertext chunk0][nonce0(12)][tag0][ciphertext chunk1][nonce1(12)][tag1]...
        /// Header is authenticated as AAD for each chunk.
        /// Each chunk has its own random nonce for maximum security.
        /// </summary>
        public static void EncryptFile(
            string inputFile,
            string outputFile,
            string password,
            int chunkSize = DefaultChunkSize,
            int argon2MemoryKB = Argon2MemoryKB_Default,
            int argon2Iterations = Argon2Iterations_Default,
            int argon2Parallelism = Argon2Parallelism_Default)
        {
            // Input validation
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("Input file path cannot be empty.", nameof(inputFile));
            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentException("Output file path cannot be empty.", nameof(outputFile));

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
            if (chunkSize > MaxChunkSize)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), $"Chunk size too large. Max {MaxChunkSize} bytes");

            if (!File.Exists(inputFile))
                throw new FileNotFoundException("Input file not found.", inputFile);

            // Validate Argon2 parameters
            if (argon2MemoryKB < MinArgon2MemoryKB || argon2MemoryKB > MaxArgon2MemoryKB)
                throw new ArgumentOutOfRangeException(nameof(argon2MemoryKB),
                    $"Argon2 memory must be between {MinArgon2MemoryKB} KB and {MaxArgon2MemoryKB} KB");
            if (argon2Iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(argon2Iterations), "Argon2 iterations must be positive");
            if (argon2Parallelism <= 0)
                throw new ArgumentOutOfRangeException(nameof(argon2Parallelism), "Argon2 parallelism must be positive");

            // Prepare header values
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            // Derive combined key via Argon2id (64 bytes) then split
            byte[]? combinedKey = null;
            byte[]? encKey = null;
            string? tempPath = null;

            try
            {
                combinedKey = DeriveKeyArgon2id(password, salt, CombinedKeyBytes, argon2MemoryKB, argon2Iterations, argon2Parallelism);
                encKey = new byte[KeySizeBytes];
                Array.Copy(combinedKey, 0, encKey, 0, KeySizeBytes);
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
                BinaryPrimitives.WriteInt32LittleEndian(intBuf, chunkSize);
                headerMs.Write(intBuf);

                byte[] headerBytes = headerMs.ToArray();

                // Create secure temporary file path
                tempPath = GenerateSecureTempPath(outputFile);

                using FileStream inFs = new(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                using FileStream outFs = new(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);

                // Write header to output
                outFs.Write(headerBytes, 0, headerBytes.Length);

                // Setup AES-GCM with encryption key and explicit tag size
                using var aesGcm = new AesGcm(encKey, TagSize);

                byte[] plainBuf = new byte[chunkSize];
                byte[] cipherBuf = new byte[chunkSize];
                byte[] tag = new byte[TagSize];
                byte[] nonce = new byte[NonceSize]; // Each chunk gets its own random nonce

                ulong chunkCount = 0;
                int read;
                while ((read = inFs.Read(plainBuf, 0, plainBuf.Length)) > 0)
                {
                    if (chunkCount == ulong.MaxValue)
                        throw new InvalidOperationException("Too many chunks — file size exceeds maximum supported limit.");

                    // Generate completely random nonce for this chunk
                    RandomNumberGenerator.Fill(nonce);

                    // Encrypt chunk with headerBytes as AAD
                    aesGcm.Encrypt(nonce, plainBuf.AsSpan(0, read), cipherBuf.AsSpan(0, read), tag, headerBytes);

                    // Write ciphertext, then nonce, then tag (nonce stored per chunk for v3 format)
                    outFs.Write(cipherBuf, 0, read);
                    outFs.Write(nonce, 0, nonce.Length);
                    outFs.Write(tag, 0, tag.Length);

                    // Zero sensitive data
                    CryptographicOperations.ZeroMemory(plainBuf.AsSpan(0, read));
                    CryptographicOperations.ZeroMemory(tag);
                    CryptographicOperations.ZeroMemory(nonce);

                    chunkCount++;
                }

                // Ensure data is written to disk before atomic move
                outFs.Flush(true);
                outFs.Close(); // Explicit close before move

                // Atomic move - this is thread-safe on most filesystems
                File.Move(tempPath, outputFile, overwrite: true);
                tempPath = null; // Successfully moved, don't clean up
            }
            finally
            {
                // Secure cleanup
                if (encKey != null)
                    CryptographicOperations.ZeroMemory(encKey);
                if (combinedKey != null)
                    CryptographicOperations.ZeroMemory(combinedKey);
                CryptographicOperations.ZeroMemory(salt);

                // Clean up temp file if operation failed
                if (tempPath != null && File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { /* Best effort cleanup */ }
                }
            }
        }

        /// <summary>
        /// Decrypt a file produced by EncryptFile (v3 format).
        /// Throws CryptographicException if authentication fails.
        /// </summary>
        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            // Input validation
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("Input file path cannot be empty.", nameof(inputFile));
            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentException("Output file path cannot be empty.", nameof(outputFile));
            if (!File.Exists(inputFile))
                throw new FileNotFoundException("Input file not found.", inputFile);

            string? tempPath = null;

            using FileStream inFs = new(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                // Read and validate header
                byte[] magic = new byte[FileMagic.Length];
                ReadFull(inFs, magic, 0, magic.Length);
                if (!FixedTimeEquals(magic, FileMagic))
                    throw new InvalidDataException("File magic mismatch or not an encrypted file.");

                int version = inFs.ReadByte();
                if (version == -1) throw new InvalidDataException("Unexpected end of file reading version.");

                // Support both v2 and v3 formats for backward compatibility
                if (version != FormatVersion && version != 2)
                    throw new InvalidDataException($"Unsupported format version: {version}");

                byte[] salt = new byte[SaltSize];
                ReadFull(inFs, salt, 0, salt.Length);

                int argon2MemoryKB = ReadInt32Little(inFs);
                int argon2Iterations = ReadInt32Little(inFs);
                int argon2Parallelism = ReadInt32Little(inFs);

                // Validate Argon2 parameters from file
                if (argon2MemoryKB < MinArgon2MemoryKB || argon2MemoryKB > MaxArgon2MemoryKB ||
                    argon2Iterations <= 0 || argon2Parallelism <= 0)
                    throw new InvalidDataException("Invalid Argon2 parameters in file header.");

                int chunkSize;
                long headerLen;

                if (version == 2)
                {
                    // v2 format: read noncePrefix then chunkSize
                    byte[] noncePrefix = new byte[4]; // v2 nonce prefix size
                    ReadFull(inFs, noncePrefix, 0, noncePrefix.Length);
                    chunkSize = ReadInt32Little(inFs);
                    headerLen = FileMagic.Length + 1 + SaltSize + (4 * 3) + 4 + 4; // includes noncePrefix
                }
                else
                {
                    // v3 format: only chunkSize (no noncePrefix)
                    chunkSize = ReadInt32Little(inFs);
                    headerLen = FileMagic.Length + 1 + SaltSize + (4 * 4); // 4 int32s for argon params + chunkSize
                }

                if (chunkSize <= 0 || chunkSize > MaxChunkSize)
                    throw new InvalidDataException($"Invalid chunk size in header: {chunkSize}");

                // Reconstruct header bytes for AAD
                long currentPos = inFs.Position;
                inFs.Position = 0;
                byte[] headerBytes = new byte[headerLen];
                ReadFull(inFs, headerBytes, 0, (int)headerLen);
                inFs.Position = currentPos; // Restore position after header

                // Derive keys
                byte[]? combinedKey = DeriveKeyArgon2id(password, salt, CombinedKeyBytes, argon2MemoryKB, argon2Iterations, argon2Parallelism);
                byte[]? encKey = null;

                try
                {
                    encKey = new byte[KeySizeBytes];
                    Array.Copy(combinedKey, 0, encKey, 0, KeySizeBytes);

                    using var aesGcm = new AesGcm(encKey, TagSize);

                    // Create secure temporary file
                    tempPath = GenerateSecureTempPath(outputFile);
                    using FileStream outFs = new(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);

                    byte[] cipherBuf = new byte[chunkSize];
                    byte[] plainBuf = new byte[chunkSize];
                    byte[] tag = new byte[TagSize];
                    byte[] nonce = new byte[NonceSize];

                    ulong chunkCount = 0;
                    while (inFs.Position < inFs.Length)
                    {
                        long remaining = inFs.Length - inFs.Position;
                        int bytesNeeded = NonceSize + TagSize;
                        if (remaining < bytesNeeded)
                            throw new InvalidDataException("Corrupted file: not enough bytes for nonce and tag.");

                        int cipherBytesThisChunk = (int)Math.Min(chunkSize, remaining - bytesNeeded);
                        if (cipherBytesThisChunk <= 0)
                            throw new InvalidDataException("Corrupted file: invalid chunk size calculation.");

                        ReadFull(inFs, cipherBuf, 0, cipherBytesThisChunk);

                        if (version == 3)
                        {
                            // v3: read nonce per chunk
                            ReadFull(inFs, nonce, 0, NonceSize);
                        }
                        else
                        {
                            // v2: reconstruct nonce from prefix + counter
                            // Note: This is simplified - in production you'd need the noncePrefix from header
                            throw new NotImplementedException("v2 format decryption requires additional implementation for nonce reconstruction");
                        }

                        ReadFull(inFs, tag, 0, TagSize);

                        try
                        {
                            aesGcm.Decrypt(nonce, cipherBuf.AsSpan(0, cipherBytesThisChunk), tag,
                                         plainBuf.AsSpan(0, cipherBytesThisChunk), headerBytes);
                        }
                        catch (CryptographicException)
                        {
                            throw new CryptographicException("GCM authentication failed — wrong password or file tampered.");
                        }

                        outFs.Write(plainBuf, 0, cipherBytesThisChunk);

                        // Zero sensitive data
                        CryptographicOperations.ZeroMemory(plainBuf.AsSpan(0, cipherBytesThisChunk));
                        CryptographicOperations.ZeroMemory(tag);
                        CryptographicOperations.ZeroMemory(nonce);

                        chunkCount++;
                    }

                    outFs.Flush(true);
                    outFs.Close(); // Explicit close before move

                    // Atomic replace
                    File.Move(tempPath, outputFile, overwrite: true);
                    tempPath = null; // Successfully moved
                }
                finally
                {
                    if (encKey != null)
                        CryptographicOperations.ZeroMemory(encKey);
                    if (combinedKey != null)
                        CryptographicOperations.ZeroMemory(combinedKey);
                    CryptographicOperations.ZeroMemory(salt);
                }
            }
            finally
            {
                // Clean up temp file if operation failed
                if (tempPath != null && File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { /* Best effort cleanup */ }
                }
            }
        }

        // ==================================================
        // Helper Methods
        // ==================================================

        /// <summary>
        /// Generate a secure temporary file path to prevent race conditions and predictable names.
        /// </summary>
        static string GenerateSecureTempPath(string basePath)
        {
            string directory = Path.GetDirectoryName(basePath) ?? Path.GetTempPath();
            string fileName = Path.GetFileNameWithoutExtension(basePath);
            string extension = Path.GetExtension(basePath);

            // Use cryptographically secure random filename
            string randomSuffix = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
            return Path.Combine(directory, $"{fileName}_{randomSuffix}_temp{extension}");
        }

        /// <summary>
        /// Derive key using Argon2id with secure parameter validation.
        /// </summary>
        static byte[] DeriveKeyArgon2id(
            string password,
            byte[] salt,
            int keyBytes,
            int memoryKb,
            int iterations,
            int parallelism)
        {
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(salt);

            if (memoryKb < MinArgon2MemoryKB || memoryKb > MaxArgon2MemoryKB)
                throw new ArgumentOutOfRangeException(nameof(memoryKb), "Invalid Argon2 memory parameter");
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "Argon2 iterations must be positive");
            if (parallelism <= 0)
                throw new ArgumentOutOfRangeException(nameof(parallelism), "Argon2 parallelism must be positive");

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

        /// <summary>
        /// Read exactly the specified number of bytes or throw an exception.
        /// </summary>
        static void ReadFull(Stream s, byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(buffer);

            if (offset < 0 || count < 0 || offset + count > buffer.Length)
                throw new ArgumentException("Invalid buffer parameters");

            int read;
            while (count > 0 && (read = s.Read(buffer, offset, count)) > 0)
            {
                offset += read;
                count -= read;
            }
            if (count != 0)
                throw new EndOfStreamException($"Failed to read expected number of bytes. Missing: {count}");
        }

        /// <summary>
        /// Read a 32-bit little-endian integer from stream.
        /// </summary>
        static int ReadInt32Little(Stream s)
        {
            ArgumentNullException.ThrowIfNull(s);

            Span<byte> buffer = stackalloc byte[4];
            int read = 0;
            while (read < 4)
            {
                int r = s.Read(buffer[read..]);
                if (r <= 0) throw new EndOfStreamException("Failed to read Int32 from stream");
                read += r;
            }
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        }

        /// <summary>
        /// Constant-time byte array comparison to prevent timing attacks.
        /// </summary>
        static bool FixedTimeEquals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}