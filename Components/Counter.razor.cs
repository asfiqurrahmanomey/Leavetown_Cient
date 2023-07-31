using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components
{
    public partial class Counter
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public int Value { get; set; }
        [Parameter] public int MaxValue { get; set; }
        [Parameter] public int MinValue { get; set; }
        [Parameter] public Counter LinkCounter { get; set; } = default!;
        [Parameter] public EventCallback<int> ValueChanged { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;


        private bool _isIncrementDisabled = false;
        private bool _isDecrementDisabled = true;

        protected override void OnInitialized()
        {
            Value = MinValue;
            Validate(this);
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender)
            {
                StateHasChanged();
            }
        }

        private void Increment()
        {
            Value++;
            Update();
        }

        private void Decrement()
        {
            Value--;
            Update();
        }

        private void Update()
        {
            Validate(this);
            OnValueChanged();
            if (LinkCounter != default) Validate(LinkCounter);
        }

        public void Reset() 
        {
            Value = MinValue;
            Validate(this);
            OnValueChanged();
        }

        public void Set(int value)
        {
            Value = value;
            Validate();
        }

        private bool ValidateIncrement() => _isIncrementDisabled = LinkCounter != default ? LinkCounter.Value + Value >= MaxValue : Value >= MaxValue;
        private bool ValidateDecrement() => _isDecrementDisabled = Value <= MinValue;

        public void Validate(Counter? counter = null)
        {
            (counter ?? this).ValidateDecrement();
            (counter ?? this).ValidateIncrement();
        }

        private Task OnValueChanged()
        {
            StateHasChanged();
            return ValueChanged.InvokeAsync(Value);
        }
    }
}
