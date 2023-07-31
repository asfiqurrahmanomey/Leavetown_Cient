document.addEventListener('click', function (e) {
    var popoverContainers = document.getElementsByClassName("lt-popover-container");
    for (let i = 0; i < popoverContainers.length; i++) {
        if (popoverContainers[i] !== null && !popoverContainers[i].contains(e.target)) {
            closePopover(popoverContainers[i]);
        }
    }
});

function togglePopover(id) {
    var popoverContainer = document.getElementById(id);

    if (popoverContainer.querySelector('.lt-popover').style.display === "none") {
        openPopover(popoverContainer);
    }
    else {
        closePopover(popoverContainer);
    }
}

function openPopover(popoverContainer) {
    popoverContainer.querySelector('.dd-caret').classList.add('dd-caret-up');

    var popover = popoverContainer.querySelector('.lt-popover');
    popover.style.display = "block";
    popover.classList.add('active');

    addAutoPositionClasses(popoverContainer, popover);
}

function closePopover(popoverContainer) {
    popoverContainer.querySelector('.dd-caret').classList.remove('dd-caret-up');

    var popover = popoverContainer.querySelector('.lt-popover');
    popover.style.display = "none";
    popover.classList.remove('active');
}


function addAutoPositionClasses(popoverContainer, popover) {    
    // 1. Remove auto position classes
    popover.classList.remove('position-on-top');
    popover.classList.remove('position-on-bottom');
    popover.classList.remove('position-to-left');
    popover.classList.remove('position-to-right');

    // 2. Add auto position classes
    var popoverContainerRect = popoverContainer.getBoundingClientRect();

    if (popoverContainerRect.y * 2 > window.innerHeight) {
        popover.classList.add('position-on-top');
    } else {
        popover.classList.add('position-on-bottom');
    }

    var popoverRect = popover.getBoundingClientRect();

    if (popoverRect.left < 0) {
        popover.classList.add('position-to-right');
    }

    if (popoverRect.right > (window.innerWidth - 10)) { // - 10 to cover rough size of scrollbar
        popover.classList.add('position-to-left');
    }
}