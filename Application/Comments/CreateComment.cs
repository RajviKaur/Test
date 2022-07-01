using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class CreateComment
    {


        public class Command : IRequest<ApiResponse<CommentDto>>
        {
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }


        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                //Set the rule from Activity validator to validate here
                RuleFor(x => x.Body).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ApiResponse<CommentDto>>
        {
            private readonly DataContext _context;
            public readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<ApiResponse<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);
                if (activity == null) return null;
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUserNAme());

                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };

                activity.Comments.Add(comment);
                
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return ApiResponse<CommentDto>.Failure("Failed to add Comment");
                return ApiResponse<CommentDto>.Success(_mapper.Map<CommentDto>(comment));
            }
        }
    }
}