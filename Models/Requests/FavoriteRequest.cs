namespace GloboClima.Server.Models.Requests;

public class FavoriteRequest
{
    public string UserId { get; set; } = "default-user";
    public string FavoriteId { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}