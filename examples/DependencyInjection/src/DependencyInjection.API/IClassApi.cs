using DependencyInjection.API.DTOs;
using SereneApi.Interfaces;

namespace DependencyInjection.API
{
    // Note how this interface is inheriting ICrudApi<ClassDto, long>
    // When this is inherited it provides all the basic method of Crud using the supplied type and identifier.
    public interface IClassApi : ICrudApi<ClassDto, long>
    {
    }
}
