# -*- coding: utf-8 -*-
from __future__ import unicode_literals

import web

from Models import *


# WARNING: 好矫情，写中文吧。
#           因为我实在是太菜了，所以下面的的“XXXX_list”参数都是一个tuple列表
#           XXXX_list = [(channel_id, content, date) from resultset of database]
#           原来的模块类静态函数query_by_period(conn, ....)是返回一个对象列表的，我还留着没删
#           但是后来我嫌麻烦又写了一个函数query_by_period(conn, ....)是返回元组列表的
#           如果有用对象完全代替数据库理想主义者和高手，能不能指导一下怎么写出优美ORM代码，拜托了OTZ
#           还是要多学习啊，这样下去连饭都没得吃

def generate_danmaku_xml(danmaku_list, templates_path = 'templates'):
    render = web.template.render(templates_path)
    web.header('Content-Type', 'text/xml')
    return render.danmaku(danmaku_list)


def generate_comment_xml(comment_list, templates_path = 'templates'):
    render = web.template.render(templates_path)
    web.header('Content-Type', 'text/xml')
    return render.comment(comment_list)
