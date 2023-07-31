using Leavetown.Client.Response;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class Custom404
    {
        [Parameter] public string? Culture { get; set; }
        [Inject] private IResponse? _response { get; set; }

        protected override void OnInitialized()
        {
            _response!.SetNotFound();
        }
    }
}
