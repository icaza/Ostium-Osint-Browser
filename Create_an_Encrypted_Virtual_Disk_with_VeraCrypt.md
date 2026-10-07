To use an encrypted virtual disk, Ostium Osint utilizes the open-source software VeraCrypt (https://veracrypt.io).

# 1. Create an Encrypted Virtual Disk with VeraCrypt

- Download the portable version of VeraCrypt: https://veracrypt.io/en/Downloads.html
- Extract the archive to a location of your choice (at the root of Ostium or elsewhere).
- Start VeraCrypt.
- Choose **"Create an encrypted file container"**.
- Choose **"Standard VeraCrypt volume"**.
- Select the destination path for your container and choose a filename, such as `renamefile.hc` (do not forget the `.hc` file extension).
- Select the Encryption Algorithm and the Hash Algorithm (KDF).
- Set the volume size (5GB or more is recommended).
- Choose a strong password.
- Select **NTFS** as the filesystem.
- Click on **"Format"**. Your encrypted virtual disk is now created.

# 2. Configure the Startup Script

At the root of the Ostium folder, you will find a file named `Start_Secure_Container.bat`.

- Open this file with a text editor (e.g., Notepad).
- At the beginning of the file, you must modify the following 4 variables to match your configuration:

```bat
set VERACRYPT=C:\...\VeraCrypt-x64.exe
set CONTAINER=C:\...\renamefile.hc
set DRIVE=X:\
set OSTIUM=C:\...\Ostium.exe
```

- **VERACRYPT** => Path to the VeraCrypt application executable.
- **CONTAINER** => Path to the container file you just created.
- **DRIVE** => Drive letter where your encrypted volume will be mounted.
- **OSTIUM** => Path to the Ostium application executable.

**⚠️ Important!** If you change the drive letter of the `DRIVE` variable (which is `X:\` by default), you must strictly apply this change in the `purge.bat` file located at the root of Ostium. Open `purge.bat`, go to the final section **[9/9]**, and replace `X:\` with your new drive letter. If you leave the `DRIVE` variable as `X:\`, you do not need to modify `purge.bat`.

```bat
echo [9/9] Deletion of the secure EnvironmentWebview folder...
if exist "X:\EnvironmentWebview" (
    rd /s /q "X:\EnvironmentWebview" 2>nul
    if errorlevel 1 (
        echo [ERROR] Unable to delete the folder
        set /a ErrorCount=%ErrorCount%+1
    ) else (
        if not exist "X:\EnvironmentWebview" (
            echo [OK] File deleted
        ) else (
            echo [ERROR] The file still exists
            set /a ErrorCount=%ErrorCount%+1
        )
    )
) else (
    echo [INFO] File not found
)
```

# 3. Usage and Testing

After correctly completing all the steps, you can test if your configuration works by double-clicking the `Start_Secure_Container.bat` file (you will be prompted for your VeraCrypt password). Ostium's `EnvironmentWebview` working directory will then be created inside the encrypted virtual disk.

**⚠️ Important!** Do not close the console window (Command Prompt). It will close automatically when you exit Ostium, and the encrypted virtual disk will be unmounted seamlessly.

To start a default standard session without using the encrypted virtual disk, simply run Ostium by double-clicking `Ostium.exe`. The `EnvironmentWebview` working directory will then be created at the root of Ostium.

Session management works identically in both modes: each session is unique, and the ability to restart an existing session is supported in both cases.

# 4. Closing and Purging Data

**⚠️ Important!** Upon closing, Ostium always asks if you want to delete web browsing files, cookies, etc.

- If you are using a session within an encrypted virtual disk and you answer "Yes" to deleting files, **make sure to let Ostium finish deleting the files completely before confirming the volume dismount**. If the volume is unmounted before the deletion is finished, the operation will fail because the disk will no longer be accessible.
- Also, keep in mind that *all* existing `EnvironmentWebview` directories will be deleted during this process (both the one in the encrypted virtual disk AND the one at the root of Ostium).
- **Tip:** If you are using a session within an encrypted virtual disk and wish to delete the root directory but *keep* the `EnvironmentWebview` directory on your encrypted disk, simply decline the file deletion prompt; the VeraCrypt volume will unmount, and you can then manually trigger the deletion of the directories. This way, only the directory located at the root of Ostium will be erased.

To manually trigger the deletion of the `EnvironmentWebview` directories, double-click the `purge.bat` file located at the root of Ostium.

**⚠️ Important!** If you are using multiple encrypted sessions and close one of them, **do not dismount the encrypted volume**. The volume should only be dismounted after closing **the last session**.

If you dismount the volume by mistake while other sessions are still active, you will need to use **the command line** or **VeraCrypt** to dismount it correctly.

To dismount a volume from the command line:

`C:\...\VeraCrypt-x64.exe /q /d X:`

Replace `X:` with the drive letter corresponding to your virtual volume and adjust the path to `VeraCrypt-x64.exe` according to your configuration.

# 5. Security

Keep your encrypted virtual disk password safe. If you forget it, you will permanently lose access to all the data saved on it.