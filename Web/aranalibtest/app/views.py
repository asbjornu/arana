from django.http import HttpResponse
from django.template import Context, loader

# Google App Engine imports.
from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app

import logging

#
# Index view
#
def index(request):
  template = loader.get_template('index.html')
  return HttpResponse(template.render(Context()))
  
#
# Login test view
#
def login_test(request):
  """username  = request.POST.get('username', None)
  password  = request.POST.get('password', None)
  login     = 'Administrator' \
              if username == 'Administrator' \
                and password == '2fmckX32a' \
              else None  """
  
  logging.debug('Post data:')
  logging.debug(request.POST)
  
  context = Context(request.POST)
  
  # Set the session variable 'username' to the POSTed username
  # request.session['username'] = login
  
  # set up the template
  template = loader.get_template('login_test.html')
  
  # render the template
  response = template.render(context)
  
  return HttpResponse(response)

#
# Simple POST test view
#
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