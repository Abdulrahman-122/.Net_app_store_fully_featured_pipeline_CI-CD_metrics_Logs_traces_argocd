from flaskblog import create_App
app=create_App()     #no parameter will be passed into this function
if __name__=='__main__':
    app.run(host='0.0.0.0',debug=True,port=5000)  # inside container it will be 5000 but on your local machine it should be the other container that you put inside copose file  in my case:8001
    