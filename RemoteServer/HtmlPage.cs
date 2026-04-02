namespace RemoteServer;

public static class HtmlPage
{
    public const string Content = """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <title>PC Remote</title>
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; }
        body { font-family: -apple-system, BlinkMacSystemFont, sans-serif; background: #1a1a2e; color: #fff; min-height: 100vh; padding: 10px; }
        .container { max-width: 400px; margin: 0 auto; }
        h1 { text-align: center; font-size: 1.2rem; margin-bottom: 15px; color: #00d4ff; }
        .section { background: #16213e; border-radius: 12px; padding: 15px; margin-bottom: 15px; }
        .section-title { font-size: 0.75rem; color: #888; margin-bottom: 10px; text-transform: uppercase; }
        .grid { display: grid; gap: 8px; }
        .media-grid { grid-template-columns: repeat(3, 1fr); }
        .btn { background: #0f3460; border: none; border-radius: 8px; padding: 15px; font-size: 1rem; color: #fff; cursor: pointer; transition: all 0.15s; -webkit-tap-highlight-color: transparent; }
        .btn:active { background: #00d4ff; color: #1a1a2e; transform: scale(0.95); }
        .btn-lg { padding: 20px; font-size: 1.5rem; }
        .btn-play { background: #e94560; }
        .touchpad { width: 100%; height: 200px; background: #0f3460; border-radius: 12px; margin-bottom: 10px; position: relative; overflow: hidden; }
        .touchpad-indicator { position: absolute; width: 40px; height: 40px; background: rgba(0,212,255,0.3); border-radius: 50%; transform: translate(-50%, -50%); pointer-events: none; }
        .touchpad-dot { position: absolute; width: 10px; height: 10px; background: #00d4ff; border-radius: 50%; transform: translate(-50%, -50%); top: 50%; left: 50%; pointer-events: none; }
        .click-btns { display: flex; gap: 10px; }
        .click-btns .btn { flex: 1; }
        .key-grid { display: grid; grid-template-columns: repeat(5, 1fr); gap: 5px; }
        .key-btn { padding: 12px 5px; font-size: 0.7rem; }
        .scroll-btns { display: flex; gap: 5px; margin-top: 10px; }
        .scroll-btns .btn { flex: 1; font-size: 1.2rem; }
    </style>
</head>
<body>
    <div class="container">
        <h1>PC Remote</h1>
        
        <div class="section">
            <div class="section-title">Media Controls</div>
            <div class="grid media-grid">
                <button class="btn" onclick="media('prev')">&#x23EA;</button>
                <button class="btn btn-play btn-lg" onclick="media('play')">&#x23F9;</button>
                <button class="btn" onclick="media('next')">&#x23ED;</button>
                <button class="btn" onclick="media('mute')">&#x1F507;</button>
                <button class="btn" onclick="media('voldown')">&#x1F509;</button>
                <button class="btn" onclick="media('volup')">&#x1F50A;</button>
            </div>
        </div>

        <div class="section">
            <div class="section-title">Touchpad</div>
            <div class="touchpad" id="touchpad">
                <div class="touchpad-dot"></div>
                <div class="touchpad-indicator" id="indicator"></div>
            </div>
            <div class="click-btns">
                <button class="btn" onclick="mouse('left')">Left Click</button>
                <button class="btn" onclick="mouse('right')">Right Click</button>
            </div>
            <div class="scroll-btns">
                <button class="btn" onclick="scroll(-5)">&#x25B2; Up</button>
                <button class="btn" onclick="scroll(5)">&#x25BC; Down</button>
            </div>
        </div>

        <div class="section">
            <div class="section-title">Keyboard</div>
            <div class="key-grid">
                <button class="btn key-btn" onclick="key('escape')">ESC</button>
                <button class="btn key-btn" onclick="key('tab')">TAB</button>
                <button class="btn key-btn" onclick="key('enter')">&#x21B5;</button>
                <button class="btn key-btn" onclick="key('backspace')">&#x232B;</button>
                <button class="btn key-btn" onclick="key('space')">SPC</button>
            </div>
            <div class="key-grid" style="margin-top: 8px;">
                <button class="btn key-btn" onclick="key('up')">&#x25B2;</button>
                <button class="btn key-btn" onclick="key('left')">&#x25C0;</button>
                <button class="btn key-btn" onclick="key('down')">&#x25BC;</button>
                <button class="btn key-btn" onclick="key('right')">&#x25B6;</button>
                <button class="btn key-btn" onclick="key('home')">&#x2302;</button>
            </div>
            <div class="key-grid" style="margin-top: 8px;">
                <button class="btn key-btn" onclick="key('pageup')">PG&#x2191;</button>
                <button class="btn key-btn" onclick="key('pagedown')">PG&#x2193;</button>
                <button class="btn key-btn" onclick="key('end')">END</button>
            </div>
        </div>
    </div>

    <script>
        var BASE = '/api';
        var sensitivity = 3;
        var touchpad = document.getElementById('touchpad');
        var indicator = document.getElementById('indicator');

        async function media(action) {
            await fetch(BASE + '/media/' + action, { method: 'POST' });
        }

        async function mouse(action) {
            await fetch(BASE + '/mouse', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ action: action })
            });
        }

        async function scroll(delta) {
            await fetch(BASE + '/mouse', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ action: 'scroll', dy: delta * 120 })
            });
        }

        async function key(k) {
            await fetch(BASE + '/keyboard', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ key: k })
            });
        }

        var lastPos = null;
        touchpad.addEventListener('touchstart', function(e) {
            lastPos = { x: e.touches[0].clientX, y: e.touches[0].clientY };
            updateIndicator(e.touches[0].clientX, e.touches[0].clientY);
        });

        touchpad.addEventListener('touchmove', function(e) {
            e.preventDefault();
            var rect = touchpad.getBoundingClientRect();
            var x = e.touches[0].clientX;
            var y = e.touches[0].clientY;
            
            if (lastPos) {
                var dx = Math.round((x - lastPos.x) * sensitivity);
                var dy = Math.round((y - lastPos.y) * sensitivity);
                fetch(BASE + '/mouse', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ action: 'move', dx: dx, dy: dy })
                });
            }
            lastPos = { x: x, y: y };
            updateIndicator(x - rect.left, y - rect.top);
        }, { passive: false });

        touchpad.addEventListener('touchend', function() { lastPos = null; });

        touchpad.addEventListener('click', function(e) {
            var rect = touchpad.getBoundingClientRect();
            updateIndicator(e.clientX - rect.left, e.clientY - rect.top);
            mouse('left');
        });

        function updateIndicator(x, y) {
            indicator.style.left = x + 'px';
            indicator.style.top = y + 'px';
        }
    </script>
</body>
</html>
""";
}
