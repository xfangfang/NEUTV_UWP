#如何部署服务器端？

##前提

请先确保你的shell环境中的python安装了web.py这个轻量级的web框架

- 克隆我们dalao的repo
```shell
git clone https://github.com/xfangfang/NEUTV_UWP.git
```
- 然后进入后端代码所在的目录
```shell
cd NEUTV_UWP/backend
```
- 生成sqlite3数据库
```shell
pyhton migrate.py
```
- 现在，可以跑服务器端了！
```shell
python light_weight_server.py
```
- 最后，祝你身体健康，再见
