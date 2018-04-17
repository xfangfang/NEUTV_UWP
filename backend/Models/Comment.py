# encoding: utf-8
from __future__ import unicode_literals

import sqlite3
import web

from .Model import Model

class Comment(Model):

    def __init__(self, channel_id = u'default', content = u'default', date = u'1111-11-11 00:00:00'):
        self.channel_id = channel_id
        self.content = content
        self.date = date

    def insert(self, conn):
        try:
            conn.execute(
                "insert into comment(channel_id, content, date) values(?, ?, ?)",
                (self.channel_id, self.content, self.date)
                )
            conn.commit()
            return True
        except Exception as e:
            print(e)
            print('insert comment failed %s, %s, %s', self.channel_id, self.content, self.date)
            return False

    # static function
    def query_by_period(conn, beg, end, channel_id):
        '''
        insert the object itself to the database by connection
        '''
        cache = []
        curr = conn.cursor()
        sql = 'select channel_id, content, date from comment' +\
        ' where datetime(date)>=datetime(?) and datetime(?)>=datetime(date) and channel_id=?'
        try:
            for tmp in  curr.execute(sql, (beg, end, channel_id)).fetchall():
                cache.append(create_comment_from_tuple(tmp))
        except Exception as e:
            print(e)
            raise AttributeError
        finally:
            curr.close()

        return cache

    # static function
    def query_by_period_tuples(conn, beg, end, channel_id):
        '''
        the static method return a list of tuples instead of objects
        tuples: (channel_id, content, date)
        '''
        sql = 'select channel_id, content, date from comment' +\
        ' where datetime(date)>=datetime(?) and datetime(?)>=datetime(date) and channel_id=?'
        try:
            return conn.execute(sql, (beg, end, channel_id)).fetchall()
        except Exception as e:
            print(e)
            raise AttributeError
            return None

    def __str__(self):
        return self.content

def create_comment_from_args(
    channel_id = u'default', content = u'default', date = u'1111-11-11 00:00:00'
    ):
    return Comment(channel_id, content, date)

def create_comment_from_tuple(tp):
    return Comment(tp[0], tp[1], tp[2])
