# -*- coding: utf-8 -*-
from __future__ import unicode_literals

import web

render = web.template.render('templates', cache=False)

animal_list = [
    'lion',
    'bird',
    'human',
    'cat',
    'dog',
    'dragon',
]

class Hello:
    def GET(self):
        web.header('Content-Type', 'text/html')
        return render.test(animal_list)

    def POST(self):
        return None
