using System;
using System.IO;
using System.Net;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class download
{
	public static bool DownloadFile(string URL, string filename)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			Stream stream = new FileStream(filename, FileMode.Create);
			byte[] array = new byte[1024];
			for (int num = responseStream.Read(array, 0, array.Length); num > 0; num = responseStream.Read(array, 0, array.Length))
			{
				stream.Write(array, 0, num);
			}
			stream.Close();
			responseStream.Close();
			httpWebResponse.Close();
			httpWebRequest.Abort();
			return true;
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send("[X] ERROR Log:" + ex.ToString());
			return false;
		}
	}

	public static void run(string url, string localpath)
	{
		if (DownloadFile(url, localpath))
		{
			SqlContext.Pipe.Send("[*] Download success");
		}
		else
		{
			SqlContext.Pipe.Send("[X] Download fail");
		}
	}
}
