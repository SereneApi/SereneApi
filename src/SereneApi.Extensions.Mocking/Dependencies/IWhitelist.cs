namespace SereneApi.Extensions.Mocking.Dependencies
{
    public interface IWhitelist
    {
        Validity Validate(object value);
    }
}
