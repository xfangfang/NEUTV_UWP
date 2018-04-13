# -*- coding: utf-8 -*-
from __future__ import unicode_literals
from queue import Queue

import sqlite3

connection_pool = Queue()

def get_connection(path = './danmaku_comment.db'):
    while connection_pool.qsize() < 3:
        try:
            connection_pool.put(sqlite3.connect(path))
        except Exception as e:
            print(e)
            return None

    return connection_pool.get()


def release_connection(conn, path = './danmaku_comment.db'):
    if conn is not None:
        connection_pool.put(conn)
