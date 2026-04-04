var BASE = '/api';
var sensitivity = 3;

var touchpad;
var indicator;
var toggles = { drag: false, shift: false, ctrl: false, alt: false, win: false, numlock: false, delete: false };
var pressStartTime = {};
var isDragActive = false;
var lastPos = null;
var touchStartTime = null;
var lastClickTime = null;

function init() {
    touchpad = document.getElementById('touchpad');
    indicator = document.getElementById('indicator');
    setupTouchpad();
    fetchState();
}

async function fetchState() {
    try {
        var response = await fetch(BASE + '/state');
        if (response.ok) {
            var state = await response.json();
            toggles.drag = state.mouse.drag;
            toggles.shift = state.keyboard.shift;
            toggles.ctrl = state.keyboard.ctrl;
            toggles.alt = state.keyboard.alt;
            toggles.win = state.keyboard.win;
            toggles.numlock = state.keyboard.numlock;
            toggles.delete = state.keyboard.delete;
            isDragActive = toggles.drag;
            updateToggleButton('drag');
            updateToggleButton('shift');
            updateToggleButton('ctrl');
            updateToggleButton('alt');
            updateToggleButton('win');
            updateToggleButton('numlock');
            updateToggleButton('delete');
        }
    } catch (e) {
        console.log('Could not fetch state:', e);
    }
}

function setupTouchpad() {
    touchpad.addEventListener('pointerdown', handlePointerDown);
    touchpad.addEventListener('pointermove', handlePointerMove);
    touchpad.addEventListener('pointerup', handlePointerUp);
    touchpad.addEventListener('pointercancel', handlePointerUp);
}

async function media(action) {
    await fetch(BASE + '/media/' + action, { method: 'POST', body: {} });
}

async function setVolume(level) {
    await fetch(BASE + '/media/volume', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(level)
    });
}

async function mouse(action) {
    await fetch(BASE + '/mouse', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action: action })
    });
}

async function doScroll(delta) {
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

async function sendText() {
    var input = document.getElementById('textInput');
    var text = input.value;
    if (text) {
        await fetch(BASE + '/keyboard/text', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ text: text })
        });
        input.value = '';
    }
}

function startPress(target) {
    pressStartTime[target] = Date.now();
}

async function endPress(target) {
    var duration = Date.now() - (pressStartTime[target] || 0);
    delete pressStartTime[target];

    if (target === 'numlock') {
        await fetch(BASE + '/keyboard', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ key: target, holdDuration: 1000 })
        });
        toggles[target] = !toggles[target];
        updateToggleButton(target);
        return;
    }

    if (duration >= 1000) {
        await fetch(BASE + '/keyboard', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ key: target, holdDuration: 1000 })
        });
        toggles[target] = !toggles[target];
        updateToggleButton(target);
    } else {
        if (toggles[target]) {
            await fetch(BASE + '/keyboard', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ key: target, holdDuration: 0 })
            });
            toggles[target] = false;
            updateToggleButton(target);
        } else {
            await fetch(BASE + '/keyboard', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ key: target })
            });
        }
    }
}

async function toggleDrag() {
    if (toggles.drag) {
        await fetch(BASE + '/mouse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ action: 'left', holdDuration: 0 })
        });
        toggles.drag = false;
    } else {
        await fetch(BASE + '/mouse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ action: 'left', holdDuration: 1000 })
        });
        toggles.drag = true;
    }
    isDragActive = toggles.drag;
    updateToggleButton('drag');
}

function updateToggleButton(target) {
    var btnId = { drag: 'dragBtn', shift: 'shiftBtn', ctrl: 'ctrlBtn', alt: 'altBtn', win: 'winBtn', numlock: 'numlockBtn', delete: 'deleteBtn' }[target];
    var btn = document.getElementById(btnId);
    if (btn) {
        if (toggles[target]) {
            btn.classList.add('btn-toggle');
            btn.title = "Quick tap to untoggle";
        } else {
            btn.classList.remove('btn-toggle');
            btn.title = "Long press to toggle";
        }
    }
}

function handlePointerDown(e) {
    lastPos = { x: e.clientX, y: e.clientY };
    touchStartTime = Date.now();
    updateIndicator(e.clientX, e.clientY);
    if (isDragActive) {
        fetch(BASE + '/mouse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ action: 'leftdown' })
        });
        indicator.style.background = 'rgba(233, 69, 96, 0.5)';
    }
}

function handlePointerMove(e) {
    e.preventDefault();
    var rect = touchpad.getBoundingClientRect();
    var x = e.clientX;
    var y = e.clientY;
    
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
}

function handlePointerUp(e) {
    if (isDragActive) {
        fetch(BASE + '/mouse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ action: 'leftup' })
        });
        indicator.style.background = 'rgba(0,212,255,0.3)';
    } else if (touchStartTime && Date.now() - touchStartTime < 200) {
        handleClick();
    }
    lastPos = null;
    touchStartTime = null;
}

function handleClick() {
    var now = Date.now();
    if (lastClickTime && (now - lastClickTime) < 300) {
        mouse('doubleclick');
        lastClickTime = null;
    } else {
        mouse('left');
        lastClickTime = now;
    }
}

function updateIndicator(x, y) {
    indicator.style.left = x + 'px';
    indicator.style.top = y + 'px';
}

document.addEventListener('DOMContentLoaded', init);
