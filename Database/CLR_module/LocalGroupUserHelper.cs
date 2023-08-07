using System.Runtime.InteropServices;
using Microsoft.SqlServer.Server;

namespace CLR_module;

public class LocalGroupUserHelper
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct LOCALGROUP_MEMBERS_INFO_3
	{
		public string domainandname;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct USER_INFO_1
	{
		public string usri1_name;

		public string usri1_password;

		public int usri1_password_age;

		public int usri1_priv;

		public string usri1_home_dir;

		public string comment;

		public int usri1_flags;

		public string usri1_script_path;
	}

	[DllImport("Netapi32.dll")]
	private static extern int NetUserAdd([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref USER_INFO_1 buf, int parm_err);

	[DllImport("Netapi32.dll")]
	private static extern int NetLocalGroupAddMembers([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, ref LOCALGROUP_MEMBERS_INFO_3 buf, int totalentries);

	public void AddUser(string serverName, string userName, string password, string strComment)
	{
		USER_INFO_1 buf = default(USER_INFO_1);
		buf.usri1_name = userName;
		buf.usri1_password = password;
		buf.usri1_priv = 1;
		buf.usri1_home_dir = null;
		buf.comment = strComment;
		buf.usri1_script_path = null;
		if (NetUserAdd(serverName, 1, ref buf, 0) != 0)
		{
			SqlContext.Pipe.Send("[X] Error Adding User");
		}
		else
		{
			SqlContext.Pipe.Send("[*] Adding User success");
		}
	}

	public void GroupAddMembers(string serverName, string groupName, string userName)
	{
		LOCALGROUP_MEMBERS_INFO_3 buf = default(LOCALGROUP_MEMBERS_INFO_3);
		buf.domainandname = userName;
		if (NetLocalGroupAddMembers(serverName, groupName, 3, ref buf, 1) != 0)
		{
			SqlContext.Pipe.Send("[X] Error Adding Group Member");
		}
		else
		{
			SqlContext.Pipe.Send("[*] Adding Group Member success");
		}
	}
}
