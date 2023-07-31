using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class Carousel
    {
		[Parameter] public List<PhotoModel> Images { get; set; } = new List<PhotoModel>();
		[Parameter] public string Id { get; set; } = "";
		[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;

		private string _carouselName = "";

		protected override void OnInitialized()
		{
			_carouselName = $"imageCarousel-{Id}";
		}
	}
}
