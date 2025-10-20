# PostIt Explorer — Cloud Sync (Phase 1.5)

Windows tray app to add “post-it” notes to files/folders (Ctrl+Space), with **Google/Microsoft sign-in** and **cloud backup**.
- Local-first (JSON at `%AppData%/PostItExplorer/data.json`).
- Optional backup to **Google Drive (AppData)** and/or **OneDrive (Application folder)**.
- Tokens are encrypted via **Windows DPAPI** per user.
- Still no native Explorer column/preview; that’s Phase 2.

## No-code build (same as before)
1) Create a GitHub repo and upload this folder.  
2) In **Actions**, run **Build Windows EXE**.  
3) Download artifacts → `PostItExplorer.exe`.

## Configure sign-in (runtime)
Open the tray menu → **Settings** → **Accounts**:
- For **Google**: paste *Client ID* and *Client Secret* of a **Desktop** OAuth client. Click **Sign in with Google**.  
  Scopes used: `openid email profile https://www.googleapis.com/auth/drive.appdata`
- For **Microsoft**: paste *Application (client) ID*. Click **Sign in with Microsoft**.  
  Scopes used: `offline_access User.Read Files.ReadWrite.AppFolder`

If you don’t have IDs yet, the Settings screen shows plain-English steps. You can also leave these blank; the app will work locally without cloud backup.

## What you’ll see
- **Ctrl+Space** on a selected file/folder opens a Post-it modal (color, label, text).  
- **Dashboard** lists and searches notes. **Open** jumps to the file in Explorer.  
- If signed in, a background sync pushes `notes.json` to Google Drive AppData and/or OneDrive App Root every ~60 seconds or when you Save.

## Building locally (optional)
- Requires .NET 8 SDK.  
- `dotnet publish PostItExplorer -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false`

## Roadmap (Phase 2)
- Native **Context Menu Handler** + **Custom Column/Preview** for reliable Explorer integration.  
- Switch storage to **SQLite + FTS** for large-scale speed.  
- MSI installer & code-signing.
