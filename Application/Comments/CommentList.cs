using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Comments;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class CommentList
    {

        public class Query : IRequest<ApiResponse<List<CommentDto>>>
        {
public  Guid ActivityId{ get; set; }
        }

        public class Handler : IRequestHandler<Query, ApiResponse<List<CommentDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<ApiResponse<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                var comments = await _context.Comments.Where(x=>x.Activity.Id==request.ActivityId).OrderBy(x=>x.CreatedAt)
                .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            
                return ApiResponse<List<CommentDto>>.Success(comments);

            }
        }
    }
}