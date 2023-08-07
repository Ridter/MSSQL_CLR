using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Principal;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class basefun
{
	public static void CombineFile(string[] infileName, string outfileName)
	{
		int num = infileName.Length;
		FileStream[] array = new FileStream[num];
		using FileStream fileStream = new FileStream(outfileName, FileMode.Create);
		for (int i = 0; i < num; i++)
		{
			try
			{
				array[i] = new FileStream(infileName[i], FileMode.Open);
				int num2;
				while ((num2 = array[i].ReadByte()) != -1)
				{
					fileStream.WriteByte((byte)num2);
				}
			}
			catch (Exception ex)
			{
				SqlContext.Pipe.Send("[X] " + ex.Message);
			}
			finally
			{
				array[i].Close();
			}
			File.Delete(infileName[i]);
		}
	}

	public static void run(string remoteFile)
	{
		try
		{
			FileInfo fileInfo = new FileInfo(remoteFile);
			SqlContext.Pipe.Send("[+] remoteFile: " + remoteFile);
			string text = remoteFile.Replace(Path.GetFileName(remoteFile), "");
			string text2 = Path.GetFileName(remoteFile) + "_*.config_txt";
			string text3 = text + text2;
			string[] files = Directory.GetFiles(text, text2);
			int num = files.Length;
			SqlContext.Pipe.Send("[+] count: " + num);
			SqlContext.Pipe.Send("[+] combinefile: " + text3 + " " + remoteFile);
			CombineFile(files, remoteFile);
			if (fileInfo.Exists)
			{
				SqlContext.Pipe.Send($"[*] '{text3}' CombineFile completed");
			}
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send("[X] " + ex.Message);
		}
	}

	public static void setAttributesNormal(DirectoryInfo dir)
	{
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo attributesNormal in directories)
		{
			setAttributesNormal(attributesNormal);
		}
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			fileInfo.Attributes = FileAttributes.Normal;
		}
	}

	public static void DeleteFile(string filename)
	{
		string fullPath = Path.GetFullPath(filename);
		if (Directory.Exists(fullPath))
		{
			try
			{
				DirectoryInfo attributesNormal = new DirectoryInfo(fullPath);
				setAttributesNormal(attributesNormal);
				Directory.Delete(fullPath, recursive: true);
				SqlContext.Pipe.Send("[*] Removed all child items and deleted directory: " + fullPath);
				return;
			}
			catch (UnauthorizedAccessException)
			{
				SqlContext.Pipe.Send("[!] Error: access denied - could not delete directory: " + fullPath);
				return;
			}
			catch (IOException)
			{
				SqlContext.Pipe.Send("[!] Error: IOException - could not delete directory: " + fullPath);
				return;
			}
			catch (Exception ex3)
			{
				SqlContext.Pipe.Send("[!] Error: Unexpected exception deleting directory: " + fullPath);
				SqlContext.Pipe.Send(ex3.ToString());
				return;
			}
		}
		if (File.Exists(fullPath))
		{
			try
			{
				File.SetAttributes(fullPath, FileAttributes.Normal);
				File.Delete(fullPath);
				SqlContext.Pipe.Send("[*] Deleted file: " + fullPath);
				return;
			}
			catch (UnauthorizedAccessException)
			{
				SqlContext.Pipe.Send("[!] Error: access denied - could not delete file: " + fullPath);
				return;
			}
			catch (IOException)
			{
				SqlContext.Pipe.Send("[!] Error: IOException - could not delete file: " + fullPath);
				return;
			}
			catch (Exception ex6)
			{
				SqlContext.Pipe.Send("[!] Error: Unexpected exception deleting file: " + fullPath);
				SqlContext.Pipe.Send(ex6.ToString());
				return;
			}
		}
		SqlContext.Pipe.Send("[!] Error: file or directory does not exist: " + fullPath);
	}

	public static void GetCurrentDir()
	{
		SqlContext.Pipe.Send($"\r\n[+] GetCurrentDir: \r\n\t{Environment.CurrentDirectory}\r\n\r\n");
	}

	public static void SetCurrentDir(string dir)
	{
		Directory.SetCurrentDirectory(dir);
		SqlContext.Pipe.Send($"\r\n[+] SetCurrentDir: {dir}\r\n\r\n");
	}

	public static void Echo(string res)
	{
		int i;
		for (i = 0; 4000 <= res.Length - i; i += 4000)
		{
			SqlContext.Pipe.Send(res.Substring(i, 4000));
		}
		SqlContext.Pipe.Send(res.Substring(i, res.Length - i));
	}

	public static void GetContent(string filename)
	{
		SqlContext.Pipe.Send("\r\n");
		try
		{
			string res = File.ReadAllText(filename);
			Echo(res);
		}
		catch (FileNotFoundException)
		{
			SqlContext.Pipe.Send("[!] Error: file not found: " + filename);
		}
		catch (SecurityException)
		{
			SqlContext.Pipe.Send("[!] Error: no permissions to read file: " + filename);
		}
		catch (IOException)
		{
			SqlContext.Pipe.Send("[!] Error: file could not be read: " + filename);
		}
		catch (Exception ex4)
		{
			SqlContext.Pipe.Send("[!] Error: Unexpected error reading file: " + filename);
			SqlContext.Pipe.Send(ex4.ToString());
		}
		SqlContext.Pipe.Send("\r\n\r\n");
	}

	public static void ListProcess()
	{
		SqlContext.Pipe.Send($"\r\n[+] ListProcess\r\n");
		Process[] processes = Process.GetProcesses();
		SqlContext.Pipe.Send(string.Format("{0,-10} {1,-1}", "ProcessId", "ProcessName"));
		Process[] array = processes;
		foreach (Process process in array)
		{
			SqlContext.Pipe.Send($"{process.Id,-10} {process.ProcessName,-1}");
		}
		SqlContext.Pipe.Send("\r\n\r\n");
	}

	public static string[] ConcatStringArray(string[] Array1, string[] Array2)
	{
		List<string> list = new List<string>();
		list.AddRange(Array1);
		list.AddRange(Array2);
		return list.ToArray();
	}

	public static bool isArray(string[] stringArray, string stringToCheck)
	{
		int num = 0;
		if (num < stringArray.Length)
		{
			string text = stringArray[num];
			if (text.Contains(stringToCheck))
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public static void ListDir(string lsdir)
	{
		string text = Directory.GetCurrentDirectory();
		string[] array = null;
		string[] array2 = null;
		long num = 0L;
		int num2 = 4;
		int num3 = 9;
		if (lsdir != "")
		{
			text = lsdir;
		}
		try
		{
			array = Directory.GetFiles(text);
			array2 = Directory.GetDirectories(text);
			SqlContext.Pipe.Send("\n  Directory listing of " + text + "\n");
		}
		catch (DirectoryNotFoundException)
		{
			SqlContext.Pipe.Send("[!] Error: directory does not exist: " + text);
			return;
		}
		catch (UnauthorizedAccessException)
		{
			SqlContext.Pipe.Send("[!] Error: no permissions to read directory: " + text);
			return;
		}
		catch (Exception ex3)
		{
			SqlContext.Pipe.Send("[!] Error: unhandled exception listing directory: " + text);
			SqlContext.Pipe.Send(ex3.ToString());
			return;
		}
		string[] array3 = ConcatStringArray(array, array2);
		Array.Sort(array3);
		if (array3 == null)
		{
			SqlContext.Pipe.Send("[*] The directory " + text + " is empty!");
			return;
		}
		string[] array4 = array;
		foreach (string fileName in array4)
		{
			long length = new FileInfo(fileName).Length;
			if (length > num)
			{
				num = length;
			}
			if (num2 < num.ToString().Length)
			{
				num2 = num.ToString().Length;
			}
		}
		string[] array5 = array3;
		foreach (string path in array5)
		{
			try
			{
				if (File.GetAccessControl(path).GetOwner(typeof(NTAccount)).ToString()
					.Length > num3)
				{
					num3 = File.GetAccessControl(path).GetOwner(typeof(NTAccount)).ToString()
						.Length;
				}
			}
			catch
			{
			}
		}
		SqlContext.Pipe.Send("Last Modify      Type     Owner" + new string(' ', num3 - 5) + "   Size" + new string(' ', num2 - 4) + "   File/Dir Name");
		SqlContext.Pipe.Send("==============   ======   " + new string('=', num3) + "   " + new string('=', num2) + "   =============");
		string[] array6 = array3;
		foreach (string text2 in array6)
		{
			string fileName2 = Path.GetFileName(text2);
			DateTime lastWriteTime = File.GetLastWriteTime(text2);
			string text3 = $"{lastWriteTime:MM/dd/yy HH:mm}";
			string text4;
			try
			{
				text4 = File.GetAccessControl(text2).GetOwner(typeof(NTAccount)).ToString();
			}
			catch
			{
				text4 = "<Unknown>";
			}
			if (isArray(array, text2))
			{
				long length2 = new FileInfo(text2).Length;
				SqlContext.Pipe.Send(text3 + "   <File>   " + text4 + new string(' ', num3 - text4.ToString().Length) + "   " + length2 + new string(' ', num2 - length2.ToString().Length) + "   " + fileName2);
			}
			else
			{
				SqlContext.Pipe.Send(text3 + "   <Dir>    " + text4 + new string(' ', num3 - text4.ToString().Length) + "   " + new string('.', num2) + "   " + fileName2);
			}
		}
	}

	public static bool PingHost(string nameOrAddress)
	{
		bool result = false;
		Ping ping = null;
		try
		{
			ping = new Ping();
			PingReply pingReply = ping.Send(nameOrAddress);
			result = pingReply.Status == IPStatus.Success;
		}
		catch (PingException)
		{
		}
		finally
		{
			ping?.Dispose();
		}
		return result;
	}

	public static void ping(string nameOrAddress)
	{
		if (PingHost(nameOrAddress))
		{
			SqlContext.Pipe.Send("[*] Host is reachable: " + nameOrAddress);
		}
		else
		{
			SqlContext.Pipe.Send("[!] Host is unreachable: " + nameOrAddress);
		}
	}

	public static void netstat()
	{
		IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
		SqlContext.Pipe.Send("Local Address          Remote Address         State");
		SqlContext.Pipe.Send("=============          ==============         =====");
		IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
		foreach (IPEndPoint iPEndPoint in activeTcpListeners)
		{
			SqlContext.Pipe.Send(string.Concat(iPEndPoint.Address, ":", iPEndPoint.Port, new string(' ', 22 - (iPEndPoint.Address.ToString().Length + iPEndPoint.Port.ToString().Length)), "0.0.0.0", new string(' ', 16), "LISTENING"));
		}
		TcpConnectionInformation[] activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
		foreach (TcpConnectionInformation tcpConnectionInformation in activeTcpConnections)
		{
			SqlContext.Pipe.Send(string.Concat(tcpConnectionInformation.LocalEndPoint, new string(' ', 23 - tcpConnectionInformation.LocalEndPoint.ToString().Length), tcpConnectionInformation.RemoteEndPoint, new string(' ', 23 - tcpConnectionInformation.RemoteEndPoint.ToString().Length), "ESTABLISHED"));
		}
	}
}
