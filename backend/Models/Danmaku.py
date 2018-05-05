# encoding: utf-8
from __future__ import unicode_literals

import sqlite3
import web

from .Model import Model

# type_list = [(
#     'top','bottom','rolling'
# )]

class Danmaku(Model):

    def __init__(self, channel_id = u'default', content = u'default danmaku content', date = u'1111-11-11 00:00:00', type = u'scroll'):
        self.channel_id = channel_id
        self.content = content
        self.date = date
        self.type = type

    def insert(self, conn):
        '''
        insert the object itself to the database by connection
        '''
        try:
            conn.execute(
                "insert into danmaku(channel_id, content, date, type) values(?, ?, ?, ?)",
                (self.channel_id, self.content, self.date, self.type)
                )
            conn.commit()
            return True
        except Exception as e:
            print("insert danmaku failed %s, %s, %s, %s" % (self.channel_id, self.content, self.date, self.type))
            return False

    def query_by_period(conn, beg, end, channel_id):
        '''
        the static method return a list consist of objects
        '''
        cache = []
        curr = conn.cursor()
        sql = 'select channel_id, content, date, type from danmaku ' +\
        'where datetime(date)>=datetime(?) and datetime(?)>=datetime(date) and channel_id=?'
        try:
            for tmp in  curr.execute(sql, (beg, end, channel_id)).fetchall():
                cache.append(create_danmaku_from_tuple(tmp))
        except Exception as e:
            print(e)
            raise AttributeError
        finally:
            curr.close()

        return cache

    def query_by_period_tuples(conn, beg, end, channel_id):
        '''
        the static method return a list of tuples instead of objects
        tuples: (channel_id, content, date, type)
        '''
        sql = 'select channel_id, content, date, type from danmaku where datetime(date)>=datetime(?)' +\
        ' and datetime(?)>=datetime(date) and channel_id=?'
        try:
            return conn.execute(sql, (beg, end, channel_id)).fetchall()
        except Exception as e:
            print(e)
            raise AttributeError
            return None

    def __str__(self):
        return self.content


def create_danmaku_from_args(
    channel_id = u'default', content = u'default danmaku content', date = '1111-11-11 00:00:00', type = u'scroll'
    ):
    return Danmaku(channel_id, content, date, type)

def create_danmaku_from_tuple(tp):
    return Danmaku(tp[0], tp[1], tp[2], tp[3])
