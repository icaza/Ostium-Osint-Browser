Pour utiliser un disque virtuel chiffré, Ostium Osint s'appuie sur le logiciel open-source VeraCrypt (https://veracrypt.io).

# 1. Créer un disque virtuel chiffré avec VeraCrypt

- Téléchargez la version portable de VeraCrypt : https://veracrypt.io/en/Downloads.html
- Décompressez l'archive dans le répertoire de votre choix (à la racine de Ostium ou ailleurs).
- Démarrez VeraCrypt.
- Choisissez **"Create an encrypted file container"** (Créer un conteneur de fichier chiffré).
- Choisissez **"Standard VeraCrypt volume"** (Volume VeraCrypt standard).
- Sélectionnez l'emplacement de votre conteneur et choisissez un nom de fichier, par exemple `renamefile.hc` (n'oubliez pas l'extension de fichier `.hc`).
- Sélectionnez l'algorithme de chiffrement (Encryption Algorithm) et l'algorithme de hachage (KDF).
- Définissez la taille du volume (5 Go ou plus sont recommandés).
- Choisissez un mot de passe robuste.
- Sélectionnez **NTFS** comme système de fichiers (Filesystem).
- Cliquez ensuite sur **"Format"**. Votre disque virtuel chiffré est maintenant créé.

# 2. Configurer le script de démarrage

À la racine du dossier Ostium, vous trouverez un fichier nommé `Start_Secure_Container.bat`.

- Ouvrez ce fichier avec un éditeur de texte (comme le Bloc-notes).
- Au début du fichier, vous devez modifier les 4 variables suivantes pour les adapter à votre configuration :

```bat
set VERACRYPT=C:\...\VeraCrypt-x64.exe
set CONTAINER=C:\...\renamefile.hc
set DRIVE=X:\
set OSTIUM=C:\...\Ostium.exe
```

- **VERACRYPT** => Chemin vers l'exécutable de l'application VeraCrypt.
- **CONTAINER** => Chemin vers le fichier conteneur que vous venez de créer.
- **DRIVE** => Lettre du lecteur sur lequel sera monté votre volume chiffré.
- **OSTIUM** => Chemin vers l'exécutable de l'application Ostium.

**⚠️ Important!** Si vous modifiez la lettre de lecteur de la variable `DRIVE` (qui est `X:\` par défaut), vous devez impérativement reporter cette modification dans le fichier `purge.bat` situé à la racine d'Ostium. Ouvrez `purge.bat`, allez à la section finale **[9/9]**, et remplacez `X:\` par votre nouvelle lettre de lecteur. Si vous laissez la variable `DRIVE` sur `X:\`, vous n'avez pas besoin de modifier `purge.bat`.

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

# 3. Utilisation et tests

Après avoir correctement réalisé l'ensemble des opérations, vous pouvez tester le bon fonctionnement de votre configuration en double-cliquant sur le fichier `Start_Secure_Container.bat` (votre mot de passe VeraCrypt vous sera demandé). Le répertoire de travail `EnvironmentWebview` d'Ostium sera alors créé à l'intérieur du disque virtuel chiffré. 

**⚠️ Important !** Ne fermez pas la fenêtre de la console (invite de commandes). Celle-ci se fermera automatiquement lorsque vous quitterez Ostium, et le disque virtuel chiffré sera démonté de façon transparente.

Pour démarrer une session standard par défaut, sans utiliser le disque virtuel chiffré, il vous suffit de lancer Ostium en double-cliquant sur `Ostium.exe`. Le répertoire de travail `EnvironmentWebview` sera alors créé à la racine d'Ostium.

Le fonctionnement des sessions reste identique dans les deux modes : chaque session est unique et la possibilité de redémarrer sur une session existante est prise en charge dans les deux cas.

# 4. Fermeture et purge des données

**⚠️ Important !** À la fermeture, Ostium vous demande toujours si vous souhaitez supprimer les fichiers de navigation Web, les cookies, etc.

- Si vous utilisez une session dans un disque virtuel chiffré et que vous répondez "Oui" à la suppression des fichiers, **prenez soin de laisser Ostium terminer la suppression des fichiers complètement avant de confirmer le démontage du volume**. Si le volume est démonté avant que la suppression ne soit terminée, l'opération échouera car le disque ne sera plus accessible.
- Prenez également en compte que *tous* les répertoires `EnvironmentWebview` existants seront supprimés lors de cette opération (celui présent dans le disque virtuel chiffré ET celui situé à la racine d'Ostium).
- **Astuce :** Si vous utilisez une session dans un disque virtuel chiffré et que vous souhaitez supprimer le répertoire de la racine, mais *conserver* le répertoire `EnvironmentWebview` de votre disque chiffré, il vous suffit de répondre non à la suppression des fichier le volume VeraCrypt sera démonter et vous pourrez ensuite déclencher manuellement la suppression des répertoires. Ainsi, seul le répertoire situé à la racine d'Ostium sera effacé.

Pour déclencher manuellement la suppression des répertoires `EnvironmentWebview`, double-cliquez sur le fichier `purge.bat` situé à la racine d'Ostium.

**⚠️ Important !** Si vous utilisez plusieurs sessions chiffrées et que vous quittez l’une d’elles, **ne démontez pas le volume chiffré**. Le volume ne doit être démonté qu’après avoir quitté **la dernière session**.

Si vous démontez le volume par erreur alors que d’autres sessions sont encore actives, vous devrez  utiliser **la ligne de commande** ou **VeraCrypt** pour le démonter correctement.

Pour démonter un volume depuis la ligne de commande :

`C:\...\VeraCrypt-x64.exe /q /d X:`

Remplacez `X:` par la lettre correspondant à votre volume virtuel et adaptez le chemin vers `VeraCrypt-x64.exe` selon votre configuration.

# 5. Sécurité

Conservez avec soin le mot de passe de votre disque virtuel chiffré. En cas d'oubli, vous perdrez définitivement l'accès à toutes les données qui y sont sauvegardées.