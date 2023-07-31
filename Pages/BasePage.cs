using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Pages
{
    public abstract class BasePage : ComponentBase, IDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private PersistentComponentState ApplicationState { get; set; } = default!;

        private PersistingComponentStateSubscription _persistingSubscription;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {                
                await JSRuntime.InvokeVoidAsync("restoreScrollFromPrerender");
            }
        }

        protected async Task<TModel> GetPageModel<TModel>(Func<Task<TModel>> loadModel)
        {
            TModel model = default!;
            const string key = "page-data";

            Task PersistPageModel()
            {
                ApplicationState.PersistAsJson(key, model);
                return Task.CompletedTask;
            }

            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistPageModel);

            if (ApplicationState.TryTakeFromJson<TModel>(key, out var m))
            {
                model = m!;
            }
            else
            {
                model = await loadModel();
            }

            return model;
        }

        public virtual void Dispose()
        {
            _persistingSubscription.Dispose();
        }
    }
}
