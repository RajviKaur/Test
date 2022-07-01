using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class EditActivity
    {
        public class Command : IRequest<ApiResponse<Unit>>
        {
            public Activity Activity { get; set; }
        }


        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                //Set the rule from Activity validator to validate here
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command,ApiResponse<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ApiResponse<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                //  activity.Title=request.Activity.Title?? activity.Title;
                _mapper.Map(request.Activity, activity);//auto Mapping  instead of doing it mannually
               var result= await _context.SaveChangesAsync()>0;
              if(!result) return ApiResponse<Unit>.Failure("Failed to update activity");
              return ApiResponse<Unit>.Success(Unit.Value);
            }


        }
    }
}