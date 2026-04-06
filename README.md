# PC Remote - Control Your PC from Your Phone

Control your Windows or Linux PC remotely from your phone using a web-based touchpad and keyboard interface.

## Features

### Mouse Control
- **Touchpad** - Move cursor by dragging (uses Pointer Events for unified touch/mouse handling)
- **Click** - Tap to click (single click, no duplicates)
- **Double-click** - Two quick taps
- **Right-click** - Dedicated right button
- **Drag mode** - Toggle drag mode to click and drag files/selection
- **Scroll** - Up/down buttons for scrolling

### Keyboard Control
- **Text typing** - Type text on your phone, sends to PC using strategy pattern:
  - Normal ASCII characters: Uses `keybd_event` API
  - Special characters (>127): Uses `SendKeys` API
- **Special characters** - Portuguese accents (á, à, â, ã, é, etc.) and special symbols (/, ?, <, >, {, }, |, ", ', ~)
- **Modifier keys** - Shift, Ctrl, Alt, Windows, Delete
- **Toggle mode** - Hold for 1 second to toggle modifier ON, quick tap to toggle OFF
- **NumLock** - Quick tap toggle (different from other modifiers)
- **Additional keys** - Arrow keys, Enter, Escape, Tab, Backspace, Delete, Insert, Home, End, Page Up/Down

### Media Controls
- **Playback** - Previous, Play/Pause, Next, Stop
- **Volume** - Slider to set volume 0-100% (using NAudio on Windows)
- **Mute** - Toggle mute

## Prerequisites

- .NET 10.0 SDK
- For Windows: NAudio package (included)
- For Linux: ydotool (`sudo apt install ydotoold`)

## Quick Start

### 1. Run the Server

```bash
cd RemoteServer
dotnet run
```

The server will start on `http://0.0.0.0:5260` and display IP addresses to access from your phone.

### 2. Allow Firewall (Windows)

Windows Firewall will block external connections by default. To allow:

1. Open **Windows Defender Firewall**
2. Click **"Allow an app through firewall"**
3. Click **"Change settings"** → **"Allow another app..."**
4. Browse to the published exe
5. Check both **Private** and **Public** boxes
6. Click **OK**

### 3. Access from Phone

1. Connect your phone to the **same WiFi network** as your PC
2. Open browser on phone and go to: `http://<your-ip>:5260`
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
- Volume Slider: Drag to set volume 0-100%
- Mute: Toggle mute

### Keyboard
- **Text Input**: Type in the text box, tap "Send" to type on PC
- **Modifier Keys**: Win (⊞), Alt, Ctrl, Shift, Delete - Hold 1s to toggle ON, tap to toggle OFF
- **Special Keys**: ESC, TAB, Enter, Backspace, Space, Arrow keys

## Architecture

### Controllers (API Endpoints)

| Controller | Route | Description |
|------------|-------|-------------|
| `IndexController` | `/`, `/api/state` | Serves HTML UI and toggle state |
| `MediaController` | `/api/media` | Media controls (play, pause, volume, etc.) |
| `MouseController` | `/api/mouse` | Mouse movement, clicks, scroll |
| `KeyboardController` | `/api/keyboard` | Key presses and text typing |

### Services (Platform-Specific)

#### Windows Services
| Service | Description |
|---------|-------------|
| `KeyPressService` | Single key press and toggle logic |
| `WritingService` | Text typing with strategy pattern |
| `MouseInputService` | Mouse movement, clicks, scroll |
| `MediaInputService` | Media keys and volume control (using NAudio) |

#### Text Strategies (WritingService)
| Strategy | Description |
|----------|-------------|
| `KeybdTextStrategy` | Uses keybd_event for normal ASCII characters |
| `SendKeysTextStrategy` | Uses SendKeys for special characters (>127) |

#### Linux Services
| Service | Description |
|---------|-------------|
| `KeyboardInputService` | Uses ydotool for input |
| `MouseInputService` | Uses ydotool for input |
| `MediaInputService` | Uses ydotool for media keys |

## Building and Publishing

### Build
```bash
dotnet build
```

### Publish (Windows)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

### Publish (Linux)
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish
```

## Linux Requirements

For Linux, `ydotool` must be installed:
```bash
sudo apt install ydotoold
```

## Usage

1. Run the server on your PC
2. Access `http://<your-ip>:5260` from your phone browser (must be on same WiFi)
3. Use the touchpad to control the mouse
4. Use the keyboard section to type text or send special keys
5. Use media controls for playback and volume

## Known Limitations

- Volume slider uses NAudio on Windows
- Some special characters may not work in all applications
- Linux requires ydotool to be installed and running

## API Endpoints

### Index
```
GET /                    # Returns HTML UI
GET /api/state          # Returns toggle states
```

### Media
```
POST /api/media/next        # Next track
POST /api/media/prev        # Previous track
POST /api/media/play       # Play/Pause toggle
POST /api/media/stop       # Stop
POST /api/media/mute       # Mute toggle
POST /api/media/volup      # Volume up
POST /api/media/voldown    # Volume down
POST /api/media/volume     # Set volume (body: 0-100)
```

### Mouse
```
POST /api/mouse
{
    action: "move" | "click" | "left" | "leftdown" | "leftup" | "right" | "scroll",
    dx?: number,
    dy?: number,
    holdDuration?: number  # 0 = quick untoggle, >=1000 = long press toggle
}
```

### Keyboard
```
POST /api/keyboard
{
    key: string,
    holdDuration?: number  # 0 = quick untoggle, >=1000 = long press toggle
}

POST /api/keyboard/text
{
    text: string
}
```

## Troubleshooting

### Phone can't connect
1. **Verify firewall is open**
2. **Check IP address** - make sure you're using the correct IPv4 address
3. **Restart the server** after making changes

### Server not listening on network
Make sure you're running with `http://0.0.0.0:5260`

### Volume slider not working (Windows)

## Project Structure

```
RemoteServer/
├── Controllers/
│   ├── IndexController.cs      # HTML UI + state endpoint
│   ├── MediaController.cs      # Media controls
│   ├── MouseController.cs      # Mouse controls
│   └── KeyboardController.cs   # Keyboard controls
├── Services/
│   ├── IMediaInput.cs          # Media interface
│   ├── IMouseInput.cs          # Mouse interface
│   ├── ITogglable.cs           # Toggle interface
│   ├── Windows/
│   │   ├── KeyboardInput/
│   │   │   ├── TextStrategies/
│   │   │   │   ├── ITextStrategy.cs
│   │   │   │   ├── KeybdTextStrategy.cs
│   │   │   │   └── SendKeysTextStrategy.cs
│   │   │   ├── KeyPressService.cs
│   │   │   ├── WritingService.cs
│   │   │   ├── KeyMapper.cs
│   │   │   └── KeyCharHelper.cs
│   │   ├── MouseInputService.cs
│   │   └── MediaInputService.cs
│   └── Linux/
│       ├── KeyboardInputService.cs
│       ├── MouseInputService.cs
│       └── MediaInputService.cs
├── Models/
│   ├── KeyboardCommand.cs
│   ├── MouseCommand.cs
│   └── TextCommand.cs
├── wwwroot/
│   ├── index.html
│   ├── app.js
│   └── style.css
├── Program.cs
└── RemoteServer.csproj
```

## License

MIT
