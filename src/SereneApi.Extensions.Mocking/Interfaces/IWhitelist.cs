using SereneApi.Extensions.Mocking.Enums;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IWhitelist
    {
        Validity Validate(object value);
    }
}
