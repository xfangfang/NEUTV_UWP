# -*- coding: utf-8 -*-

# WARNING: This python script just can help you to build a brand
#          new database rather than migrate the old one.
#          Please ensure that you read the docsself.
#          We would update the script in futrue to make it works for the existed
#          database.
from __future__ import unicode_literals

import sqlite3

def migrate(path = './danmaku_comment.db'):
    conn  = sqlite3.connect(path)
    tag = True
    try:
        conn.execute(
            "create table danmaku (id integer primary key," +
            "channel_id varchar(20), content varchar(100), date datetime)"
        )
        conn.execute(
            "create table comment (id integer primary key, " +
            "channel_id varchar(20), content text, date datetime)"
        )
        conn.commit()
    except Exception as e:
        tag = False
    finally:
        conn.close()
        return tag


def desc(path = './danmaku_comment.db'):
    conn  = sqlite3.connect(path)
    tag = True
    try:
        curr = conn.cursor()
        print('desc danmaku: ', curr.execute("pragma table_info(danmaku)").fetchall())
        print('desc comment: ', curr.execute("pragma table_info(comment)").fetchall())
    except Exception as e:
        tag = False
    finally:
        conn.close()
        return tag



if __name__ == "__main__":
    judge = migrate()
    if judge is False:
        print("Ops! Failed!")
    desc()
