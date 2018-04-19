# -*- coding: utf-8 -*-
from __future__ import unicode_literals

import web

from Models import *

def generate_danmaku_xml(danmaku_list, templates_path = 'templates'):
    render = web.template.render(templates_path)
    web.header('Content-Type', 'text/xml')
    return render.danmaku(danmaku_list)


def generate_comment_xml(comment_list, templates_path = 'templates'):
    render = web.template.render(templates_path)
    web.header('Content-Type', 'text/xml')
    return render.comment(comment_list)
