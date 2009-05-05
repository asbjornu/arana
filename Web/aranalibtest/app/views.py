from django.http import HttpResponse
from django.template import Context, loader

# Google App Engine imports.
from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app

def index(request):
  template = loader.get_template('index.html')
  return HttpResponse(template.render(Context()))

def simple_post_test(request):
  # initialize request values
  context = Context({
    'textbox'   : request.POST.get('textbox',   ''),
    'radio'     : request.POST.get('radio',     ''),
    'textarea'  : request.POST.get('textarea',  ''),
    'select'    : request.POST.get('select',    ''),
    'submit'    : request.POST.get('submit',    ''),
  })
  
  # set up the template
  template = loader.get_template('simple_post_test.html')
  
  # render the thing
  return HttpResponse(template.render(context))