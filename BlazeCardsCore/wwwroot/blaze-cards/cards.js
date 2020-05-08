﻿function changeFlushPacket(packet) {
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

function setFocus(elementID) {
    element = document.querySelector(`#${elementID}`);
    if (element == undefined) return;

    element.focus();
}

function calculateTextRect(text) {
    let svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    svg.classList.add("blaze-calculate-hidden");

    let textsvg = document.createElementNS("http://www.w3.org/2000/svg", "text");
    textsvg.classList.add("blaze-text");
    textsvg.setAttribute("id", "text-calculation-id");
    textsvg.innerHTML = text;
    svg.appendChild(textsvg);

    document.body.appendChild(svg);
    let rect = getBoudingRect("text-calculation-id");
    document.body.removeChild(svg);

    return rect;
}



// TEXT
let textInstance;

function hookEditing(instance, elementID) {
    textInstance = instance;

    let inputEl = document.createElement("input");
    inputEl.classList.add("blaze-input-hidden");
    document.body.appendChild(inputEl);

    //inputEl.addEventListener('keypress', (e) => {
    //    instance.invokeMethodAsync("KeyDown", e.key);
    //})

    inputEl.addEventListener('keydown', (e) => {
        instance.invokeMethodAsync("KeyDown", e.key);
    })

    inputEl.focus();
}



function getBoudingRect(elementID) {
    element = document.querySelector(`#${elementID}`);
    if (element == undefined) return;
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
}


// DEBUG
function blazeAlert(text) {
    //text = String.fromCharCode.apply(null, byteText);
    id = BINDING.conv_string(text)

    alert(id);
    console.log(id);
}




function Init() {
    window.addEventListener('resize', () => {
        canvasInstance.invokeMethodAsync('CanvasSizeChanged', canvasElement.getBoundingClientRect());
    })
}

Init();