﻿using System;
using System.Text;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Data.SQLite;

namespace FilesBackuper
{
    /// <summary>
    /// 自定义类
    /// </summary>
    class Library
    {
        /// <summary>
        /// INI配置文件的读取和写入
        /// </summary>
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


    /// <summary>
    /// 文件复制类
    /// </summary>
    class fileOperation
    {
        /// <summary>
        /// 全量复制并记录至access数据库
        /// </summary>
        /// <param name="srcdir">源路径</param>
        /// <param name="desdir">目标路径</param>
        public void CopyDirectory(string srcdir, string desdir)
        {
            string exePath = Environment.CurrentDirectory;//本程序所在路径
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1); //获取源路径最后的那个文件名or文件夹名
            string desfolderdir = desdir + "\\" + folderName; //目标文件or文件夹的完整路径
            SQLiteConnection conn;
            conn = new SQLiteConnection("Data Source=" + exePath + @"\FilesDetails.db" + "; Version=3;");
            conn.Open();
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
                    string orignalPath = srcdir + file.Substring(file.LastIndexOf("\\"));   //最原始的文件路径
                    FileInfo fl = new FileInfo(orignalPath);
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    DateTime writetime = fl.LastWriteTime.ToLocalTime();
                    int timestamp = (int)(writetime - startTime).TotalSeconds; //原始文件的修改时间（时间戳）    
                    SQLiteCommand command = new SQLiteCommand("select * from Lists where FileName=" + "'" + orignalPath + "'", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable(); //新建表对象
                    dt.Load(reader);
                    if (dt.Rows.Count == 0) //判断数据库中是否有记录到这个文件路径
                    {
                        SQLiteCommand comm = new SQLiteCommand("insert into Lists(FileName,ChangeTime)values('" + orignalPath + "','" + timestamp + "')", conn);
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLiteCommand comm = new SQLiteCommand("update Lists set ChangeTime=" + "'" + timestamp + "'" + " where FileName=" + "'" + orignalPath + "'", conn);
                        comm.ExecuteNonQuery();
                    }
                }
            }
            conn.Close();
        }

        /// <summary>
        /// 差异复制并记录到access数据库
        /// </summary>
        /// <param name="srcdir">源路径</param>
        /// <param name="desdir">目标途径</param>
        public void CopyDirectoryDif(string srcdir, string desdir)
        {
            string exePath = Environment.CurrentDirectory;//本程序所在路径
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1); //获取源路径最后的那个文件名or文件夹名
            string desfolderdir = desdir + "\\" + folderName; //目标文件or文件夹的完整路径
            SQLiteConnection conn;
            conn = new SQLiteConnection("Data Source=" + exePath + @"\FilesDetails.db" + "; Version=3;");
            conn.Open();
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
                    CopyDirectoryDif(file, desfolderdir);
                }
                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);
                    srcfileName = desfolderdir + "\\" + srcfileName;
                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }
                    string orignalPath = srcdir + file.Substring(file.LastIndexOf("\\"));   //最原始的文件路径
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    FileInfo fl = new FileInfo(orignalPath);
                    DateTime realchangeTime = fl.LastWriteTime.ToLocalTime();
                    int realchangeTimeStamps = (int)(realchangeTime - startTime).TotalSeconds;
                    SQLiteCommand command = new SQLiteCommand("select * from Lists where FileName=" + "'" + orignalPath + "'", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable(); //新建表对象
                    dt.Load(reader);
                    if (dt.Rows.Count != 0) //SQL中有数据
                    {
                        int sqlchangTimeStamps = Convert.ToInt32(dt.Rows[0]["ChangeTime"]);  //数据库内的修改时间
                        if (realchangeTimeStamps > sqlchangTimeStamps)  //文件有修改
                        {
                            File.Copy(file, srcfileName);
                            SQLiteCommand comm = new SQLiteCommand("update Lists set ChangeTime=" + "'" + realchangeTimeStamps + "'" + " where FileName=" + "'" + orignalPath + "'", conn);
                            comm.ExecuteNonQuery();
                        }
                    }
                    else //SQL中没数据
                    {
                        File.Copy(file, srcfileName);
                        SQLiteCommand comm = new SQLiteCommand("insert into Lists(FileName,ChangeTime)values('" + orignalPath + "','" + realchangeTimeStamps + "')", conn);
                        comm.ExecuteNonQuery();
                    }
                }
            }
            conn.Close();
        }

        /// <summary>
        /// Access数据库操作类
        /// </summary>
        class AccessDB
        {
            static string exePath = Environment.CurrentDirectory;//本程序所在路径
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + exePath + @"\FilesDetails.accdb");

            /// <summary>
            /// Access数据库查询
            /// </summary>
            /// <param name="sql">SQL语句</param>
            public DataTable AccdbQuery(string sql)
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn); //创建适配对象
                DataTable dt = new DataTable(); //新建表对象
                da.Fill(dt); //用适配对象填充表对象
                conn.Close();
                return dt;
            }

            /// <summary>
            /// Access数据库增删改
            /// </summary>
            /// <param name="sql">SQL语句</param>
            public void AccdbChange(string sql)
            {
                conn.Open();
                OleDbCommand comm = new OleDbCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
