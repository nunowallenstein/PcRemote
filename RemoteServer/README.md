# PC Remote

A web-based remote control application that allows you to control your Windows or Linux PC from your phone browser. The app simulates keyboard, mouse, and media inputs on the host machine.

## Features

### Mouse Control
- **Touchpad** - Move cursor by dragging on the touchpad area
- **Click** - Tap to click (single click)
- **Double-click** - Two quick taps for double-click
- **Right-click** - Dedicated right button
- **Drag mode** - Toggle drag mode to click and drag files/selection
- **Scroll** - Up/down buttons for scrolling

### Keyboard Control
- **Text typing** - Type text on your phone, sends to PC
- **Special characters** - Portuguese accents (á, à, â, ã, é, etc.) and special symbols (/, ?, <, >, {, }, |, ", ', ~)
- **Modifier keys** - Shift, Ctrl, Alt, Windows, Delete
- **Toggle mode** - Hold for 1 second to toggle modifier ON, quick tap to toggle OFF
- **NumLock** - Quick tap toggle (different from other modifiers)

### Media Controls
- **Playback** - Previous, Play/Pause, Next, Stop
- **Volume** - Slider to set volume 0-100%
- **Mute** - Toggle mute

### Additional Keys
- Arrow keys, Enter, Escape, Tab, Backspace, Delete, Insert, Home, End, Page Up/Down

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
- `KeyboardInputService` - Uses ydotool for input
- `MouseInputService` - Uses ydotool for input
- `MediaInputService` - Uses ydotool for media keys

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

## Building and Running

### Prerequisites
- .NET 10.0 SDK
- For Windows: NAudio package (included)
- For Linux: ydotool (`sudo apt install ydotoold`)

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

The server will start on port 5260 and display the IP addresses to access from your phone.

### Publish (Windows)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

## Usage

1. Run the server on your PC
2. Access `http://<your-ip>:5260` from your phone browser (must be on same WiFi)
3. Use the touchpad to control the mouse
4. Use the keyboard section to type text or send special keys
5. Use media controls for playback and volume

## Known Limitations

- Volume slider uses NAudio on Windows (may require admin for first run)
- Some special characters may not work in all applications
- Linux requires ydotool to be installed and running

## License

MIT
