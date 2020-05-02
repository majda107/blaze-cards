var frame;

async function translateGraphics(graphics, x, y) {

    let transformStr = `translate3d(${x}px, ${y}px, 0)`;
    graphics.style.webkitTransform = graphics.style.transform = transformStr;

    //DotNet.invokeMethodAsync('BlazeCardsCore', 'DisableEvents')

    //console.log("requesting frame...")

    //DotNet.invokeMethodAsync('BlazeCardsCore', 'DisableEvents')
    //frame = window.requestAnimationFrame(function () {

    //    let transformStr = `translate3d(${x}px, ${y}px, 0)`;
    //    graphics.style.webkitTransform = graphics.style.transform = transformStr;

    //    //DotNet.invokeMethodAsync('BlazeCardsCore', 'EnableEvents')

    //})
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

async function changeFlush(changes) {
    //console.log(changes);

    for (let change of changes) {
        await translateGraphics(change.element, change.change.x, change.change.y);
    }
}


// DEBUG
function blazeAlert(text) {
    alert(text)
}