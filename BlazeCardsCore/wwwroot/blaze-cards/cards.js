function changeFlush(changes) {
    for (let change of changes) {
        translateGraphics(change.element, change.change.x, change.change.y);
    }
}

function translateGraphics(graphics, x, y) {
    let transformStr = `translate3d(${x}px, ${y}px, 0)`;
    graphics.style.webkitTransform = graphics.style.transform = transformStr;
}


function scaleGraphics(graphics, zoom) {
    graphics.style.transform = `scale(${zoom}, ${zoom})`
}

function setFocus(element) {
    element.focus();
}

function getBoudingRect(element) {
    return element.getBoundingClientRect();
}

function setWidthHeightAttribute(element, width, height) {
    element.setAttribute("width", width);
    element.setAttribute("height", height);
}




let canvasElement;
let canvasInstance;

function hookCanvasElement(element, instance) {
    canvasElement = element;
    canvasInstance = instance;

    instance.invokeMethodAsync('CanvasSizeChanged', canvasElement.getBoundingClientRect());

    window.addEventListener('resize', () => {
        instance.invokeMethodAsync('CanvasSizeChanged', canvasElement.getBoundingClientRect());
    })
}


// DEBUG
function blazeAlert(text) {
    alert(text)
}