using Application;
using Application.Extensions;
using Infrastructure;
using Infrastructure.SignalR;
using Task.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
builder.Services.AddSignalR();
builder.Services.AddApiDependencies(configuration);
builder.Services.AddApplicationDependencie();
builder.Services.AddInfrastructureDependencies(configuration);

builder.Services.AddControllers();



//builder.Services.AddControllers();
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
app.HandleException();
app.UseCors(c => c.SetIsOriginAllowed(_ => true)
.AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseRouting();

app.UseCors("AllowAngularClient");
app.UseDeveloperExceptionPage();

app.UseAuthentication(); // If you're using auth
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<LiveChatHub>("/api/livechatHub"); // <--- REQUIRED
});

//app.MapHub<LiveChatHub>("/livechatHub");

app.Run();
