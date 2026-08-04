from flask import Blueprint,Response
from prometheus_client import Counter,generate_latest,Gauge,Histogram


monitoring=Blueprint("monitoring",__name__)

http_requests=Counter(
        "http_requests_total",
        "Total number of http requests"
)
active_users=Gauge(
        "active_users",
        "Current logged in users"
        )
request_latency=Histogram(
        "request_latency_seconds",
        "HTTP request latency"
        )
http_errors=Counter(
        "http_errors_total",
        "HTTP errors",
        ["status_code"]
        )
@monitoring.route("/metrics")
def metrics():
    return Response(
            generate_latest(),
            mimetype="text/plain"
            )  #now we want the response to be in text format type plain.


print("Monitoring Gauge ID:",id(active_users))
#----------------------------------------------

database_queries=Counter(
        "database_queries_total",
        "Total database queries executed"
        )






