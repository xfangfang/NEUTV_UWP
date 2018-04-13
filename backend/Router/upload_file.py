# encoding: utf-8
from __future__ import unicode_literals

import web

upload_file_html = '''
<html><head></head><body>
<form method="POST" enctype="multipart/form-data" action="">
<input type="file" name="filedict" />
<br/>
<input type="submit" />
</form>
</body></html>
'''

class UploadFile:
    def GET(self):
        return upload_file_html

    def POST(self):
        f = web.input(filedict={})
        print(type(f))
        web.debug(f['filedict'].filename) # 这里是文件名
        web.debug(f['filedict'].value) # 这里是文件内容
        web.debug(f['filedict'].file.read()) # 或者使用一个文件对象
        raise web.seeother('/upload')
