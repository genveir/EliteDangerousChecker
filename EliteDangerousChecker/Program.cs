using EliteDangerousChecker.Core;
using EliteDangerousChecker.InaraScraper;

namespace EliteDangerousChecker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services
                .AddHttpClient()
                .AddCore()
                .AddInaraScraper();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
