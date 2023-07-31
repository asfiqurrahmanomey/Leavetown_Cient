using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class Gallery
    {
        [Parameter] public List<PhotoModel> Images { get; set; } = new List<PhotoModel>();
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private ImageViewer _imageViewer = new();

        private int _imageViewerIndex = 0;
        private const int maxImagesOnGalleryPreview = 5;

        public void OpenImageViewer(int? index = null)
        {
            _imageViewer.SetVisible(true);
            _imageViewerIndex = index ?? 0;
        }
    }
}
