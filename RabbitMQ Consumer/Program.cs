using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Services;

[CompilerGenerated]
internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddControllers();
		builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		WebApplication app = builder.Build();
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		try
		{
			IRabbitMQConsumer rabbitMqConsumer = app.Services.GetRequiredService<IRabbitMQConsumer>();
			rabbitMqConsumer.StartConsuming();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		app.UseHttpsRedirection();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}
