using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class CreateActivity
    {
        public class Command : IRequest<ApiResponse<Unit>>
        {
            public Activity Activity { get; set; }
        }


        public class CommandValidator :AbstractValidator<Command>
        {
            public CommandValidator()
            {
                //Set the rule from Activity validator to validate here
                RuleFor(x=>x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command,ApiResponse<Unit>>
        {
            private readonly DataContext _context;
        public  readonly IUserAccessor _userAccessor ;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
            _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<ApiResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user= await _context.Users.FirstOrDefaultAsync(x=>x.UserName== _userAccessor.GetUserNAme());
                var attendee= new ActivityAttendee{
                    AppUser= user,
                    Activity= request.Activity,
                    ISHost=true
                };

                request.Activity.Attendees.Add(attendee);
                _context.Activities.Add(request.Activity);
               var result = await _context.SaveChangesAsync()>0;
               if(!result) return ApiResponse<Unit>.Failure("Failed to create activity");
                return ApiResponse<Unit>.Success(Unit.Value);
            }
        }
    }
}