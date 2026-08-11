from flask import (
    Blueprint,
    current_app,  # this  is a flask obj application used to get the current running app that is running
    render_template,
    request,
)
from flask_login import current_user, login_required

from flaskblog.models import Post
from flaskblog.monitoring.routes import http_requests

main=Blueprint('main',__name__)




@main.route('/')
@login_required       
def home():
    page=request.args.get('page',1,int) 
    posts=Post.query.order_by(Post.date_posted.desc()).paginate(page=page,per_page=5)
    http_requests.inc()
    current_app.logger.info(f"Home page visited by user {current_user.username}")
    return render_template('home.html',posts=posts)

@main.route('/about',methods=['POST','GET'])
@login_required 
def  about():
    current_app.logger.info(f"About page visited by user {current_user.username}")
    return render_template('about.html',title="About")




