# start from database untill finish it then go to the next topic
from datetime import datetime
from flask_sqlalchemy import SQLAlchemy
from flaskblog import db 
from flaskblog import login_manager   #  look at __init__.py -> import from it the login_manager
from flask_login import UserMixin



class User(db.Model,UserMixin):
    id=db.Column(db.Integer,primary_key=True)
    username=db.Column(db.String(20),unique=True,nullable=False)
    email=db.Column(db.String(120),unique=True,nullable=False)
    image_file=db.Column(db.String(20),nullable=False,default='default.jpg')
    password=db.Column(db.String(60),nullable=False)       # why we make it 60 characters as any bycrypt(hashed pass) it's length=60 + later we will convert the string password using bycrypt to gurantee that it's encrypted
    posts=db.relationship('Post',backref='author',lazy=True) 
    def __repr__(self):
        return f"User('{self.username}','{self.email}','{self.image_file}')"

@login_manager.user_loader        #Loginmanager -> this is the variable that we decalered in our  init  file  
def load_user(user_id):      #behind the scene -> flask-login -> get userid and put it into load_user(user_id) to see whether that user in the database or not in order to take the action
    return User.query.get(int(user_id))   #this return all the info about the user (email,name....) + you must use get not all as get take id of user and return it as object 




class Post(db.Model):  #   we need id for each post,title for each post,date of posting for each post,content for each post,user_id for each post
    id=db.Column(db.Integer,primary_key=True)
    title=db.Column(db.String(100),nullable=False)
    date_posted=db.Column(db.DateTime,nullable=False,default=datetime.utcnow) # make sure that if user didn't write the date of the post => put date at the current moment when he add that post by default
    content=db.Column(db.Text,nullable=False)
    user_id=db.Column(db.Integer,db.ForeignKey('user.id'),nullable=False)
    def __repr__(self):
        return f"Post('{self.title}','{self.date_posted}')"


# notes;
# backref='author' means:

# On User you have .posts to get posts

# On Post you have .author to get the user

# lazy=True means:

# Related posts are loaded only when you ask for them, 
# not when you get the user initially
# Every User has an attribute .posts which is a list of their Post objects.

# Every Post has an attribute .author which is the User who created it.
# __repr__ defines the string shown when printing or inspecting your object.

# It helps debugging by showing meaningful info about the object.

# What 'user.id' really means

# This string:

# 'user.id'


# means:

# “The id column in the user table”

# So SQLAlchemy reads this as:

# FOREIGN KEY (user_id) REFERENCES user(id)


# This is pure database language, not Python.




# from datetime import datetime
# from app import app
# from models import db, User, Post

# with app.app_context():
#     # Create user
#     user = User(
#         username='Abdulrahman',
#         email='email',
#         password='hashed_password'
#     )
#     db.session.add(user)
#     db.session.commit()
#     print(f"User created with id: {user.id}")

#     # Create post linked to user
#     post = Post(
#         title="About sports",
#         date_posted=datetime.strptime("12/12/2026", "%m/%d/%Y"),
#         content="How sports is good for our bodies if we use it well and avoid excessive TV.",
#         user_id=user.id  # link post to the created user
#     )
#     db.session.add(post)
#     db.session.commit()
#     print("Post inserted")
# # ...      
# user inserted
# [<User 1>]
# >>> user.username
# 'Abdulrhamn'
# >>> user.email
# 'abdulrahman@gmail.com'
# >>> user.password
# '1234add'
# >>> 




# now; now check post column then go to see mariadb to check your data whether it contain on data or not then complete building the app??

# this is how to test your database through python;
# with app.app_context():
    # Get all users
    # users = User.query.all()
    # print("Users:")
    # for user in users:
        # print(user)  # This calls __repr__ if defined
# 
    # Get all posts
    # posts = Post.query.all()
    # print("\nPosts:")
    # for post in posts:
        # print(post)  # Calls __repr__ too
        # print("Author:", post.author.username)  # because of backref 'author' in Post
# 