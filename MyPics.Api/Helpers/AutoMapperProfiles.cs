using AutoMapper;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;

namespace MyPics.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserForRegisterDto, User>();
            CreateMap<User, UserForSearchDto>();
            CreateMap<User, UserForFollowDto>();

            CreateMap<PostForAddDto, Post>().ForSourceMember(x => x.Files, opt => opt.DoNotValidate());
        }
    }
}