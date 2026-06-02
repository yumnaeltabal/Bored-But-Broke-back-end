namespace Bored_But_Broke_back_end.Exceptions
{
    public class EmailAlreadyInUseException(string email) 
        : Exception($"Email '{email}' is already in use")
    {
    }
}
