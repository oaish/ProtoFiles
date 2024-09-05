using MongoDB.Driver;
using ProtoFiles.API.Repositories;
using ProtoFiles.API.Repositories.Contracts;
using ProtoFiles.API.Services;
using ProtoFiles.API.Services.Contracts;

namespace ProtoFiles.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = new MongoClient(builder.Configuration.GetValue<string>("MongoUri"));
            var db = client.GetDatabase(builder.Configuration.GetValue<string>("Database"));
            return db;
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDriveRepository, DriveRepository>();
        builder.Services.AddScoped<IDriveService, DriveService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}