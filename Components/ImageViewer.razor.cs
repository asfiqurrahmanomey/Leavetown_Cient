using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components
{
    public partial class ImageViewer
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter] public List<PhotoModel> Images { get; set; } = new List<PhotoModel>();
        [Parameter] public int Index { get; set; } = 0;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private bool _visible = false;
        private ElementReference _imageViewer;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_visible && Images.Any())
            {
                await _imageViewer.FocusAsync();
            }
        }

        public void SetVisible(bool value)
        {
            _visible = value;
            StateHasChanged();
        }

        public void Close()
        {
            _visible = false;
        }

        public void PreviousImage()
        {
            Index = Index - 1 >= 0 ? Index - 1 : Index;
            StateHasChanged();
        }

        public void NextImage()
        {
            Index = Index + 1 < Images.Count ? Index + 1 : Index;
            StateHasChanged();
        }

        public void ToggleFullscreen()
        {
            InvokeAsync(async () => await JSRuntime.InvokeVoidAsync("toggleFullScreen", _imageViewer));
        }

        private void HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Escape":
                case "Esc":
                    _visible = false;
                    break;
                case "ArrowLeft":
                    PreviousImage();
                    break;
                case "ArrowRight":
                    NextImage();
                    break;
            }
        }
    }
}
