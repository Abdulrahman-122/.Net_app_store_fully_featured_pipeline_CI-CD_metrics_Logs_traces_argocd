import logging
import os
import sys
import time

from flask import Flask, g  # g namespace scoped for a single requests.
from flask_bcrypt import Bcrypt
from flask_login import LoginManager
from flask_mail import Mail
from flask_migrate import Migrate
from flask_sqlalchemy import SQLAlchemy

#for opentelemetry 
from opentelemetry import trace
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
from opentelemetry.instrumentation.flask import FlaskInstrumentor
from opentelemetry.sdk.resources import Resource
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor

from flaskblog.config import Config

    #as you see this recent two lines ->upload that folder name when need it instead of hardcoding of it.
db=SQLAlchemy()   # create database manager to make tables and make your backend talk to the database
bcrypt=Bcrypt()
login_manager=LoginManager()
login_manager.login_view='users.login'   # if the user not logged in -> redirect him to the login page
mail=Mail()

migrate=Migrate()


logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s %(levelname)s %(message)s",
        handlers=[
            logging.StreamHandler(sys.stdout)    #this define the type of the stream that we want flask to extract from the app
            ],
        )

#for opentelemetry
resource=Resource.create({
    "service.name": os.environ.get(
        "OTEL_SERVICE_NAME",
        "flaskblog"
)
    }
                         )
provider=TracerProvider(resource=resource)
exporter=OTLPSpanExporter (
        endpoint="http://otel-opentelemetry-collector.tracing.svc.cluster.local:4317",insecure=True) 


processor=BatchSpanProcessor(exporter)
provider.add_span_processor(processor)
trace.set_tracer_provider(provider)

def create_App(config_class=Config):  
    """return the app manager that we will use to build application"""
    app=Flask(__name__)
    app.config.from_object(Config)
    db.init_app(app)
    bcrypt.init_app(app)
    login_manager.init_app(app)   #this connect your flask app to flask login
    mail.init_app(app)
    FlaskInstrumentor().instrument_app(app)
    # from flaskblog import models
    migrate.init_app(app,db)
    from flaskblog.errors.handlers import errors
    from flaskblog.main.routes import main
    from flaskblog.monitoring.routes import (
        http_errors,
        http_requests,
        monitoring,
        request_latency,
    )
    from flaskblog.posts.routes import posts
    from flaskblog.users.routes import users
    @app.before_request
    def count_requests():
        http_requests.inc()
    @app.before_request  #you must add this decorator else flask wouldn't run this  function.
    def before_request():
        g.start_time=time.perf_counter()
# time.perf_counter -> this will estimate the current time for that request in a high resolution way.
    @app.after_request
    def start_request(response):
        duration=time.perf_counter() - g.start_time
        request_latency.observe(duration)
        if response.status_code >= 400:
            http_errors.labels(
                    status_code=str(response.status_code)).inc()
        return response 
    from sqlalchemy import event

    from flaskblog.monitoring.routes import database_queries
    with app.app_context():
         @event.listens_for(db.engine,"before_cursor_execute")
         def count_queries(conn,cursor,statement,parameters,context,executemany,):
             database_queries.inc()

    app.register_blueprint(main)
    app.register_blueprint(posts)
    app.register_blueprint(users)
    app.register_blueprint(errors)
    app.register_blueprint(monitoring)
    app.logger.info("Flask application started")

    return app

