function translateGraphics(graphics, x, y) {
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