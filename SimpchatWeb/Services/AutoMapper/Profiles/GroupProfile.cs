using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Creates;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Gets;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupPostDto, Group>();
            CreateMap<GroupResponseDto, Group>();
            CreateMap<GroupGetDto, Group>();
            CreateMap<Group, GroupGetDto>();
            CreateMap<Group, GroupPostDto>();
            CreateMap<Group, GroupResponseDto>();
        }
    }
}
