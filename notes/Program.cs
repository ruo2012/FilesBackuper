using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notes
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "a\\b\\c\\d\\e";
            //Console.WriteLine(s.Substring(s.LastIndexOf("\\") - 1));
            //Console.WriteLine(s.Length - 1);
            //int i = s.LastIndexOf("\\");
            //Console.WriteLine(i);
            //if(s.LastIndexOf("\\") == s.Length - 1)
            //{
            //    Console.WriteLine("123");
            //}
            //Console.ReadKey();
            Console.WriteLine(s.Substring(s.Length));
            Console.WriteLine(s.Length);
            Console.ReadKey();
        }
    }
}
