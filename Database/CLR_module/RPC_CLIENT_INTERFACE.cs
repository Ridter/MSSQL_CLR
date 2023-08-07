using System;
using System.Runtime.InteropServices;

namespace CLR_module;

internal struct RPC_CLIENT_INTERFACE
{
	public uint Length;

	public RPC_SYNTAX_IDENTIFIER InterfaceId;

	public RPC_SYNTAX_IDENTIFIER TransferSyntax;

	public IntPtr DispatchTable;

	public uint RpcProtseqEndpointCount;

	public IntPtr RpcProtseqEndpoint;

	public IntPtr Reserved;

	public IntPtr InterpreterInfo;

	public uint Flags;

	public static Guid IID_SYNTAX = new Guid(2324192516u, 7403, 4553, 159, 232, 8, 0, 43, 16, 72, 96);

	public RPC_CLIENT_INTERFACE(Guid iid, ushort InterfaceVersionMajor, ushort InterfaceVersionMinor)
	{
		Length = (uint)Marshal.SizeOf(typeof(RPC_CLIENT_INTERFACE));
		RPC_VERSION syntaxVersion = new RPC_VERSION(InterfaceVersionMajor, InterfaceVersionMinor);
		InterfaceId = default(RPC_SYNTAX_IDENTIFIER);
		InterfaceId.SyntaxGUID = iid;
		InterfaceId.SyntaxVersion = syntaxVersion;
		syntaxVersion = new RPC_VERSION(2, 0);
		TransferSyntax = default(RPC_SYNTAX_IDENTIFIER);
		TransferSyntax.SyntaxGUID = IID_SYNTAX;
		TransferSyntax.SyntaxVersion = syntaxVersion;
		DispatchTable = IntPtr.Zero;
		RpcProtseqEndpointCount = 0u;
		RpcProtseqEndpoint = IntPtr.Zero;
		Reserved = IntPtr.Zero;
		InterpreterInfo = IntPtr.Zero;
		Flags = 0u;
	}
}
