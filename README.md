# PC Remote - Control Your PC from Your Phone

Control your Windows PC remotely from your phone using a web-based touchpad and keyboard interface.

## Features

- **Touchpad**: Move cursor, click (tap), and drag (hold drag button + touch)
- **Mouse Buttons**: Left click, right click
- **Scroll**: Up/Down buttons
- **Media Controls**: Play/Pause, Next, Previous, Volume, Mute
- **Keyboard**: Special keys (ESC, TAB, Enter, etc.) + modifier keys (Win, Alt, Ctrl, Shift)
- **Text Input**: Type text from your phone's keyboard
- **Drag & Drop**: Hold the Drag button while moving on touchpad to drag items

## Quick Start

### 1. Run the Server

```bash
cd RemoteServer
dotnet run
```

The server will start on `http://0.0.0.0:5260`

### 2. Allow Firewall (Important!)

Windows Firewall will block external connections by default. To allow:

1. Open **Windows Defender Firewall** (search in Start menu)
2. Click **"Allow an app through firewall"**
3. Click **"Change settings"** → **"Allow another app..."**
4. Browse to `RemoteServer\bin\Debug\net10.0\RemoteServer.exe`
5. Check both **Private** and **Public** boxes
6. Click **OK**

### 3. Find Your PC's IP Address

1. Open **Command Prompt** (Win + R, type `cmd`, Enter)
2. Run: `ipconfig`
3. Look for **IPv4 Address** under your active network adapter (usually "Wireless LAN adapter Wi-Fi")

Example output:
```
Wireless LAN adapter Wi-Fi:

   IPv4 Address. . . . . . . . . . . : 192.168.1.100
```

### 4. Access from Phone

1. Connect your phone to the **same WiFi network** as your PC
2. Open browser on phone and go to:
   ```
   http://192.168.1.100:5260
   ```
   (Replace `192.168.1.100` with your actual IP address)

3. The remote interface should load

## Controls

### Touchpad
| Action | Effect |
|--------|--------|
| Tap | Left click |
| Move finger | Move cursor |
| Hold Drag button + touch | Drag (move with left button held) |

### Media Controls
- Play/Pause: Toggle play/pause
- Prev/Next: Previous/Next track
- Volume Up/Down: Adjust volume
- Mute: Toggle mute

### Keyboard
- **Text Input**: Type in the text box, tap "Send" to type on PC
- **Modifier Keys**: Win (⊞), Alt, Ctrl, Shift
- **Special Keys**: ESC, TAB, Enter, Backspace, Space, Arrow keys

## Troubleshooting

### Phone can't connect
1. **Verify firewall is open** (see step 2 above)
2. **Check IP address** - make sure you're using the correct IPv4 address
3. **Restart the server** after making changes
4. **Try ping** from phone's browser - should show timeout if firewall issue, or different error if network issue

### Server not listening on network
Make sure you're running with:
```json
"applicationUrl": "http://0.0.0.0:5260"
```
in `Properties/launchSettings.json`

If running via command line:
```bash
dotnet run --urls=http://0.0.0.0:5260
```

### Still not working?
- Ensure phone and PC are on the **same WiFi network**
- Check if any antivirus software is blocking connections
- Try accessing from PC browser first: `http://localhost:5260`

## Building

```bash
# Build for release
dotnet publish -c Release
```

The executable will be in `bin\Release\net10.0\publish\`

## Running as Windows Service (Optional)

To run on startup without manual launch, use [NSSM](https://nssm.cc/):

1. Download NSSM
2. Run: `nssm install RemoteServer "C:\path\to\RemoteServer.exe"`
3. Set AppDirectory to the folder containing the exe
4. Start: `nssm start RemoteServer`