using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace CLR_module;

internal class ProcessWaitHandle : WaitHandle
{
	internal ProcessWaitHandle(SafeWaitHandle processHandle)
	{
		base.SafeWaitHandle = processHandle;
	}
}
