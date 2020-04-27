from mara_app.app import MaraApp


app = MaraApp()

wsgi_app = app.wsgi_app
