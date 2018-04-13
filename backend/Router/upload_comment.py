# encoding: utf-8
from __future__ import unicode_literals

import sqlite3
import web

from DBUtils import DBUtils
from Models import Comment

class UploadComment:
    def GET(self):
        return None

    def POST(self):
        content = web.input()
        comment = content.get('comment')
        date = content.get('date')
        channel_id = content.get('channel_id')
        for key, val in content.items():
            print(key, ': ', val)

        conn = DBUtils.get_connection()
        Comment.create_comment_from_args(channel_id=channel_id, content=comment, date=date).insert(conn)
        DBUtils.release_connection(conn)
        return u'insert comment successfully'
