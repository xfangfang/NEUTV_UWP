# encoding: utf-8
from __future__ import unicode_literals

import web

router = (
    '/', 'Index',
    '/upload_file', 'Router.upload_file.UploadFile',
    # '/upload_danmaku', 'UploadDanmaku',
    # '/upload_comment', 'UploadComment',
)

class Index:
    def GET(self):
        return u'你想干嘛？'

    def POST(self):
        return None


if __name__ == "__main__":
    app = web.application(router, globals())
    app.run()
