using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<GlobalPermission, GlobalPermissionDto>();
            CreateMap<GlobalPermissionDto, GlobalPermission>();
        }
    }
}
