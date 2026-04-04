# PcRemote Server

ASP.NET Core server for remote controlling Windows/Linux PCs via HTTP API.

## Architecture

- **4 Controllers** in `Controllers/` (MediaController, MouseController, KeyboardController, IndexController)
- **Platform-specific services** in `Services/Windows/` and `Services/Linux/`
- **Keyboard services** in `Services/Windows/KeyboardInput/` using strategy pattern
- **Platform detection** at startup via `OperatingSystem.IsWindows()` / `IsLinux()` in `Program.cs`

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | HTML remote interface |
| GET | `/api/state` | Toggle state (keyboard modifiers, mouse drag) |
| POST | `/api/media/{cmd}` | Media controls (next, prev, play, stop, mute, volup, voldown) |
| POST | `/api/media/volume` | Set volume (body: 0-100) |
| POST | `/api/mouse` | Mouse input (move, click, left, rightclick, scroll, leftdown, leftup) |
| POST | `/api/keyboard` | Keyboard input (key press, modifier toggles) |
| POST | `/api/keyboard/text` | Type text string |

## Toggle Mechanism

The app supports **toggle mode** for modifier keys and drag:

- **Hold >= 1000ms (1 second)**: Toggle ON (holds key/button)
- **Quick tap (< 1000ms)**: Toggle OFF (if currently ON) or normal press
- **NumLock**: Quick tap toggle (no hold required, different from others)
- **Delete**: Added as toggle (same as other modifiers)

### Togglable Targets

| Target | Description |
|--------|-------------|
| `shift` | Left shift key |
| `ctrl` | Left control key |
| `alt` | Left alt key |
| `win` | Left windows key |
| `numlock` | NumLock key |
| `delete` | Delete key |
| `drag` | Left mouse button drag |

## Text Typing Strategy Pattern

The `WritingService` uses two strategies for text input:

| Strategy | When Used | API |
|----------|-----------|-----|
| `KeybdTextStrategy` | Normal ASCII (≤127) | `keybd_event` |
| `SendKeysTextStrategy` | Special chars (>127) | `System.Windows.Forms.SendKeys` |

This ensures:
- Normal typing uses fast keybd_event
- Special characters (Portuguese accents, symbols) work via SendKeys
- No Alt+Numpad complexity needed

## Services

### IKeyPressService
- `KeyPress(string key)` - Single key press
- `Toggle(string target)` - Toggle modifier on/off
- `IsToggled(string target)` - Check if modifier is toggled

### IWritingService
- `TypeText(string text)` - Type string using strategy pattern

### IMouseInput
- `Move(int dx, int dy)` - Relative mouse movement
- `Click()` - Left click
- `RightClick()` - Right click
- `Scroll(int dy)` - Vertical scroll
- `LeftDown()` - Hold left button
- `LeftUp()` - Release left button

### IMediaInput
- Media controls: Next, Previous, PlayPause, Stop, Mute
- Volume controls: VolumeUp, VolumeDown, SetVolume(int level)

## Platform-Specific Details

### Windows
- Uses P/Invoke to `user32.dll`: `keybd_event`, `mouse_event`
- Key codes: VK_* constants
- Namespace: `RemoteServer.Services.Windows`
- Volume uses NAudio for precise control

### Linux
- Uses `ydotool` and `ydotoold` daemon
- Must have `ydotoold` running: `sudo apt install ydotoold`
- Key codes: `KEY_*` constants
- Namespace: `RemoteServer.Services.Linux`

## Dependencies

- ASP.NET Core 10.0
- NAudio (for Windows volume control)
- Windows: Uses Windows Forms (for SendKeys)
- Linux: requires `ydotool`/`ydotoold`

## Running

```bash
dotnet run
# Server listens on http://0.0.0.0:5260
```

## Publishing

### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish
```

## Configuration

Port configured in `Program.cs`:
```csharp
app.Urls.Add("http://0.0.0.0:5260");
```

---

## Future Features

### 1. Real-Time Multi-User State Sync

**Problem**: When multiple users control the same PC, toggle states are not synchronized. Each user's browser tracks its own local state.

**Solution**: Use **SignalR** for real-time state broadcasting.

#### Architecture
```
User A (toggle Shift) → API → Backend (update state) → SignalR Hub → All Connected Users
```

#### Implementation Plan

1. **Add SignalR NuGet package**
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.SignalR" Version="*" />
   ```

