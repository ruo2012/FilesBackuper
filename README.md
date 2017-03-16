#FilesBackuper

FileBackuper是一个简单的无UI文件/文件夹复制工具。可应用于文件服务器等需要时常进行备份的环境中。备份同时会记录源文件路径和修改时间，以用于差异备份和增量备份。数据库使用SQLite。
开发基于.Net Framework 4.5，请确保已经安装了此环境。
*默认程序实行完成后不会自动关闭进程，如有需要请在``MainForm_Load``方法中加入``Close()``;

[更新日志](https://git.oschina.net/famio/FilesBackuper/blob/master/Changelogs.md)

![FilesBackuper](https://www.famio.cn/io/post/pictures/badges/master.svg "来自绯末")
感谢：[啊你吓到我啦](https://git.oschina.net/augustpluscn)  一位有问必答的好老师

##配置说明

使用时需先在配置文件``config.ini``中设定备份模式，源路径和目标路径。

``Option``为备份模式，有以下三种可选：
```
 F - 全量备份，全部的备份模式。
 D - 差异备份，和最后一次全量备份做比对，备份新增和有修改的文件。
 I - 增量备份（WIP）
```

``SourcePath``为需备份的源路径。
``TargetPath``为需要放置备份的路径。

##软件文件架构
``FileBackuper.exe`` 主程序。
``FilesDetails.db`` 数据库文件，用于记录文件名称和修改时间。
``config.ini`` 配置文件，用于设定备份模式和相关路径。

##备份文件夹架构
备份会以时间点命名的文件夹架构进行区分不同版本，下面是一个简单的示例，Backup是``TargetPath``，test是``SourcePath``：
```
Backup
 -F2017_3_2 21_54_00
   -test
     -1.txt
     -2.txt
     -3.txt
     -4.txt
 -F2017_3_2 22_13_21
   -test
     -1.txt
 -F2017_3_2 23_11_56
 -F2017_3_3 01_38_00
 -F2017_3_3 20_04_00
```

##使用场景范例

某台文件服务器SVR1，需要定期做文件备份，并放置SVR2上。
在SVR1上映射SVR2的备份路径作为网络驱动器``S:``，FileBackuper放置在SVR1上，并设置好``config.ini``中的``SourcePath``为SVR1的需备份文件路径，以及``TargetPath``为驱动器``S:``。
在SVR1上设定排程，每天凌晨1点执行一次FileBackuper.exe。