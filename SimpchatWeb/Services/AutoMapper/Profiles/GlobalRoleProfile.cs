using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GlobalRolesDtos;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class GlobalRoleProfile : Profile
    {
        public GlobalRoleProfile()
        {
            CreateMap<GlobalRoleDto, GlobalRole>();
            CreateMap<GlobalRole, GlobalRoleDto>();
        }
    }
}
