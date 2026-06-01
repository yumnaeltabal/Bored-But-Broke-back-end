using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Bored_But_Broke_back_end.Models.Queries
{
    public class GetPlacesQuery
    {
        [Required(ErrorMessage = "Location must not be empty.")]
        [StringLength(250, ErrorMessage = "Location must not be longer than 250 characters.")]
        public required string Location { get; set; }
        public string[]? Categories { get; set; }

        [Range(0, 40000, ErrorMessage = "Radius must be between 0 and 40,000.")]
        public int Radius { get; set; }

        [Length(1, 4, ErrorMessage = "Price array must contains between 1 and 4 items.")]
        public Price[] Budget { get; set; } = [ Price.Cheap, Price.Moderate, Price.Expensive, Price.VeryExpensive ];

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; } = new TimeOnly(0, 0);
        public TimeOnly EndTime { get; set; } = new TimeOnly(23, 00);

        [Range(0, 50, ErrorMessage = "Limit must be between 0 and 50.")]
        public int Limit { get; set; } = 50;
    }
}
