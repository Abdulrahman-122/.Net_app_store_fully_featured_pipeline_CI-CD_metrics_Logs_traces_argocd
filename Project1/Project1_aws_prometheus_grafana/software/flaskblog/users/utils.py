from itsdangerous import URLSafeTimedSerializer 
from flaskblog import mail
from flask import url_for,current_app      # we use current_app instead of app to avoid circular import
from flask_mail import Message  #as you didn't create an object of it inside init
import secrets,os
from PIL import Image
def generate_reset_token(user_id):  
    """Generate a token that will be sent to the user email in order to check it's credentials."""
    secret_key=current_app.config['SECRET_KEY']
    serializer=URLSafeTimedSerializer(secret_key)
    token=serializer.dumps(user_id,salt='Password-reset-salt')
    return token
def check_reset_token(token,expiration=1800):      #expiration -> the reset link that will be sent to the user will be expire after 30 minutes
    """check token that is clicked by the user to see whether it is the userid or not"""
    secret_key=current_app.config['SECRET_KEY']
    serializer=URLSafeTimedSerializer(secret_key)
    try:
        check_token_is_userid=serializer.loads(token,salt='Password-reset-salt',max_age=expiration)
   

    except:
        return None
    return check_token_is_userid



#if you didn't understand this two function please -> review info_about_project you will find how to handle itsdangerous or you can ask any chat about itsdangerous explaination
#don't give up


def send_reset_email(user_email,token):
    msg=Message(subject='Password reset request',sender=current_app.config['MAIL_USERNAME'],recipients=[user_email])
    link=url_for('users.change_password',token=token,_external=True)
    #_external -> mandatory in order to send to email absolute path to gmail to accept it  
    #link should be go to change_password.html not send_token.html as we was there before send link to gmail
    msg.body=f"""
To reset your password, click the link below:
{link}
if you didn't click ignore this email for ever and do new one if you want.  
Ms:{user_email}
thank you.  
"""
    mail.send(msg)


def  save_picture(form_picture):
    random_hex=secrets.token_hex(8)    #generate random hexadicemal numbers  to put it for the image name to protect collisions
    _, ext=os.path.splitext(form_picture.filename)
    picture_filename=random_hex+ext
    picture_path=os.path.join(current_app.root_path,'static/images',picture_filename)
    # this create the path of image   thatwe will save to it
    #here we  need  to resize  the image to be small and don't  take more time to be  send  from db to browser
    output_size=(125,125)
    i=Image.open(form_picture)
    i.thumbnail(output_size)
    i.save(picture_path)
    
    return picture_filename 
