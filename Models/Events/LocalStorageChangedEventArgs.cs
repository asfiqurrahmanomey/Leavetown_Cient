namespace Leavetown.Client.Models.Events
{
    public class LocalStorageChangedEventArgs
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = default!;

        public LocalStorageChangedEventArgs(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
