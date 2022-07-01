using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ApiResponse<ActivityDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ApiResponse<ActivityDto>>
        {
             private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper=mapper;
                _context = context;
            }

            public async Task<ApiResponse<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity= await _context.Activities
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x=>x.Id==request.Id);
                return ApiResponse<ActivityDto>.Success(activity);
            }
        }
    }
}