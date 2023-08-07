using System;

namespace CLR_module;

internal struct MIDL_STUB_DESC
{
	public IntPtr RpcInterfaceInformation;

	public IntPtr pfnAllocate;

	public IntPtr pfnFree;

	public IntPtr pAutoBindHandle;

	public IntPtr apfnNdrRundownRoutines;

	public IntPtr aGenericBindingRoutinePairs;

	public IntPtr apfnExprEval;

	public IntPtr aXmitQuintuple;

	public IntPtr pFormatTypes;

	public int fCheckBounds;

	public uint Version;

	public IntPtr pMallocFreeStruct;

	public int MIDLVersion;

	public IntPtr CommFaultOffsets;

	public IntPtr aUserMarshalQuadruple;

	public IntPtr NotifyRoutineTable;

	public IntPtr mFlags;

	public IntPtr CsRoutineTables;

	public IntPtr ProxyServerInfo;

	public IntPtr pExprInfo;

	public MIDL_STUB_DESC(IntPtr pFormatTypesPtr, IntPtr RpcInterfaceInformationPtr, IntPtr pfnAllocatePtr, IntPtr pfnFreePtr)
	{
		pFormatTypes = pFormatTypesPtr;
		RpcInterfaceInformation = RpcInterfaceInformationPtr;
		CommFaultOffsets = IntPtr.Zero;
		pfnAllocate = pfnAllocatePtr;
		pfnFree = pfnFreePtr;
		pAutoBindHandle = IntPtr.Zero;
		apfnNdrRundownRoutines = IntPtr.Zero;
		aGenericBindingRoutinePairs = IntPtr.Zero;
		apfnExprEval = IntPtr.Zero;
		aXmitQuintuple = IntPtr.Zero;
		fCheckBounds = 1;
		Version = 327682u;
		pMallocFreeStruct = IntPtr.Zero;
		MIDLVersion = 134283886;
		aUserMarshalQuadruple = IntPtr.Zero;
		NotifyRoutineTable = IntPtr.Zero;
		mFlags = new IntPtr(1);
		CsRoutineTables = IntPtr.Zero;
		ProxyServerInfo = IntPtr.Zero;
		pExprInfo = IntPtr.Zero;
	}
}
