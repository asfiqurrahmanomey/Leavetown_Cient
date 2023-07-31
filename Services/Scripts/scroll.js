function scrollToFirstError() {
    setTimeout(() => {

        var invalidInput = document.getElementsByClassName('invalid')[0];

        if (invalidInput)
            invalidInput.parentNode.scrollIntoView();

    }, 500)

}

// ***************** Handle fixing scroll position between prerender and client render ****************
var prerenderScrollPosition = null;
document.body.addEventListener('scroll', capturePrerenderScrollPosition);

function capturePrerenderScrollPosition() {
    prerenderScrollPosition = document.body.scrollTop;
}

function restoreScrollFromPrerender() {
    document.body.removeEventListener('scroll', capturePrerenderScrollPosition);

    if (prerenderScrollPosition) {        
        document.body.scrollTop = prerenderScrollPosition;
        prerenderScrollPosition = null;
    }    
}
// *****************************************************************************************************
