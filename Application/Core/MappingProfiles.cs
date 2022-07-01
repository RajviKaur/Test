using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Comments;
using Application.Profiles;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        // create Mapping  : which will help us to avoid passing value while updating/passing value to DB
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            // need to add mapping for member that cannot automatically map
            CreateMap<Activity, ActivityDto>()
            .ForMember(d => d.HostUserName, o => o.MapFrom(s => s.Attendees
            .FirstOrDefault(x => x.ISHost).AppUser.UserName));

            CreateMap<ActivityAttendee, UserProfile>()
            .ForMember(x=>x.DisplayName, o=>o.MapFrom(s=>s.AppUser.DisplayName))
            .ForMember(x=>x.UserName, o=>o.MapFrom(s=>s.AppUser.UserName))
            .ForMember(x=>x.Bio, o=>o.MapFrom(s=>s.AppUser.Bio));


            CreateMap<Comment, CommentDto>()
            .ForMember(x=>x.DisplayName, o=>o.MapFrom(s=>s.Author.DisplayName));
        }
    }
}