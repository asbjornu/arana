from django.http import HttpResponse
from django.template import Context, loader

# Google App Engine imports.
from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app

def home(request):
  template = loader.get_template('main.html')
  return HttpResponse(template.render(Context()))