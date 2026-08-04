to make containers for that application

build docker-compose.yaml 
  
    services:
    flask:
      build: .
      container_name: backend_flask
      ports:
        - "8001:5000"
      environment:
        DATABASE_URL: mysql+pymysql://maria:AppSecrets@mariadb/My_maria
        FLASK_APP: run.py
      healthcheck:
        test: ["CMD","curl","-f","127.0.0.1"]
        interval: 20s
        timeout: 10s
        retries: 5
      depends_on:
        mariadb:
           condition: service_healthy
   
  
    mariadb:
      image: mariadb:latest
      healthcheck:
        test: ["CMD","mariadb-admin","ping","-h","127.0.0.1","-u","root","-p1234Root"]
        interval: 10s
        timeout: 5s
        retries: 5
      container_name: database_maria
      environment:
        MYSQL_ROOT_PASSWORD: 1234Root
        MYSQL_DATABASE: My_maria
        MYSQL_USER: maria
        MYSQL_PASSWORD: AppSecrets
      volumes:
        - maria_data:/var/lib/mysql
    volumes:
      maria_data:


    then build; 
      Dockerfile
    FROM python:3.14-slim
    RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
    WORKDIR /app
    COPY requirements.txt .
    RUN pip install -r  requirements.txt
    COPY . .
    CMD ["python","run.py"]
then;

    pip freeze > requirements.txt

then;
start compose the file
         
    docker  compose up --build
    then migrate everything(convert code to tables inside mariadb)
    docker exec -it flask_backend bash
    flask db init
    flask db migrate -m "initial"
    flask db upgrade 
    

this is from the container;
     this is flask_container
  <img width="1037" height="110" alt="image" src="https://github.com/user-attachments/assets/0852d477-7aaf-4cb9-a771-58a559878c02" />
    this is maria container
    <img width="1265" height="101" alt="image" src="https://github.com/user-attachments/assets/a36c9a3f-08a7-4f2c-bfa3-916a577c942b" />
from here you will see two dockers running 
<img width="1312" height="166" alt="image" src="https://github.com/user-attachments/assets/0afe4d77-6405-49f2-8b40-26b1f75cae3f" />
and then i stop them then removed them;
<img width="694" height="249" alt="image" src="https://github.com/user-attachments/assets/b8dcd90f-ff4f-4bb1-8ef1-385be559d011" />
after removing them everything stopped;
<img width="1299" height="653" alt="image" src="https://github.com/user-attachments/assets/d30bb079-54ba-4795-b1f3-487c6087043d" />




Step 2. Generate the migration against an empty database

Right now your local database already has the tables, so Alembic sees nothing to do.

You have two choices.

Option A (recommended): Create a fresh local database

For example:

CREATE DATABASE flask_migration_test;

Then temporarily point your app to it:

export DATABASE_URL="mysql+pymysql://root:password@localhost/flask_migration_test"

Now run:

flask --app run.py db init      # only if migrations doesn't exist
flask --app run.py db migrate -m "Initial migration"

This time Alembic should print:

Detected added table 'user'
Detected added table 'post'

and create a file in:

migrations/
└── versions/
    xxxxxxxxx_initial_migration.py