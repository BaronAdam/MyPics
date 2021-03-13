using AutoMapper;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;

namespace MyPics.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForRegisterDto>()
                .ForMember(x => x.Password, y => y.Ignore());
        }
    }
}