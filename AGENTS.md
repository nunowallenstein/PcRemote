# AGENTS.md - AI Assistant Guidance

This file provides instructions for AI assistants working on this project.

## Project Overview

**PcRemote** - A web-based remote control application to control Windows/Linux PCs from a phone browser.

## Key Files

| File | Description |
|------|-------------|
| `SKILL.md` | Detailed technical architecture, API endpoints, services, and future feature plans |
| `README.md` | User-facing documentation with setup instructions |
| `RemoteServer/` | Main application code |

## Important Context

### Architecture
- **4 Controllers**: MediaController, MouseController, KeyboardController, IndexController
- **Windows Services**: KeyPressService, WritingService (with strategy pattern), MouseInputService, MediaInputService
- **Linux Services**: KeyboardInputService, MouseInputService, MediaInputService (using ydotool)

### API Routes
| Endpoint | Controller |
|----------|------------|
| `/`, `/api/state` | IndexController |
| `/api/media/*` | MediaController |
| `/api/mouse` | MouseController |
| `/api/keyboard`, `/api/keyboard/text` | KeyboardController |

### Dependencies
- NAudio (Windows volume control)
- Windows Forms (SendKeys)
- ydotool (Linux input)

## Before Making Changes

1. **Read SKILL.md** - Contains detailed architecture and API information
2. **Check existing code patterns** - Follow the established code style
3. **Consider platform differences** - Changes may need to be applied to both Windows and Linux services

## Common Tasks

### Adding a New API Endpoint
1. Add to appropriate controller in `Controllers/`
2. Update SKILL.md API table

### Adding a New Service
1. Create interface in `Services/`
2. Implement in `Services/Windows/` and/or `Services/Linux/`
3. Register in `Program.cs`

### Modifying Frontend
- HTML: `RemoteServer/wwwroot/index.html`
- JavaScript: `RemoteServer/wwwroot/app.js`
- CSS: `RemoteServer/wwwroot/style.css`

## Testing

Run the server:
```bash
dotnet run
```

Access from phone at `http://<ip>:5260`

## Future Features

See SKILL.md for detailed plans on:
- SignalR state synchronization
- GitHub Actions release workflow
- mDNS service discovery
