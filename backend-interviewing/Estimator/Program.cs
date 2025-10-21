using Microsoft.EntityFrameworkCore;
using Estimator;

public class Program
{
   static void Main(string[] args)
   {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<EstimatesContext>(options =>
            options.UseInMemoryDatabase("EstimatesDb"));

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EstimatesContext>();
            context.Database.EnsureCreated();
        }

        app.Run();
   }
}