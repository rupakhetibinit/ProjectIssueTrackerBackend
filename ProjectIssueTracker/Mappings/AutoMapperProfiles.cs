using AutoMapper;
using Microsoft.Build.Framework;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ProjectCreateDto, Project>().ReverseMap();
            CreateMap<ProjectUpdateDto, Project>().ReverseMap();
            CreateMap<UserRegistrationDto, User>().ReverseMap();
            CreateMap<UserLoginDto, User>().ReverseMap();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.CreatedIssues, opt => opt.MapFrom(src => src.CreatedIssues))
                .ForMember(dest => dest.CollaboratedProjects, opt => opt.MapFrom(src => src.CollaborativeProjects))
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.OwnedProjects))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ReverseMap();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Issues, opt => opt.MapFrom(src => src.Issues))
                .ForMember(dest => dest.Collaborators, opt => opt.MapFrom(src => src.Collaborators))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ReverseMap();

           CreateMap<ProjectCollaborator, CollaboratorDto>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
               .ReverseMap();

            //CreateMap<CollaboratorDto, ProjectCollaborator>()
            //    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            //    .ForMember(dest => dest.User.Email, opt => opt.MapFrom(src => src.Email))
            //    .ReverseMap();


            CreateMap<CommentDto, Comment>().ReverseMap();

            CreateMap<Issue, IssueDto>()
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.Name))
                .ForMember(dest => dest.CreatorEmail, opt => opt.MapFrom(src => src.Creator.Email))
                .ReverseMap();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.CommenterName, opt => opt.MapFrom(src => src.Commenter.Name))
                .ReverseMap();

        }

    }
}
