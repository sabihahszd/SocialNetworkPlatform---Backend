using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.Services;
using Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IChatThreadRepository, ChatThreadRepository>();
builder.Services.AddScoped<IChatThreadService, ChatThreadService>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
