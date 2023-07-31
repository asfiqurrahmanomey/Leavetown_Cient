function setLocalStorage(key, value) {
    if (navigator.cookieEnabled) {
        localStorage.setItem(key, value);
    }
}

function getLocalStorage(key) {
    if (navigator.cookieEnabled) {
        return localStorage.getItem(key);
    }
    else {
        return null;
    }
}