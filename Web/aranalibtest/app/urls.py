# -*- coding: utf-8 -*-
from django.conf.urls.defaults import *

urlpatterns = patterns('app.views',
  (r'^$',                   'index'),  
  (r'^simple_post_test/$',  'simple_post_test'),
  (r'^login_test/$',        'login_test'),
  (r'^redirect_test/$',     'redirect_test'),
  (r'^create_admin_user$',  'create_admin_user'),
  (r'^create/$',            'add_person'),
  (r'^show/(?P<key>.+)$',   'show_person'),
  (r'^edit/(?P<key>.+)$',   'edit_person'),
  (r'^delete/(?P<key>.+)$', 'delete_person'),
  (r'^download/(?P<key>.+)/(?P<name>.+)$', 'download_file'),
)
