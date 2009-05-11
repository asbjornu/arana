from django.http import HttpResponse
from django.template import Context, loader

# Google App Engine imports.
from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app

import logging

def index(request):
  template = loader.get_template('index.html')
  return HttpResponse(template.render(Context()))

def simple_post_test(request):
  # initialize the request context
  context = Context(request.POST)
  
  logging.debug('Post data:')
  logging.debug(request.POST)
  
  # set up the template
  template = loader.get_template('simple_post_test.html')
  
  # render the template
  response = template.render(context)
  
  return HttpResponse(response)