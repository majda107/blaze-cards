function translateGraphics(graphics, x, y) {
    //console.log("[js] translating element...")
    //console.log(graphics)
    graphics.style.transform = `translate(${x}px, ${y}px)`;
}

function setFocus(element) {
    element.focus();
}

function getTextWidth(element) {
    return element.getBoundingClientRect().width;
}

function getTextHeight(element) {
    return element.getBoundingClientRect().height;
}

function getBoudingRect(element) {
    return element.getBoundingClientRect();
}