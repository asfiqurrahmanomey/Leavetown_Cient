namespace Leavetown.Client.Models.Projections;

public class BookingResponseModel
{
    public int EntityId { get; set; }
    public bool Successful { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string[] Warnings { get; set; } = Array.Empty<string>();
}