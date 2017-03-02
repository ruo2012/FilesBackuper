using System;
using System.IO;
using System.Windows.Forms;

namespace FilesBackuper
{
    class Program
    {
        static void Main(string[] args)
        {
            string stringFileTempPath = Application.ExecutablePath; //把程序主路径改为小写
            string realIniPath = stringFileTempPath.Replace("FilesBackuper.exe", "config.ini");
            Library.Ini ini = new Library.Ini(realIniPath);
            string iniOption = ini.ReadValue("Main", "Option");
            string iniSource = ini.ReadValue("Main", "SourcePath");
            string iniTarget = ini.ReadValue("Main", "TargetPath");
            string datetime = DateTime.Now.ToString().Replace(":", "_").Replace("/", "_");  //获取当前时间并把：和/转换为_
            if (Directory.Exists(iniSource) && Directory.Exists(iniTarget)) //判断源文件夹和目标文件夹是否存在
            {
                switch (iniOption)
                {
                    case "F":   //全量备份，同时记录到数据库
                        fileOperation fo = new fileOperation();
                        fo.CopyDirectory(iniSource, iniTarget + iniOption + datetime.ToString());
                        //Console.WriteLine(datetime);
                        //Console.ReadKey();
                        break;
                    case "D":   //差异备份

                        break;
                    case "I":   //增量备份

                        break;
                    default:
                        MessageBox.Show("ini配置文件中[Option]值错误，请检查：\nF - 全量备份\nD - 差异备份\nI - 增量备份");
                        break;
                }
            }
            else
            {
                Console.WriteLine("目录未找到！");
            }
        }
    }
}
