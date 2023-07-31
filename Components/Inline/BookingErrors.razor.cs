using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Leavetown.Client.Components.Inline
{
    public partial class BookingErrors
    {
        [Parameter, EditorRequired, DisallowNull] public List<string> ErrorMessageCodes { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private List<string> _errorMessages = new();

        protected override void OnInitialized()
        {
            foreach (var errorCode in ErrorMessageCodes)
            {
                var errorMessage = ResourcesShared[errorCode];
                _errorMessages.Add(errorMessage ?? ResourcesShared.BookingErrorGeneric);
            }
        }
    }
}
