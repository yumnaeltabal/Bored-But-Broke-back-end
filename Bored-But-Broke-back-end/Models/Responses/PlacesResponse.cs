namespace Bored_But_Broke_back_end.Models.Responses
{
    public class PlacesResponse
    {
        public List<PlaceResponse> Places { get; set; } = [];
        public bool IsIndoor { get; set; }
    }
}
