using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PD.Register.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Blue;

            System.Console.WriteLine("请输入系统运行指令:(-r 注册 -un 卸载)");

            while (true)
            {
                string path = @"C:\PDSystem.db";

                System.Console.ForegroundColor = ConsoleColor.White;

                string instruct = System.Console.ReadLine();

                if (instruct.Equals("-r"))
                {
                    if (File.Exists(path))
                        File.Delete(path);

                    System.Console.ForegroundColor = ConsoleColor.Magenta;

                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        StreamWriter strwriter = new StreamWriter(fs);
                        try
                        {
                            strwriter.Write("register");
                            strwriter.Close();
                            System.Console.WriteLine("运行指令执行正确");
                            System.Console.WriteLine("正在重启IIS");
                            Process.Start("iisreset");
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("运行指令执行错误:" + ex.Message);
                        }
                        finally
                        {
                            strwriter.Close();
                            strwriter = null;
                        }
                    }
                }
                else if (instruct.Equals("-un"))
                {
                    if (File.Exists(path))
                        File.Delete(path);
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine("卸载指令执行正确");
                    System.Console.WriteLine("正在重启IIS");
                    Process.Start("iisreset");
                }
                else if (instruct.Equals("exit"))
                {
                    break;
                }
                else
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("命令错误");
                }
            }
        }
    }
}
