# encoding: utf-8
from __future__ import unicode_literals

import sqlite3
import web

from Utils import DBUtils
from Models import Danmaku

class UploadDanmaku:
    def GET(self):
        return None

    def POST(self):
        content = web.input()
        channel_id = content.get('channel_id')
        danmaku = content.get('danmaku')
        date = content.get('date')
        for key, val in content.items():
            print(key, ': ', val)

        conn = DBUtils.get_connection()
        Danmaku.create_danmark_from_args(channel_id=channel_id, content=danmaku, date=date).insert(conn)
        DBUtils.release_connection(conn)
        return u'insert danmaku successfully'
