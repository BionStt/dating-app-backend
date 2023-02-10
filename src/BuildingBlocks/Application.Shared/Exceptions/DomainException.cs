namespace Application.Shared.Exceptions
{
    public class DomainException : ApiException
    {
        public DomainException(string message) : base("Domain Error", message) { }
    }
}
