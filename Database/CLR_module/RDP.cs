using System.Diagnostics;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;

namespace CLR_module;

internal class RDP
{
	public static void run()
	{
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Terminal Server");
		string text = registryKey.GetValue("fDenyTSConnections").ToString();
		RegistryKey registryKey2 = localMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Terminal Server\\WinStations\\RDP-Tcp");
		string arg = registryKey2.GetValue("PortNumber").ToString();
		registryKey2.Close();
		if (text.Contains("0"))
		{
			SqlContext.Pipe.Send("[*] RDP is already enabled");
			SqlContext.Pipe.Send($"[+] RDP Port: {arg}");
			return;
		}
		SqlContext.Pipe.Send("[*] RDP is disabled, enabling it ...");
		RegistryKey registryKey3 = localMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\Terminal Server");
		registryKey3.SetValue("fDenyTSConnections", "0", RegistryValueKind.DWord);
		registryKey3.Close();
		Process process = new Process();
		process.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		process.StandardInput.WriteLine("sc config termservice start= auto");
		process.StandardInput.WriteLine("netsh firewall set service remotedesktop enable");
		process.StandardInput.WriteLine("exit");
		process.WaitForExit();
		process.Close();
		process.Dispose();
		SqlContext.Pipe.Send($"[+] RDP Port: {arg}");
	}
}
