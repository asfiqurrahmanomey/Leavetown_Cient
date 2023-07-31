namespace Leavetown.Client.Models
{
    public class FormStepItem
    {
        public int Index { get; set; }
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";

        public FormStepItem(int index, string title, string subtitle)
        {
            Index = index;
            Title = title;
            Subtitle = subtitle;
        }
    }
}
