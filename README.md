#FilesBackuper

FileBackuper是一个简单的无UI文件/文件夹复制工具。可应用与文件服务器等需要时常进行备份的环境中。

使用时需现在配置文件```config.ini```中设定好备份模式，源路径和目标路径。

Option为备份模式，有以下三种可选：
 F - 全量备份，全部的备份模式。
 D - 差异备份（WIP）
 I - 增量备份（WIP）
SourcePath为需备份的源路径。
TargetPath为需要放置备份的路径。

备份会以时间点命名的文件夹架构进行区分不同版本，下面是一个简单的示例，Backup是```TargetPath```，test是```SourcePath```：
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