# encoding: utf-8
from __future__ import unicode_literals

import web

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
        return u'insert danmaku successfully'
