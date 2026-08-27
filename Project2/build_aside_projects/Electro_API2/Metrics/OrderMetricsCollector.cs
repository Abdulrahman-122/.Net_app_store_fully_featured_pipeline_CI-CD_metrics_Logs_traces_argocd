using ElectroAPI.Data;
using  ElectroAPI.Metrics;
using Microsoft.EntityFrameworkCore;
namespace ElectroAPI.Services;

public class OrderMetricsCollector: BackgroundService
{
	private readonly IServiceScopeFactory  _scopeFactory;
	private readonly ILogger<OrderMetricsCollector> _logger;
	public  OrderMetricsCollector(
			IServiceScopeFactory scopeFactory,
			ILogger<OrderMetricsCollector>  logger)
	{
		_scopeFactory=scopeFactory;
		_logger=logger;
	}
	protected override async Task ExecuteAsync(
			CancellationToken  stoppingToken)
	{
		while(!stoppingToken.IsCancellationRequested)
		{
			try{
				await using var scope=_scopeFactory.CreateAsyncScope();
				var db=scope.ServiceProvider.GetRequiredService<ElectroDbContext>();
				var activeOrders=await  db.Orders.CountAsync(o=>o.Status !="Cancelled",stoppingToken);
				ElectroMetrics.CurrentOrders.Set(activeOrders);
				_logger.LogInformation(
						"Updated current active orders metrics:{ActiveOrders}",activeOrders);
			}
			catch (Exception ex)
			{
				_logger.LogError(
						ex,
						"Failed to updata current active orders  metrics");
			}
			await Task.Delay(
					TimeSpan.FromSeconds(15),
					stoppingToken);
		}
	}
}