2. **Create SignalR Hub** (`Hubs/RemoteHub.cs`)

3. **Create Broadcaster Service** (`Services/ToggleBroadcaster.cs`)
   - Stores current toggle state
   - Broadcasts **full state** to all clients when anything changes
   - Provides initial state on client connect

4. **Update Services**
   - Inject broadcaster into keyboard/mouse services
   - Call broadcaster after each Toggle()

5. **Add State Endpoint**
   - `GET /api/state` - Returns current full state for initial sync

6. **Frontend** (SignalR client)
   - Connect to hub on page load
   - Request initial state
   - Listen for `StateChanged` events and update UI

#### Data Flow
1. User presses key for 1+ seconds → API receives request
2. Service updates internal toggle state
3. Broadcaster sends **full state** to SignalR Hub
4. Hub broadcasts to ALL connected clients
5. All clients update their UI in real-time

#### State Format
```json
{
  "keyboard": {
    "shift": false,
    "ctrl": false,
    "alt": false,
    "win": false,
    "numlock": false,
    "delete": false
  },
  "mouse": {
    "drag": false
  }
}
```

#### Benefits
- No polling required
- Instant state sync across all users
- Works for multi-user scenarios (teaching, remote assistance)

---

### 2. GitHub Actions Release Workflow

**Purpose**: Automatically build and publish self-contained binaries when creating releases.

#### Implementation

Create `.github/workflows/release.yml`:

```yaml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  release:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Publish Windows
        run: |
          dotnet publish -c Release -r win-x64 --self-contained true \
            -p:PublishSingleFile=true \
            -p:IncludeNativeLibrariesForSelfExtract=true \
            -o ./publish/win-x64
      
      - name: Publish Linux
        run: |
          dotnet publish -c Release -r linux-x64 --self-contained true \
            -p:PublishSingleFile=true \
            -p:IncludeNativeLibrariesForSelfExtract=true \
            -o ./publish/linux-x64
      
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: |
            ./publish/win-x64/*.exe
            ./publish/linux-x64/*
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

#### Usage
1. Create a new tag: `git tag v1.0.0`
2. Push tag: `git push origin v1.0.0`
3. GitHub Actions builds and creates release automatically
4. Users download from Releases page

#### Benefits
- No manual build required
- Consistent releases
- Both Windows and Linux binaries
- Self-contained (no .NET runtime needed)

---

### 3. mDNS Service Discovery

**Purpose**: Allow users to access via `hostname.local` instead of IP:port (e.g., `http://pcremote-win.local` instead of `http://192.168.1.100:5260`)

#### Architecture
```
User phone → http://pcremote-win.local → mDNS resolves → Windows PC IP
User phone → http://pcremote-linux.local → mDNS resolves → Linux PC IP
```

#### Implementation Plan

1. **Platform-specific hostname** (Program.cs)
   - Use `Dns.GetHostName()` with platform suffix
   - Windows: `pcremote-win`
   - Linux: `pcremote-linux`

2. **Display .local URL in console**
   - Show users both IP and hostname.local URLs

3. **Publish mDNS service** (optional, via external daemon)

#### Platform-Specific Commands

**Linux (avahi-publish)**:
```bash
avahi-publish -s "PC Remote" _http._tcp 5260 pcremote-linux
# Results in: http://pcremote-linux.local:5260
```

**Windows (dns-sd)**:
```bash
dns-sd -P "PC Remote" _http._tcp local 5260 pcremote-win
# Results in: http://pcremote-win.local:5260
```

#### Benefits
- No need to know IP address
- Works with any mDNS-aware device (most phones, tablets)
- Easy to remember URL

#### Limitations
- Requires mDNS support on network (usually works on same WiFi)
- Requires port to be specified (or use port 80)

---

### Combined Enhancement: Port 80 + mDNS

| Change | Benefit |
|--------|---------|
| Try port 80 | No port in URL |
| Display `.local` URL | Easy to remember |
| Platform suffix | Distinguish Windows/Linux |

Console output would show:
```
=======================================
PC Remote Server (linux)
=======================================
Access from your phone at:
  http://pcremote-linux.local
  http://192.168.1.101
Make sure your phone is on the same WiFi network.
=======================================
```
