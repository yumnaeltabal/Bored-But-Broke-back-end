namespace Bored_But_Broke_back_end.Exceptions
{
    public class PlaceNotFoundException(string id)
        : Exception($"The place with the ID {id} does not exist")
    {
    }
}
