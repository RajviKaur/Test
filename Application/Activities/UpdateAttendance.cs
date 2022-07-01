using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {

        public class Command : IRequest<ApiResponse<Unit>>
        {
            public Guid Id { get; set; }
        }


        public class Handler : IRequestHandler<Command, ApiResponse<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<ApiResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.Include(a => a.Attendees).ThenInclude(b => b.AppUser).SingleOrDefaultAsync(x => x.Id == request.Id);
                if (activity == null) return null;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserNAme());
                if (user == null) return null;

                var hostname = activity.Attendees.FirstOrDefault(x => x.ISHost)?.AppUser.UserName;
                var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                if (attendance != null && hostname == user.UserName)
                    activity.IsCancelled = !activity.IsCancelled;

                if (attendance != null && hostname != user.UserName)
                    activity.Attendees.Remove(attendance);

                if (attendance == null)
                {
                    attendance = new ActivityAttendee
                    {
                        AppUser = user,
                        Activity = activity,
                        ISHost = false
                    };
                    activity.Attendees.Add(attendance);
                }
                var result= await _context.SaveChangesAsync()>0;

                return result ?ApiResponse<Unit>.Success(Unit.Value):ApiResponse<Unit>.Failure("problem Updating Attendees");
            }


        }
    }
}