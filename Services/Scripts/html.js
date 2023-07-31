// TODO: Consider creating a service for interacting with the DOM if this functionality continues to grow

function getElementInnerHtml(elementID) {
    var element = document.getElementById(elementID);
    if (element === null) return null;
    return element.innerHTML;
}

function setId(oldid, newId) {
    var elements = document.querySelectorAll(`#${oldid}`)
    elements.forEach((el) => el.id = newId)
}