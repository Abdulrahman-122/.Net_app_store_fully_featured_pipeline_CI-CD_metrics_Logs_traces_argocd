from flask import Flask
from flask_login import LoginManager
from flask_sqlalchemy import SQLAlchemy
from flask_bcrypt import Bcrypt
from flask_mail import Mail
from flaskblog.config import Config
from flask_migrate import Migrate



  
    #as you see this recent two lines ->upload that folder name when need it instead of hardcoding of it.
db=SQLAlchemy()   # create database manager to make tables and make your backend talk to the database
bcrypt=Bcrypt()
login_manager=LoginManager()
login_manager.login_view='users.login'   # if the user not logged in -> redirect him to the login page
mail=Mail()

migrate=Migrate()

def create_App(config_class=Config):  
    """return the app manager that we will use to build application"""
    app=Flask(__name__)
    app.config.from_object(Config)
    db.init_app(app)
    bcrypt.init_app(app)
    login_manager.init_app(app)   #this connect your flask app to flask login
    mail.init_app(app)
    from flaskblog import models
    migrate.init_app(app,db)
    from flaskblog.main.routes import main
    from flaskblog.posts.routes import posts
    from flaskblog.users.routes import users 
    from flaskblog.errors.handlers import errors
    app.register_blueprint(main)
    app.register_blueprint(posts)
    app.register_blueprint(users)
    app.register_blueprint(errors)

    return app

