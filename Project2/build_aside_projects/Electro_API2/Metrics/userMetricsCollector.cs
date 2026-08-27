using ElectroAPI.Data;
using ElectroAPI.Metrics;
using Microsoft.EntityFrameworkCore;
namespace ElectroAPI.Services;
public class UserMetricsCollector : BackgroundService
{
	private readonly  IServiceScopeFactory _scopeFactory;
	private readonly  ILogger<UserMetricsCollector> _logger;
	public UserMetricsCollector(
			IServiceScopeFactory scopeFactory,
			ILogger<UserMetricsCollector> logger)
	{
		_scopeFactory=scopeFactory;
		_logger=logger;
	}
// note  now we have built a construction in order to access the outer services->scopeFactory,logger
	protected override  async Task ExecuteAsync(
			CancellationToken stoppingToken)
	{

		while(!stoppingToken.IsCancellationRequested)
		{

			try
				{
					using var scope=_scopeFactory.CreateScope();  //create  a scope in memory
					var  db=scope.ServiceProvider.GetRequiredService<ElectroDbContext>(); //calculate  the total rows  in the database table called customers 
					var userCount=await db.Customers.CountAsync(stoppingToken);
// this will define how many rows  of customers inside that table so that it will be done async or while other code running 
					ElectroMetrics.CurrentUsers.Set(userCount);
					//update current users -> inside dashboard
					_logger.LogInformation(
							"Update current users metric:{UserCount}",userCount);
				}
			catch(Exception ex)
			{
				_logger.LogError(
						ex,
						"Failed to  update current users metrics");
			}
			await  Task.Delay(
					TimeSpan.FromSeconds(15),
					stoppingToken);
		}
	}

}




