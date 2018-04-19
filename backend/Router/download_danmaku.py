# -*- coding: utf-8 -*-
from __future__ import unicode_literals

import web
import sqlite3

from Utils import DBUtils, FileUtils
from Models import Danmaku

class DownloadDanmaku:
    def GET(self):
        return None

    def POST(self):
        post = web.input()
        beg_date = post.get('beg_date')
        end_date = post.get('end_date')
        channel_id = post.get('channel_id')

        for key, val in post.items():
            print(key, val)

        conn = DBUtils.get_connection()
        danmaku_list = Danmaku.Danmaku.query_by_period_tuples(conn, beg_date, end_date, channel_id)
        DBUtils.release_connection(conn)

        return FileUtils.generate_danmaku_xml(danmaku_list)
