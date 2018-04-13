# encoding: utf-8
from __future__ import unicode_literals

import sqlite3
import web

from .Model import Model

class Danmaku(Model):

    def __init__(self, channel_id = u'default', content = u'default danmaku content', date = u'1111-11-11 00:00:00'):
        self.channel_id = channel_id
        self.content = content
        self.date = date

    def insert(self, conn):
        try:
            conn.execute(
                "insert into danmaku(channel_id, content, date) values(?, ?, ?)",
                (self.channel_id, self.content, self.date)
                )
            conn.commit()
            return True
        except Exception as e:
            print("insert danmaku failed %s, %s, %s" % (self.channel_id, self.content, self.date))
            return False


def create_danmark_from_args(
    channel_id = u'default', content = u'default danmaku content', date = '1111-11-11 00:00:00'
    ):
    return Danmaku(channel_id, content, date)

def create_danmark_from_tuple(tp):
    return Danmaku(tp[0], tp[1], tp[2])