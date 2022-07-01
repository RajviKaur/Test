using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class DeleteActivity
    {
         public class Command : IRequest<ApiResponse<Unit>>
        {
            public Guid Id { get; set; }
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
                var activity = await _context.Activities.FindAsync(request.Id);
               // if(activity==null) return null; 

               // Exception will be thrown here if the the activity is not found and will be catch by Application Middleware
               _context.Remove(activity);
                var result=await _context.SaveChangesAsync()>0;
                if(!result) ApiResponse<Unit>.Failure("Failed to delete the activity");
                return ApiResponse<Unit>.Success(Unit.Value);
            }
        }

    }
}