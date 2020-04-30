﻿function translateGraphics(graphics, x, y) {
    //console.log("[js] translating element...")
    //console.log(graphics)
    graphics.style.transform = `translate(${x}px, ${y}px)`;
}

function scaleGraphics(graphics, zoom) {
    graphics.style.transform = `scale(${zoom}, ${zoom})`
}

function setFocus(element) {
    element.focus();
}

//function getTextWidth(element) {
//    return element.getBoundingClientRect().width;
//}

//function getTextHeight(element) {
//    return element.getBoundingClientRect().height;
//}

function getBoudingRect(element) {
    return element.getBoundingClientRect();
}

function setWidthHeightAttribute(element, width, height) {
    element.setAttribute("width", width);
    element.setAttribute("height", height);
}

function changeFlush(changes) {
    for (let change of changes) {
        translateGraphics(change.element, change.change.x, change.change.y);
    }
}