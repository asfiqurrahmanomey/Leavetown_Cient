using Leavetown.Client.Models;
using Leavetown.Client.Utilities.Extensions;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class FormSteps
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        [Parameter] public List<FormStepItem> Items { get; set; } = new List<FormStepItem>();
        [Parameter] public int ActiveIndex { get; set; } = 0;
    }
}
