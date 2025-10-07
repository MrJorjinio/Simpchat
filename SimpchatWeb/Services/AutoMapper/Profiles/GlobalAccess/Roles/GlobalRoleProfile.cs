using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;

namespace SimpchatWeb.Services.AutoMapper.Profiles.GlobalAccess.Roles
{
    public class GlobalRoleProfile : Profile
    {
        public GlobalRoleProfile()
        {
            CreateMap<UserGlobalRolesPostDto, GlobalRole>();
            CreateMap<GlobalRole, UserGlobalRolesPostDto>();
        }
    }
}
