import os
from dotenv import load_dotenv
load_dotenv()        # load all variables from .env that contain secret key,database uri...


class Config:

    SECRET_KEY=os.getenv('SECRET_KEY')   # this line will take the secret key that you store into your .env 
    SQLALCHEMY_DATABASE_URI=os.getenv('DATABASE_URL') # this line will take the url for your database on your system 
    SQLALCHEMY_TRACK_MODIFICATIONS=False    # this ignore tracking changes in database using sqlAlchemy which made system heavy when run it.
    Upload_Folder=os.path.join('flaskblog/static/images')
    UPLOAD_FOLDER=Upload_Folder 
    MAIL_SERVER='smtp.gmail.com'
    MAIL_PORT=587
    MAIL_USE_TLS=True
    MAIL_USERNAME=os.getenv('MAIL_USERNAME')
    MAIL_PASSWORD=os.getenv('MAIL_PASSWORD') 