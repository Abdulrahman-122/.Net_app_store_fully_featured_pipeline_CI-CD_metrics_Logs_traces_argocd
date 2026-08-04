from flask  import render_template,url_for,flash,redirect,request,abort,Blueprint
from flaskblog.posts.forms import PostForm,DeletePostForm
from flaskblog import db
from flaskblog.models import User,Post
from flask_login import current_user ,login_required
from flask import current_app     # this  is a flask obj application used to get the current running app that is running



posts= Blueprint('posts',__name__)


@posts.route('/post/new',methods=['GET','POST'])
@login_required
def new_post():
    form=PostForm()
    if form.validate_on_submit():
        post=Post(
            title=form.title.data,
            content=form.content.data,
            author=current_user
        )
        db.session.add(post)
        db.session.commit()
        flash('You have added new Post successfully!!!','success')
        return  redirect(url_for('main.home'))

    return render_template('create_post.html',form=form,title='Create_post',legend='Create New Post')

@posts.route('/post/<int:post_id>')                #this creates a dynamic routing with URL parameter
@login_required
def post(post_id):
    post=Post.query.get_or_404(post_id)          # this bring the post with that id as you know that every post has an id the database gives to it when storing in it 
    form=DeletePostForm()
    return render_template('post.html',post=post,form=form,title=post.title)

@posts.route('/post/<int:post_id>/update',methods=['POST','GET'])
@login_required
def update_post(post_id):

    post=Post.query.get_or_404(post_id)

    if post.author!=current_user:
        abort(403)

    form=PostForm()
    #this is after the user change the content of the form from the old one to the newest one it will be sended to the database
    if form.validate_on_submit():
        post.title=form.title.data
        post.content=form.content.data
        db.session.commit()
        flash('Post Updated successfully!!!','success')
        return redirect(url_for('posts.post',post_id=post.id))
 
 # this elif runs once you open the old post before you update it
    elif request.method=="GET":
        form.title.data=post.title
        form.content.data=post.content
    return render_template('create_post.html',title='Update Post',legend='Update Post',form=form)  # at the end what will happend -> the form you creted will be sent to the create_post page to put in but the title of the post will be update post+ the new title of the page will be update post

@posts.route('/post/<int:post_id>/delete',methods=['POST'])
@login_required
def delete_post(post_id):

    post=Post.query.get_or_404(post_id)

    if post.author!=current_user:
        abort(403)
    db.session.delete(post)
    db.session.commit()
    flash('Post had been deleted successfully!!!!','success')
    return redirect(url_for('main.home'))
@posts.route('/user_posts/<string:username>/',methods=['GET','POST'])
@login_required
def user_posts(username):

    page=request.args.get('page',1,int)       #fetch the url 
    user=User.query.filter_by(username=username).first_or_404()  #get the first user that match username
    posts=Post.query.filter_by(author=user).order_by(Post.date_posted.desc()).paginate(page=page,per_page=3) # do pagination on the username posts desc by date_posted

    return render_template('user_posts.html',posts=posts,user=user)