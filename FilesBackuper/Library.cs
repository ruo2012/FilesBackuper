using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace FilesBackuper
{
    class Library
    {
        public class Ini
        {
            // 声明INI文件的写操作函数 WritePrivateProfileString()
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

            // 声明INI文件的读操作函数 GetPrivateProfileString()
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            private string sPath = null;


            public Ini(string path)
            {
                this.sPath = path;
            }


            public void Writue(string section, string key, string value)
            {
                // section=配置节，key=键名，value=键值，path=路径
                WritePrivateProfileString(section, key, value, sPath);
            }

            public string ReadValue(string section, string key)
            {
                // 每次从ini中读取多少字节
                StringBuilder temp = new StringBuilder(255);

                // section=配置节，key=键名，temp=上面，path=路径
                GetPrivateProfileString(section, key, "", temp, 255, sPath);
                return temp.ToString();
            }
        }
    }



    class fileOperation
    {
        /// <summary>
        /// 复制文件夹内所有文件+文件夹到指定路径
        /// </summary>
        /// <param name="srcdir">源路径</param>
        /// <param name="desdir">目标路径</param>
        public void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1); //获取源路径最后的那个文件名or文件夹名

            string desfolderdir = desdir + "\\" + folderName; //目标文件or文件夹的完整路径

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))    //前面是目标路径的最后一个文件夹路径，后面是目标文件夹长度?
            {
                desfolderdir = desdir + folderName; //目标文件路径 = 目标文件路径+文件名 （判断是否是子文件夹）
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);    //将源路径下的所有元素加入到数组中

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }

                    File.Copy(file, srcfileName);
                }
            }
        }

    }
}
