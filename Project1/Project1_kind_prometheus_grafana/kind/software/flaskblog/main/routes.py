from flask  import render_template,url_for,flash,redirect,request,abort,Blueprint
from flaskblog.posts.forms import PostForm,DeletePostForm
from flaskblog import db
from flaskblog.models import User,Post
from flask_login import current_user ,login_required
from flask import current_app     # this  is a flask obj application used to get the current running app that is running

main=Blueprint('main',__name__)




@main.route('/')
@login_required       
def home():
    page=request.args.get('page',1,int) 
    posts=Post.query.order_by(Post.date_posted.desc()).paginate(page=page,per_page=5)
    return render_template('home.html',posts=posts)

@main.route('/about',methods=['POST','GET'])
@login_required 
def  about():

    return render_template('about.html',title="About")




