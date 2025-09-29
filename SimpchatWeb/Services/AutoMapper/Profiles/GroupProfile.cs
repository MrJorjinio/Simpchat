using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupCreateDto, Group>();
            CreateMap<GroupResponseDto, Group>();
            CreateMap<Group, GroupCreateDto>();
            CreateMap<Group, GroupResponseDto>();
        }
    }
}
