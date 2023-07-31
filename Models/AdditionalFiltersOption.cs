namespace Leavetown.Client.Models
{
    public class AdditionalFiltersOption<T> where T : class
    {
        public T Value { get; set; } = default!;
        public bool isActive { get; set; } = false;

        public AdditionalFiltersOption(T value) { Value = value; }
    }
}
