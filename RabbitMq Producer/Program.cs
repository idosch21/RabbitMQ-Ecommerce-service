using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProducerService_Exercise1.Connection;

[CompilerGenerated]
internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddControllers();
		builder.Services.AddSingleton<IMessageProducer, RabbitMQProducer>();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		WebApplication app = builder.Build();
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		app.UseHttpsRedirection();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}
