registerOnResizeHandler = function (dotNetObjRef) {
    let previousWidth = 0;
    function OnResizeHandler() {
        // width of the window in em units
       
        let width = window.innerWidth / parseFloat(
            getComputedStyle(
                document.querySelector('body')
            )['font-size']
        );

        if (width < 48 && (previousWidth >= 48 || previousWidth == 0)) {
            if (dotNetObjRef) {        
                dotNetObjRef.invokeMethodAsync("ToggleMap", false);
            }
        }

        if (width >= 48 && (previousWidth < 48 || previousWidth == 0)) {
            if (dotNetObjRef) {
                dotNetObjRef.invokeMethodAsync("ToggleMap", true);
            }
        }

        previousWidth = width;
        return;
    }
    OnResizeHandler();
    window.addEventListener('resize', OnResizeHandler);
};








