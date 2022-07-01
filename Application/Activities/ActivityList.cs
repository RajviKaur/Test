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
    public class ActivityList
    {

        public class Query : IRequest<ApiResponse<List<ActivityDto>>>
        {

        }

        public class Handler : IRequestHandler<Query, ApiResponse<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<ApiResponse<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // 1Way-----------
                // var activities= await _context.Activities
                // .Include(x => x.Attendees)
                // .ThenInclude(x => x.AppUser)
                // .ToListAsync();

                // var activitiesToReturn= _mapper.Map<List<ActivityDto>>(activities);
                // return  ApiResponse<List<ActivityDto>>.Success(activitiesToReturn);


                // The above query is fering everything from the database , Including the member that arde not required 
                // To make query more effecient you can  use 'Projection'  which automap and fetch only required field.

                var activities = await _context.Activities
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

                //var activitiesToReturn= _mapper.Map<List<ActivityDto>>(activities); Above query is doing the mapping
                return ApiResponse<List<ActivityDto>>.Success(activities);

            }
        }
    }
}