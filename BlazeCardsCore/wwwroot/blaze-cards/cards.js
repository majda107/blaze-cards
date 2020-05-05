function changeFlushPacket(packet) {
    let text = BINDING.conv_string(packet);

    let changes = text.split('|');

    for (let change of changes) {
        let changeData = change.split(';');
        let el = document.querySelector(`#${changeData[0]}`);

        if (el == undefined) continue;
        //console.log(changeData[1]);
        translateGraphics(el, changeData[1], changeData[2])
    }

    //console.log(text);
}

function translateGraphics(graphics, x, y) {
    let transformStr = `translate3d(${x}px, ${y}px, 0)`;
    graphics.style.webkitTransform = graphics.style.transform = transformStr;
}


function scaleGraphics(packet) {
    //console.log(BINDING.conv_string(packet));
    let split = BINDING.conv_string(packet).split(';');
    let graphics = document.querySelector(`#${split[0]}`);

    graphics.style.webkitTransformOrigin = `${split[2]}px ${split[3]}px`;
    graphics.style.transformOrigin = `${split[2]}px ${split[3]}px`;

    graphics.style.transform = `scale(${split[1]}, ${split[1]})`;
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

function hookCanvasElement(elementID, instance) {
    canvasElement = document.querySelector(`#${elementID}`);
    canvasInstance = instance;

    instance.invokeMethodAsync('CanvasSizeChanged', canvasElement.getBoundingClientRect());

    window.addEventListener('resize', () => {
        instance.invokeMethodAsync('CanvasSizeChanged', canvasElement.getBoundingClientRect());
    })
}


// DEBUG
function blazeAlert(text) {
    //text = String.fromCharCode.apply(null, byteText);
    id = BINDING.conv_string(text)

    alert(id);
    console.log(id);
}