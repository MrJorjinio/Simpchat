using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles.Groups
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupPostDto, Group>();
            CreateMap<GroupResponseDto, Group>();
            CreateMap<Group, GroupPostDto>();
            CreateMap<Group, GroupResponseDto>();
        }
    }
}
