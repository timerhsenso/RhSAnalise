namespace RhSensoERP.API;

/// <summary>
/// Classe principal da aplicação.
/// </summary>
public static class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// </summary>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync().ConfigureAwait(false);
    }
}
