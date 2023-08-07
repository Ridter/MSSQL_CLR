using System;
using System.Diagnostics;
using System.Text;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class exec
{
	public static void run(string cmd)
	{
		RunCommand("cmd.exe", " /c " + cmd);
	}

	public static void run1(string proc, string arg)
	{
		RunCommand(proc, arg);
	}

	public static string RunCommand(string filename, string arguments)
	{
        SqlContext.Pipe.Send("[+] Process: " + filename);
		SqlContext.Pipe.Send("[+] arguments: " + arguments);
		SqlContext.Pipe.Send("[+] RunCommand: " + filename + " " + arguments);
		Process process = new Process();
		SqlContext.Pipe.Send("\n");
		process.StartInfo.FileName = filename;
		if (!string.IsNullOrEmpty(arguments))
		{
			process.StartInfo.Arguments = arguments;
		}
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.RedirectStandardOutput = true;
		StringBuilder stdOutput = new StringBuilder();
		process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
		{
			stdOutput.AppendLine(args.Data);
		};
		string value = null;
		try
		{
			process.Start();
			process.BeginOutputReadLine();
			value = process.StandardError.ReadToEnd();
			process.WaitForExit();
		}
		catch (Exception ex)
		{
			SqlContext.Pipe.Send(ex.Message);
		}
		if (process.ExitCode == 0)
		{
			SqlContext.Pipe.Send(stdOutput.ToString());
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.AppendLine(value);
			}
			if (stdOutput.Length != 0)
			{
				stringBuilder.AppendLine("Std output:");
				stringBuilder.AppendLine(stdOutput.ToString());
			}
			SqlContext.Pipe.Send("[X] " + filename + arguments + " finished with exit code = " + process.ExitCode + ": " + stringBuilder);
		}
		return stdOutput.ToString();
	}
}
