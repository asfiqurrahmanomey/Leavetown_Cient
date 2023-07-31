using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Leavetown.Client.Components
{
    public partial class DrillDownMenu<TItem> where TItem : class
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public List<TItem> Items { get; set; } = new List<TItem>();
        [Parameter] public string Item1 { get; set; } = string.Empty;
        [Parameter] public string Item2 { get; set; } = string.Empty;
        [Parameter] public string Item3 { get; set; } = string.Empty;
        [Parameter] public string Value { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public int SelectedLevel { get; set; } = 1;

        public TItem SelectedItem = default!;

        private Dictionary<string, bool> _itemsOnLevel = new();
        private string? _expandedItem = string.Empty;
        private int? _index;
        protected override void OnInitialized()
        {
            Items.Select(x => GetStringValueFromItem(x, GetItem()))
                .Distinct()
                .ToList()
                .ForEach(x => _itemsOnLevel.Add(x, false));
        }

        public void Reset()
        {
            Value = string.Empty;
            SetLevel(1);
            ClearSelection();
            ValueChanged.InvokeAsync();
        }

        public void SetSelection(string arg, TItem? item = null)
        {
            if (!Items.Any() || !_itemsOnLevel.Any() || string.IsNullOrEmpty(arg)) return;
            ClearSelection();

            if(_itemsOnLevel.ContainsKey(arg)) _itemsOnLevel[arg] = true;
            Value = arg;
            if(item != null) SelectedItem = item;
            ValueChanged.InvokeAsync();

            StateHasChanged();
        }

        private string GetStringValueFromItem(TItem item, string property)
        {
            if (item == null) return string.Empty;

            PropertyInfo pi = item.GetType().GetProperty(property)!;
            object value = pi.GetValue(item, null)!;

            return value?.ToString() ?? string.Empty;
        }

        private string GetItem(int? level = null)
        {
            return (level ?? SelectedLevel) switch
            {
                1 => Item1 ?? string.Empty,
                2 => Item2 ?? string.Empty,
                3 => Item3 ?? string.Empty,
                _ => Item1 ?? string.Empty, // default return Item1 if it is not null
            };
        }

        public void SetLevel(int level, int? index = null, string? item = null)
        {
            SelectedLevel = level;
            _index = SelectedLevel == 1 ? null : index;
            _itemsOnLevel.Clear();
            _expandedItem = item;
            ClearSelection();

            var items = Items;
            if (_index != null)
            {
                items = items.Where(x =>
                    GetStringValueFromItem(Items[_index.Value], GetItem(SelectedLevel - 1)) == GetStringValueFromItem(x, GetItem(SelectedLevel - 1))
                ).ToList();
            }

            items.Select(x => GetStringValueFromItem(x, GetItem()))
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList()
                .ForEach(x => _itemsOnLevel.Add(x, false));

            StateHasChanged();
        }

        private void ClearSelection()
        {
            foreach (var item in _itemsOnLevel)
            {
                _itemsOnLevel[item.Key] = false;
            }
        }
    }
}
