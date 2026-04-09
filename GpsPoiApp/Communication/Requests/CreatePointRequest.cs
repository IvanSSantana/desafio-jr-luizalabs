using System.ComponentModel.DataAnnotations;

namespace GpsPoiApp.Communication.Requests
{
    public class CreatePointRequest
    {   
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "X coordinate is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "X coordinate must be a positive number.")]
        public int X { get; set; }
        
        [Required(ErrorMessage = "Y coordinate is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Y coordinate must be a positive number.")]
        public int Y { get; set; }
    }
}