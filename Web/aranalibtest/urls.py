# -*- coding: utf-8 -*-
from django.conf.urls.defaults import *
from ragendja.urlsauto import urlpatterns
from ragendja.auth.urls import urlpatterns as auth_patterns
from django.contrib import admin
from app.forms import UserRegistrationForm
from app.urls import urlpatterns

admin.autodiscover()

handler500 = 'ragendja.views.server_error'

urlpatterns = urlpatterns   \
            + auth_patterns \
            + patterns('',
  ('^admin/(.*)', admin.site.root),
  (r'^$', 'django.views.generic.simple.direct_to_template', {'template': 'main.html'}),  
  # Override the default registration form
  url(r'^account/register/$', 'registration.views.register',
      kwargs  = {'form_class': UserRegistrationForm},
      name    = 'registration_register'),
) 