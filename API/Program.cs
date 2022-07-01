using Persistence;
using API.Extensions;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using Application.Activities;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
    opt=>{
        //Every single endpoint in our app need authorization
        var policy= new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
    })
//Add fluent validation  to all the classed in the assembly having createActivity
.AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateActivity>());

//Adding Services using extension method
builder.Services.AddApplicationServices(builder.Configuration);
// Adding Identity services
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();

//Add Exception middleWare : Positioning is important
app.UseMiddleware<ExceptionMiddleware>();
app.MapHub<ChatHub>("/chat");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.UseAuthentication();//Ordering is imp
app.UseAuthorization();


app.MapControllers();

using var scope = app.Services.CreateScope();
try
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var userManager= scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync(); //Apply all the pending migrations and create db if not exists.
   
    await Seed.SeedData(context, userManager); //Insert data if not exists.
}
catch (Exception e)
{
    //add logger
}

app.Run();


