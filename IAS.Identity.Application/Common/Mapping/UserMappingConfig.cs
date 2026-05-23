using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using Mapster;

namespace IAS.Identity.Application.Common.Mapping
{
    internal class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserDto>();
        }
    }
}