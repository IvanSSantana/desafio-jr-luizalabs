namespace GpsPoiApp.Communication.Responses;

public class CreatedPointResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int X { get; set; }
    public int Y { get; set; }
}
