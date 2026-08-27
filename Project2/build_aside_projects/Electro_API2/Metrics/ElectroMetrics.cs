using Prometheus;
namespace ElectroAPI.Metrics;
public static class ElectroMetrics
{
	public  static  readonly Counter UsersCreated=Prometheus.Metrics.CreateCounter(
			"electro_users_created_total",
			"Total number of users successfully created." );
	public static  readonly Counter OrdersCreated=Prometheus.Metrics.CreateCounter(
			"electro_orders_created_total",
			"Total number of orders created." );
	public static  readonly Counter PaymentsProcessed= Prometheus.Metrics.CreateCounter( 
			"electro_payments_processed_total",
			"Total number of payments processd."  );
	public static readonly Counter LoginAttempts=Prometheus.Metrics.CreateCounter(
			"electro_login_attempt_total",
			"Total number of attempt total"
			);
	public static readonly Counter LoginFailures=Prometheus.Metrics.CreateCounter(

			"electro_login_failures_total",
			"Total number of failed login attempts"

			);
	public static readonly Gauge CurrentUsers=Prometheus.Metrics.CreateGauge(
		"electro_users_current",
		"Current number of users."
		);
	public static readonly Gauge CurrentOrders=Prometheus.Metrics.CreateGauge(
			"electro_orders_current",
			"Current number  of  active orders"
			);

	public static readonly Histogram OrderCreationDuration=Prometheus.Metrics.CreateHistogram(
			"electro_order_creation_duration_seconds",
			"Time spent  Creating an order.");
	public  static readonly  Histogram PaymentDuration=Prometheus.Metrics.CreateHistogram(
			"electro_payment_duration_seconds",
			"Time spent processing a payment"
			);

		
// this histogram of payment  we will do it later   as no payment yet .
}
