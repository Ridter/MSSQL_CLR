namespace CLR_module;

internal struct RPC_VERSION
{
	public ushort MajorVersion;

	public ushort MinorVersion;

	public RPC_VERSION(ushort InterfaceVersionMajor, ushort InterfaceVersionMinor)
	{
		MajorVersion = InterfaceVersionMajor;
		MinorVersion = InterfaceVersionMinor;
	}
}
