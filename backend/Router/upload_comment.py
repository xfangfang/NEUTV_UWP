# encoding: utf-8
from __future__ import unicode_literals

import web

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
        return u'insert comment successfully'
