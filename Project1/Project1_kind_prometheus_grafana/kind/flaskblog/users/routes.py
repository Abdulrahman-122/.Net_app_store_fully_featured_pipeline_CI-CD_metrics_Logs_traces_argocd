from flask  import Flask,render_template,url_for,flash,redirect,request,abort,Blueprint
from flaskblog.users.forms import RegistrationForm,LoginForm,UpdateProfileForm,RequestRestForm,ResetPasswordForm
from flaskblog.posts.forms import PostForm,DeletePostForm
from flaskblog import bcrypt,db
from flaskblog.models import User,Post
from flask_login import login_user,current_user ,login_required,logout_user
from flask import current_app     # this  is a flask obj application used to get the current running app that is running
import secrets
from  PIL import Image
import os
from flaskblog.users.utils import generate_reset_token,check_reset_token,send_reset_email,save_picture







users= Blueprint('users',__name__)

@users.route('/login',methods=['GET','POST'])
def login():
    if current_user.is_authenticated:     # is authen means user is already logged in .
        return redirect(url_for('users.profile'))

    form=LoginForm()
    if form.validate_on_submit():
        user=User.query.filter_by(email=form.email.data).first()              
        if user and bcrypt.check_password_hash(user.password,form.password.data):  
            login_user(user,remember=form.remember.data)                         #log the user in if he checked remember me button -> it's session will be saved even if he close the browser in the last time                   
            flash('You have logged in successfully!!!','success')                #bootstrap class to make button green
            return redirect(url_for('users.profile'))                                        
        else:
            flash('Login unsuccessfull.Please register first','danger')              #danger ->bootstrap class to make button red                           
            return redirect(url_for('users.register'))                                             
    return render_template('login.html', form=form)

@users.route('/register',methods=['GET','POST'])
def register():
    form=RegistrationForm()   #create registeration form to fill in by user
    if form.validate_on_submit(): # once form is filled and now user submit button
        hashed_pass=bcrypt.generate_password_hash(form.password.data).decode('utf-8') # convert bytes of hashed passwords that bcrypt generated from bytes of code to string using utf-8
        user=User(
            username=form.username.data,
            email=form.email.data,
            password=hashed_pass
        )
        db.session.add(user) # add new user to the current table 
        db.session.commit() # save the session so that the user will be added permenantly to the database table.
        flash('Your account has been created?you can now log in!','success')
        return redirect(url_for('main.home'))
    return render_template('register.html',title='Register',form=form)


@users.route('/admin/users',methods=["GET"])
def admin_users():
    from flaskblog.models import User
    with  current_app.app_context():
        users=User.query.all()
    return render_template('admin_users.html',users=users,title="Info_about_users")
#still add some validation here to prevent any user from login into this page to see data about other suers

@users.route('/logout',methods=["POST","GET"])
@login_required   #make sure that   the user is logged in before do this process
def logout():
    logout_user()                                      # this is class inside flask-login 
    flash('You have been logged out','info')         # flash temporary message for user to tell him that he now out of session
    return redirect(url_for('users.login'))                  # return him to login
@users.route('/profile',methods=['POST','GET'])
@login_required
def profile():
    form=UpdateProfileForm()
    if form.validate_on_submit():       # this  line  checks whehter user submit the button+ it's data is validates
        
        if form.picture.data:
            picture_file=save_picture(form.picture.data)
            current_user.image_file=picture_file     #update image into database

        current_user.username=form.username.data
        current_user.email=form.email.data     #these two  lines updata the username,email of this user in DB
        db.session.commit()                      #save  the changes to the DB
        flash('Your profile has been  updated!!!','success')
        return redirect(url_for('users.profile'))
    elif request.method=="GET":      #after redirection again to the same page(profile)+update the username,email fields with the updated info then render profile.html
        form.username.data=current_user.username
        form.email.data=current_user.email
    image_file=url_for('static',filename='images/'+current_user.image_file)      #create a url for the browser to load image from  
    return  render_template('profile.html',title='Profile',image_file=image_file,form=form)

@users.route('/reset_password',methods=['GET','POST'])
def send_token():

    form=RequestRestForm()

    if form.validate_on_submit():
        user=User.query.filter_by(email= form.email.data).first()
        if user:
            token=generate_reset_token(user.id) 
            send_reset_email(user.email,token)
        flash('If that email exists a message will be sent to your account to create a new Password!!!','info')

        return redirect(url_for('users.login'))

    return render_template('send_token.html',form=form)

    

@users.route('/reset_password/<token>',methods=['POST','GET']) #create new route for confirm+create new password 
def change_password(token):

    user_id=check_reset_token(token)

    if not user_id:
        flash('This email is invalid or expired','warning')
        return redirect(url_for('users.send_token'))
    user=User.query.get(user_id)  #after we checked the token and found a user id with it in db now let's pass the user_id into filter to get that user

    form=ResetPasswordForm()
    if form.validate_on_submit():
        hashed=bcrypt.generate_password_hash(form.password.data).decode('utf-8')
        user.password=hashed
        db.session.commit()   #save the changes into DB
        flash('You have been added new password !!!!','success')
        return redirect(url_for('users.login'))
    return  render_template('change_password.html',form=form)
