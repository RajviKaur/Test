using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{

    // customize Middleware to handle exceptions
    public class ExceptionMiddleware
    {
        public readonly RequestDelegate _next ;
        public  readonly ILogger<ExceptionMiddleware> _logger ;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _next = next; 
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try{
             await _next(context); // If no exception : pass to next middleware
            }
            catch(Exception ex)
            {
               // if Exception Found: Pass 
               _logger.LogError(ex, ex.Message);
               context.Response.ContentType= "application/json";
               context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;

               var response= _env.IsDevelopment()
               ?new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
               :new AppException(context.Response.StatusCode, ex.Message, "Server Error");

               //var options= new JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase);
               //var json= JsonSerializer.Serialize(response, options);
               var json= JsonSerializer.Serialize(response);

               await context.Response.WriteAsync(json);

            }
        }
    }
}