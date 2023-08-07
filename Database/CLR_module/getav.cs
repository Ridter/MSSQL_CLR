using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace CLR_module;

internal class getav
{
	public static void run()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string item in avList())
		{
			string[] array = Regex.Split(item, ":", RegexOptions.IgnoreCase);
			string key = array[0].ToString();
			string value = array[1].ToString();
			dictionary.Add(key, value);
		}
		SqlContext.Pipe.Send("[*] Finding....");
		string value2 = "";
		int num = 0;
		Process[] processes = Process.GetProcesses();
		Process[] array2 = processes;
		foreach (Process process in array2)
		{
			if (dictionary.TryGetValue(process.ProcessName, out value2))
			{
				SqlContext.Pipe.Send($"   [>] proName: {process.ProcessName} appName: {value2}");
				num++;
			}
		}
		if (num == 0)
		{
			SqlContext.Pipe.Send("[!] No anti-virus software on this machine");
		}
		SqlContext.Pipe.Send("[*] Finish!");
		GC.Collect();
	}

	public static List<string> avList()
	{
		List<string> list = new List<string>();
		list.AddRange(new string[557]
		{
			"360tray:360安全卫士-实时保护", "360safe:360安全卫士-主程序", "ZhuDongFangYu:360安全卫士-主动防御", "360sd:360杀毒", "a2guard:a-squared杀毒", "ad-watch:Lavasoft杀毒", "cleaner8:The Cleaner杀毒", "vba32lder:vb32杀毒", "MongoosaGUI:Mongoosa杀毒", "CorantiControlCenter32:Coranti2012杀毒",
			"F-PROT:F-Prot AntiVirus", "CMCTrayIcon:CMC杀毒", "K7TSecurity:K7杀毒", "UnThreat:UnThreat杀毒", "CKSoftShiedAntivirus4:Shield Antivirus杀毒", "AVWatchService:VIRUSfighter杀毒", "ArcaTasksService:ArcaVir杀毒", "iptray:Immunet杀毒", "PSafeSysTray:PSafe杀毒", "nspupsvc:nProtect杀毒",
			"SpywareTerminatorShield:SpywareTerminator反间谍软件", "BKavService:Bkav杀毒", "MsMpEng:Windows Defender", "SBAMSvc:VIPRE", "ccSvcHst:Norton杀毒", "f-secure:冰岛", "avp:Kaspersky", "KvMonXP:江民杀毒", "RavMonD:瑞星杀毒", "Mcshield:McAfee",
			"Tbmon:McAfee", "Frameworkservice:McAfee", "egui:ESET NOD32", "ekrn:ESET NOD32", "eguiProxy:ESET NOD32", "kxetray:金山毒霸", "knsdtray:可牛杀毒", "TMBMSRV:趋势杀毒", "avcenter:Avira(小红伞)", "avguard:Avira(小红伞)",
			"avgnt:Avira(小红伞)", "sched:Avira(小红伞)", "ashDisp:Avast网络安全", "rtvscan:诺顿杀毒", "ccapp:SymantecNorton", "NPFMntor:Norton杀毒软件", "ccSetMgr:赛门铁克", "ccRegVfy:Norton杀毒软件", "ksafe:金山卫士", "QQPCRTP:QQ电脑管家",
			"avgwdsvc:AVG杀毒", "QUHLPSVC:QUICK HEAL杀毒", "mssecess:微软杀毒", "SavProgress:Sophos杀毒", "SophosUI:Sophos杀毒", "SophosFS:Sophos杀毒", "SophosHealth:Sophos杀毒", "SophosSafestore64:Sophos杀毒", "SophosCleanM:Sophos杀毒", "fsavgui:F-Secure杀毒",
			"vsserv:比特梵德", "remupd:熊猫卫士", "FortiTray:飞塔", "safedog:安全狗", "parmor:木马克星", "Iparmor.exe:木马克星", "beikesan:贝壳云安全", "KSWebShield:金山网盾", "TrojanHunter:木马猎手", "GG:巨盾网游安全盾",
			"adam:绿鹰安全精灵", "AST:超级巡警", "ananwidget:墨者安全专家", "AVK:AntiVirusKit", "avg:AVG Anti-Virus", "spidernt:Dr.web", "avgaurd:Avira Antivir", "vsmon:Zone Alarm", "cpf:Comodo", "outpost:Outpost Firewall",
			"rfwmain:瑞星防火墙", "kpfwtray:金山网镖", "FYFireWall:风云防火墙", "MPMon:微点主动防御", "pfw:天网防火墙", "BaiduSdSvc:百度杀毒-服务进程", "BaiduSdTray:百度杀毒-托盘进程", "BaiduSd:百度杀毒-主程序", "SafeDogGuardCenter:安全狗", "safedogupdatecenter:安全狗",
			"safedogguardcenter:安全狗", "SafeDogSiteIIS:安全狗", "SafeDogTray:安全狗", "SafeDogServerUI:安全狗", "D_Safe_Manage:D盾", "d_manage:D盾", "yunsuo_agent_service:云锁", "yunsuo_agent_daemon:云锁", "HwsPanel:护卫神", "hws_ui:护卫神",
			"hws:护卫神", "hwsd:护卫神", "HipsTray:火绒", "HipsDaemon:火绒", "wsctrl:火绒", "usysdiag:火绒", "SPHINX:SPHINX防火墙", "bddownloader:百度卫士", "baiduansvx:百度卫士-主进程", "AvastUI:Avast!5主程序",
			"emet_agent:EMET", "emet_service:EMET", "firesvc:McAfee", "firetray:McAfee", "hipsvc:McAfee", "mfevtps:McAfee", "mcafeefire:McAfee", "scan32:McAfee", "shstat:McAfee", "vstskmgr:McAfee",
			"engineserver:McAfee", "mfeann:McAfee", "mcscript:McAfee", "updaterui:McAfee", "udaterui:McAfee", "naprdmgr:McAfee", "cleanup:McAfee", "cmdagent:McAfee", "frminst:McAfee", "mcscript_inuse:McAfee",
			"mctray:McAfee", "_avp32:卡巴斯基", "_avpcc:卡巴斯基", "_avpm:卡巴斯基", "aAvgApi:AVG", "ackwin32:已知杀软进程,名称暂未收录", "alertsvc:Norton AntiVirus", "alogserv:McAfee VirusScan", "anti-trojan:Anti-Trojan Elite", "arr:Application Request Route",
			"atguard:AntiVir", "atupdater:已知杀软进程,名称暂未收录", "atwatch:Mustek", "au:NSIS", "aupdate:Symantec", "auto-protect.nav80try:已知杀软进程,名称暂未收录", "autodown:AntiVirus AutoUpdater", "avconsol:McAfee", "avgcc32:AVG", "avgctrl:AVG",
			"avgemc:AVG", "avgrsx:AVG", "avgserv:AVG", "avgserv9:AVG", "avgw:AVG", "avkpop:G DATA SOFTWARE AG", "avkserv:G DATA SOFTWARE AG", "avkservice:G DATA SOFTWARE AG", "avkwctl9:G DATA SOFTWARE AG", "avltmain:Panda Software Aplication",
			"avnt:H+BEDV Datentechnik GmbH", "avp32:Kaspersky Anti-Virus", "avpcc: Kaspersky AntiVirus", "avpdos32: Kaspersky AntiVirus", "avpm: Kaspersky AntiVirus", "avptc32: Kaspersky AntiVirus", "avpupd: Kaspersky AntiVirus", "avsynmgr:McAfee", "avwin: H+BEDV", "bargains:Exact Advertising SpyWare",
			"beagle:Avast", "blackd:BlackICE", "blackice:BlackICE", "blink:micromedia", "blss:CBlaster", "bootwarn:Symantec", "bpc:Grokster", "brasil:Exact Advertising", "ccevtmgr:Norton Internet Security", "cdp:CyberLink Corp.",
			"cfd:Motive Communications", "cfgwiz: Norton AntiVirus", "claw95:已知杀软进程,名称暂未收录", "claw95cf:已知杀软进程,名称暂未收录", "clean:windows流氓软件清理大师", "cleaner:windows流氓软件清理大师", "cleaner3:windows流氓软件清理大师", "cleanpc:windows流氓软件清理大师", "cpd:McAfee", "ctrl:已知杀软进程,名称暂未收录",
			"cv:已知杀软进程,名称暂未收录", "defalert:Symantec", "defscangui:Symantec", "defwatch:Norton Antivirus", "doors:已知杀软进程,名称暂未收录", "dpf:已知杀软进程,名称暂未收录", "dpps2:PanicWare", "dssagent:Broderbund", "ecengine:已知杀软进程,名称暂未收录", "emsw:Alset Inc",
			"ent:已知杀软进程,名称暂未收录", "espwatch:已知杀软进程,名称暂未收录", "ethereal:RationalClearCase", "exe.avxw:已知杀软进程,名称暂未收录", "expert:已知杀软进程,名称暂未收录", "f-prot95:已知杀软进程,名称暂未收录", "fameh32:F-Secure", "fast: FastUsr", "fch32:F-Secure", "fih32:F-Secure",
			"findviru:F-Secure", "firewall:AshampooSoftware", "fnrb32:F-Secure", "fp-win: F-Prot Antivirus OnDemand", "fsaa:F-Secure", "fsav:F-Secure", "fsav32:F-Secure", "fsav530stbyb:F-Secure", "fsav530wtbyb:F-Secure", "fsav95:F-Secure",
			"fsgk32:F-Secure", "fsm32:F-Secure", "fsma32:F-Secure", "fsmb32:F-Secure", "gbmenu:已知杀软进程,名称暂未收录", "guard:ewido", "guarddog:ewido", "htlog:已知杀软进程,名称暂未收录", "htpatch:Silicon Integrated Systems Corporation", "hwpe:已知杀软进程,名称暂未收录",
			"iamapp:Symantec", "iamserv:Symantec", "iamstats:Symantec", "iedriver: Urlblaze.com", "iface:Panda Antivirus Module", "infus:Infus Dialer", "infwin:Msviewparasite", "intdel:Inet Delivery", "intren:已知杀软进程,名称暂未收录", "jammer:已知杀软进程,名称暂未收录",
			"kavpf:Kapersky", "kazza:Kapersky", "keenvalue:EUNIVERSE INC", "launcher:Intercort Systems", "ldpro:已知杀软进程,名称暂未收录", "ldscan:Windows Trojans Inspector", "localnet:已知杀软进程,名称暂未收录", "luall:Symantec", "luau:Symantec", "lucomserver:Norton",
			"mcagent:McAfee", "mcmnhdlr:McAfee", "mctool:McAfee", "mcupdate:McAfee", "mcvsrte:McAfee", "mcvsshld:McAfee", "mfin32:MyFreeInternetUpdate", "mfw2en:MyFreeInternetUpdate", "mfweng3.02d30:MyFreeInternetUpdate", "mgavrtcl:McAfee",
			"mgavrte:McAfee", "mghtml:McAfee", "mgui:BullGuard", "minilog:Zone Labs Inc", "mmod:EzulaInc", "mostat:WurldMediaInc", "mpfagent:McAfee", "mpfservice:McAfee", "mpftray:McAfee", "mscache:Integrated Search Technologies Spyware",
			"mscman:OdysseusMarketingInc", "msmgt:Total Velocity Spyware", "msvxd:W32/Datom-A", "mwatch:已知杀软进程,名称暂未收录", "nav:Reuters Limited", "navapsvc:Norton AntiVirus", "navapw32:Norton AntiVirus", "navw32:Norton Antivirus", "ndd32:诺顿磁盘医生", "neowatchlog:已知杀软进程,名称暂未收录",
			"netutils:已知杀软进程,名称暂未收录", "nisserv:Norton", "nisum:Norton", "nmain:Norton", "nod32:ESET Smart Security", "norton_internet_secu_3.0_407:已知杀软进程,名称暂未收录", "notstart:已知杀软进程,名称暂未收录", "nprotect:Symantec", "npscheck:Norton", "npssvc:Norton",
			"ntrtscan:趋势反病毒应用程序", "nui:已知杀软进程,名称暂未收录", "otfix:已知杀软进程,名称暂未收录", "outpostinstall:Outpost", "patch:趋势科技", "pavw:已知杀软进程,名称暂未收录", "pcscan:趋势科技", "pdsetup:已知杀软进程,名称暂未收录", "persfw:Tiny Personal Firewall", "pgmonitr:PromulGate SpyWare",
			"pingscan:已知杀软进程,名称暂未收录", "platin:已知杀软进程,名称暂未收录", "pop3trap:PC-cillin", "poproxy:NortonAntiVirus", "popscan:已知杀软进程,名称暂未收录", "powerscan:Integrated Search Technologies", "ppinupdt:已知杀软进程,名称暂未收录", "pptbc:已知杀软进程,名称暂未收录", "ppvstop:已知杀软进程,名称暂未收录", "prizesurfer:Prizesurfer",
			"prmt:OpiStat", "prmvr:Adtomi", "processmonitor:Sysinternals", "proport:已知杀软进程,名称暂未收录", "protectx:ProtectX", "pspf:已知杀软进程,名称暂未收录", "purge:已知杀软进程,名称暂未收录", "qconsole:Norton AntiVirus Quarantine Console", "qserver:Norton Internet Security", "rapapp:BlackICE",
			"rb32:RapidBlaster", "rcsync:PrizeSurfer", "realmon:Realmon ", "rescue:已知杀软进程,名称暂未收录", "rescue32:卡巴斯基互联网安全套装", "rshell:已知杀软进程,名称暂未收录", "rtvscn95:Real-time virus scanner ", "rulaunch:McAfee User Interface", "run32dll:PAL PC Spy", "safeweb:PSafe Tecnologia",
			"sbserv:Norton Antivirus", "scrscan:360杀毒", "sfc:System file checker", "sh:MKS Toolkit for Win3", "showbehind:MicroSmarts Enterprise Component ", "soap:System Soap Pro", "sofi:已知杀软进程,名称暂未收录", "sperm:已知杀软进程,名称暂未收录", "supporter5:eScorcher反病毒", "symproxysvc:Symantec",
			"symtray:Symantec", "tbscan:ThunderBYTE", "tc:TimeCalende", "titanin:TitanHide", "tvmd:Total Velocity", "tvtmd: Total Velocity", "vettray:eTrust", "vir-help:已知杀软进程,名称暂未收录", "vnpc3000:已知杀软进程,名称暂未收录", "vpc32:Symantec",
			"vpc42:Symantec", "vshwin32:McAfee", "vsmain:McAfee", "vsstat:McAfee", "wfindv32:已知杀软进程,名称暂未收录", "zapro:Zone Alarm", "zonealarm:Zone Alarm", "AVPM:Kaspersky", "A2CMD:Emsisoft Anti-Malware", "A2SERVICE:a-squared free",
			"A2FREE:a-squared Free", "ADVCHK:Norton AntiVirus", "AGB:安天防线", "AHPROCMONSERVER:安天防线", "AIRDEFENSE:AirDefense", "ALERTSVC:Norton AntiVirus", "AVIRA:小红伞杀毒", "AMON:Tiny Personal Firewall", "AVZ:AVZ", "ANTIVIR:已知杀软进程,名称暂未收录",
			"APVXDWIN:熊猫卫士", "ASHMAISV:Alwil", "ASHSERV:Avast Anti-virus", "ASHSIMPL:AVAST!VirusCleaner", "ASHWEBSV:Avast", "ASWUPDSV:Avast", "ASWSCAN:Avast", "AVCIMAN:熊猫卫士", "AVCONSOL:McAfee", "AVENGINE:熊猫卫士",
			"AVESVC:Avira AntiVir Security Service", "AVEVL32:已知杀软进程,名称暂未收录", "AVGAM:AVG", "AVGCC:AVG", "AVGCHSVX:AVG", "AVGCSRVX:AVG", "AVGNSX:AVG", "AVGCC32:AVG", "AVGCTRL:AVG", "AVGEMC:AVG",
			"AVGFWSRV:AVG", "AVGNTMGR:AVG", "AVGSERV:AVG", "AVGTRAY:AVG", "AVGUPSVC:AVG", "AVINITNT:Command AntiVirus for NT Server", "AVPCC:Kaspersky", "AVSERVER:Kerio MailServer", "AVSCHED32:H+BEDV", "AVSYNMGR:McAfee",
			"AVWUPSRV:H+BEDV", "BDSWITCH:BitDefender Module", "BLACKD:BlackICE", "CCEVTMGR:Symantec", "CFP:COMODO", "CLAMWIN:ClamWin Portable", "CUREIT:DrWeb CureIT", "DEFWATCH:Norton Antivirus", "DRWADINS:Dr.Web", "DRWEB:Dr.Web",
			"DEFENDERDAEMON:ShadowDefender", "EWIDOCTRL:Ewido Security Suite", "EZANTIVIRUSREGISTRATIONCHECK:e-Trust Antivirus", "FIREWALL:AshampooSoftware", "FPROTTRAY:F-PROT Antivirus", "FPWIN:Verizon", "FRESHCLAM:ClamAV", "FSAV32:F-Secure", "FSBWSYS:F-secure", "FSDFWD:F-Secure",
			"FSGK32:F-Secure", "FSGK32ST:F-Secure", "FSMA32:F-Secure", "FSMB32:F-Secure", "FSSM32:F-Secure", "GUARDGUI:网游保镖", "GUARDNT:IKARUS", "IAMAPP:Symantec", "INOCIT:eTrust", "INORPC:eTrust",
			"INORT:eTrust", "INOTASK:eTrust", "INOUPTNG:eTrust", "ISAFE:eTrust", "KAV:Kaspersky", "KAVMM:Kaspersky", "KAVPF:Kaspersky", "KAVPFW:Kaspersky", "KAVSTART:Kaspersky", "KAVSVC:Kaspersky",
			"KAVSVCUI:Kaspersky", "KMAILMON:金山毒霸", "MCAGENT:McAfee", "MCMNHDLR:McAfee", "MCREGWIZ:McAfee", "MCUPDATE:McAfee", "MCVSSHLD:McAfee", "MINILOG:Zone Alarm", "MYAGTSVC:McAfee", "MYAGTTRY:McAfee",
			"NAVAPSVC:Norton", "NAVAPW32:Norton", "NAVLU32:Norton", "NAVW32:Norton Antivirus", "NEOWATCHLOG:NeoWatch", "NEOWATCHTRAY:NeoWatch", "NISSERV:Norton", "NISUM:Norton", "NMAIN:Norton", "NOD32:ESET NOD32",
			"NPFMSG:Norman个人防火墙", "NPROTECT:Symantec", "NSMDTR:Norton", "NTRTSCAN:趋势科技", "OFCPFWSVC:OfficeScanNT", "ONLINENT:已知杀软进程,名称暂未收录", "OP_MON: OutpostFirewall", "PAVFIRES:熊猫卫士", "PAVFNSVR:熊猫卫士", "PAVKRE:熊猫卫士",
			"PAVPROT:熊猫卫士", "PAVPROXY:熊猫卫士", "PAVPRSRV:熊猫卫士", "PAVSRV51:熊猫卫士", "PAVSS:熊猫卫士", "PCCGUIDE:PC-cillin", "PCCIOMON:PC-cillin", "PCCNTMON:PC-cillin", "PCCPFW:趋势科技", "PCCTLCOM:趋势科技",
			"PCTAV:PC Tools AntiVirus", "PERSFW:Tiny Personal Firewall", "PERVAC:已知杀软进程,名称暂未收录", "PESTPATROL:Ikarus", "PREVSRV:熊猫卫士", "RTVSCN95:Real-time Virus Scanner", "SAVADMINSERVICE:SAV", "SAVMAIN:SAV", "SAVSCAN:SAV", "SDHELP:Spyware Doctor",
			"SHSTAT:McAfee", "SPBBCSVC:Symantec", "SPIDERCPL:Dr.Web", "SPIDERML:Dr.Web", "SPIDERUI:Dr.Web", "SPYBOTSD:Spybot ", "SWAGENT:SonicWALL", "SWDOCTOR:SonicWALL", "SWNETSUP:Sophos", "SYMLCSVC:Symantec",
			"SYMPROXYSVC:Symantec", "SYMSPORT:Sysmantec", "SYMWSC:Sysmantec", "SYNMGR:Sysmantec", "TMLISTEN:趋势科技", "TMNTSRV:趋势科技", "TMPROXY:趋势科技", "TNBUTIL:Anti-Virus", "VBA32ECM:已知杀软进程,名称暂未收录", "VBA32IFS:已知杀软进程,名称暂未收录",
			"VBA32PP3:已知杀软进程,名称暂未收录", "VCRMON:VirusChaser", "VRMONNT:HAURI", "VRMONSVC:HAURI", "VSHWIN32:McAfee", "VSSTAT:McAfee", "XCOMMSVR:BitDefender", "ZONEALARM:Zone Alarm", "360rp:360杀毒", "afwServ: Avast Antivirus ",
			"safeboxTray:360杀毒", "360safebox:360杀毒", "QQPCTray:QQ电脑管家", "KSafeTray:金山毒霸", "KSafeSvc:金山毒霸", "KWatch:金山毒霸", "gov_defence_service:云锁", "gov_defence_daemon:云锁", "smartscreen:Windows Defender", "macompatsvc:McAfee",
			"mcamnsvc.exe :McAfee", "masvc:McAfee", "mfemms:McAfee", "mctary:McAfee", "mcshield:McAfee", "mfewc:McAfee", "mfewch:McAfee", "mfefw:McAfee", "mfefire:McAfee", "mfetp:McAfee",
			"mfecanary:McAfee", "mfeconsole:McAfee", "mfeesp:McAfee", "fcag:McAfee", "fcags:McAfee", "fcagswd:McAfee", "fcagate:McAfee", "360EntClient:天擎EDR Agent", "edr_sec_plan:深信服EDR Agent", "edr_monitor:深信服EDR Agent",
			"edr_agent:深信服EDR Agent", "ESCCControl:启明星辰天珣EDR Agent", "ESCC:启明星辰天珣EDR Agent", "ESAV:启明星辰天珣EDR Agent", "ESCCIndex:启明星辰天珣EDR Agent", "AliYunDun:阿里云云盾", "wdswfsafe:360杀毒-网盾"
		});
		return list;
	}
}
