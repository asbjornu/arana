# -*- coding: utf-8 -*-
from django.core.urlresolvers import reverse
from django.contrib.auth.models import User
from django.http import \
     HttpResponse, Http404, HttpResponsePermanentRedirect
from django.template import Context, loader
from django.views.generic.list_detail import object_list, object_detail
from django.views.generic.create_update import \
     create_object, delete_object, update_object
from google.appengine.ext import db
from mimetypes import guess_type
from app.forms import PersonForm
from app.models import Contract, File, Person
from ragendja.dbutils import get_object_or_404
from ragendja.template import render_to_response
import logging

#
# Index view
#
def index(request):
  template = loader.get_template('index.html')
  return HttpResponse(template.render(Context()))

#
# Generic test method that calls the specific
# tests based on the 'test' string argument
#
def test(request, test):
  # hack to get a reference to the xyz_test() method
  exec('method = ' + test + '_test')

  # invoke the method with the request object as argument
  request = method(request)
  
  # return the request
  return request

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
  
  
#
# Redirect test view
#
def redirect_test(request):
  # Redirect if the "submit" button was pushed
  if (request.POST.get('submit', None)):
    return HttpResponsePermanentRedirect('http://code.google.com/p/arana/')

  # set up the template
  template = loader.get_template('redirect_test.html')
  
  # render the template
  response = template.render(Context())
  
  return HttpResponse(response)
  

#
# QueryString test view
#
def querystring_test(request):
  path = request.get_full_path()
  
  template = loader.get_template('querystring_test.html')
  
  context = Context({'querystring' : path})
  
  response = template.render(context)
  
  return HttpResponse(response)


def list_people(request):
    return object_list(request, Person.all(), paginate_by=10)

def show_person(request, key):
    return object_detail(request, Person.all(), key)

def add_person(request):
    return create_object(request, form_class=PersonForm,
        post_save_redirect=reverse('app.views.show_person',
                                   kwargs=dict(key='%(key)s')))

def edit_person(request, key):
    return update_object(request, object_id=key, form_class=PersonForm)

def delete_person(request, key):
    return delete_object(request, Person, object_id=key,
        post_delete_redirect=reverse('app.views.list_people'))

def download_file(request, key, name):
    file = get_object_or_404(File, key)
    if file.name != name:
        raise Http404('Could not find file with this name!')
    return HttpResponse(file.file,
        content_type=guess_type(file.name)[0] or 'application/octet-stream')

def create_admin_user(request):
    user = User.get_by_key_name('admin')
    if not user or user.username != 'admin' or not (user.is_active and
            user.is_staff and user.is_superuser and
            user.check_password('admin')):
        user = User(key_name='admin', username='admin',
            email='admin@localhost', first_name='Boss', last_name='Admin',
            is_active=True, is_staff=True, is_superuser=True)
        user.set_password('admin')
        user.put()
    return render_to_response(request, 'app/admin_created.html')
