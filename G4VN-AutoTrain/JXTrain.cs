using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ManagedWinapi.Windows;

namespace CrackVLBS
{
	// Token: 0x02000005 RID: 5
	public partial class JXTrain : Form
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00004698 File Offset: 0x00004698
		public JXTrain()
		{
			this.InitializeComponent();
			if (!File.Exists(JXTrain.pathAutoVLBS19) || !File.Exists(JXTrain.pathAutoVLBS13))
			{
				MessageBox.Show("自动挂机文件丢失，请重新下载！");
				base.Close();
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000046D0 File Offset: 0x000046D0
		private void killprocess(string nameprocess)
		{
			try
			{
				foreach (Process process in Process.GetProcessesByName(nameprocess))
				{
					if (process.Id != Process.GetCurrentProcess().Id)
					{
						process.Kill();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004724 File Offset: 0x00004724
		public static string GetIDComputer()
		{
			string result;
			try
			{
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive").Get().GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						result = "";
					}
					else
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						object obj = managementObject["Model"];
						string str = (obj != null) ? obj.ToString() : null;
						string str2 = " ";
						object obj2 = managementObject["SerialNumber"];
						result = (str + str2 + ((obj2 != null) ? obj2.ToString() : null)).Replace(" ", string.Empty);
					}
				}
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000047E4 File Offset: 0x000047E4
		public static void WriteConfig(string pathConfig)
		{
			string[] contents = new string[]
			{
				"IDConfig=",
				"IDVLBS="
			};
			if (File.Exists(pathConfig))
			{
				return;
			}
			File.WriteAllLines(pathConfig, contents);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004818 File Offset: 0x00004818
		public static void GetConfig(string pathConfig)
		{
			if (!File.Exists(pathConfig))
			{
				return;
			}
			foreach (string text in new List<string>(File.ReadAllLines(pathConfig)))
			{
				if (text.IndexOf("IDConfig=") != -1)
				{
					JXTrain.IDConfig = text.Split(new char[]
					{
						'='
					})[1];
				}
				if (text.IndexOf("IDVLBS=") != -1)
				{
					JXTrain.IDVLBS = text.Split(new char[]
					{
						'='
					})[1];
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000048C0 File Offset: 0x000048C0
		public static void EditIDConfig(string _id, string patchConfig)
		{
			if (!File.Exists(patchConfig))
			{
				return;
			}
			string[] array = File.ReadAllLines(patchConfig);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IndexOf("IDConfig=") != -1)
				{
					string text = "IDConfig=" + _id;
					array[i] = text;
					break;
				}
			}
			File.WriteAllLines(patchConfig, array);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004914 File Offset: 0x00004914
		public static void EditVLBSConfig(string _id, string patchConfig)
		{
			if (!File.Exists(patchConfig))
			{
				return;
			}
			string[] array = File.ReadAllLines(patchConfig);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IndexOf("IDVLBS=") != -1)
				{
					string text = "IDVLBS=" + _id;
					array[i] = text;
					break;
				}
			}
			File.WriteAllLines(patchConfig, array);
		}

		// Token: 0x06000046 RID: 70
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

		// Token: 0x06000047 RID: 71
		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		// Token: 0x06000048 RID: 72
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		public static extern int SendMessage_1(IntPtr hwndControl, uint Msg, int wParam, StringBuilder strBuffer);

		// Token: 0x06000049 RID: 73
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		public static extern int SendMessage_2(IntPtr hwndControl, uint Msg, int wParam, int lParam);

		// Token: 0x0600004A RID: 74 RVA: 0x00004968 File Offset: 0x00004968
		public static int GetTextBoxTextLength(IntPtr hTextBox)
		{
			return JXTrain.SendMessage_2(hTextBox, 14U, 0, 0);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004974 File Offset: 0x00004974
		public static string GetTextBoxText(IntPtr hTextBox)
		{
			Thread.Sleep(100);
			uint msg = 13U;
			int textBoxTextLength = JXTrain.GetTextBoxTextLength(hTextBox);
			if (textBoxTextLength <= 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder(textBoxTextLength + 1);
			JXTrain.SendMessage_1(hTextBox, msg, textBoxTextLength + 1, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000049B8 File Offset: 0x000049B8
		private static string GetHWID(string key)
		{
			string[] array = key.Split(new char[]
			{
				'-'
			});
			if (array.Length != 3 || !(array[2].Length == 13 & array[0].Length == 13 & array[1].Length == 8))
			{
				return key;
			}
			array[1] = array[1] + array[2].Substring(0, 1);
			array[2] = array[2].Substring(1, array[2].Length - 1);
			char[] array2 = array[2].ToCharArray();
			for (int i = 0; i < array2.Length; i++)
			{
				if (i % 2 == 1)
				{
					char c = array2[i];
					array2[i] = array2[i - 1];
					array2[i - 1] = c;
				}
			}
			array[2] = new string(array2);
			return string.Concat(new string[]
			{
				array[0],
				"-",
				array[1],
				"-",
				array[2]
			});
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004A98 File Offset: 0x00004A98
		public static string GetVLBSKey(string pathAuto)
		{
			string result;
			try
			{
				if (!File.Exists(pathAuto))
				{
					Console.WriteLine("DEBUG: GetVLBSKey - File not found: " + pathAuto);
					result = "";
				}
				else
				{
					Console.WriteLine("DEBUG: GetVLBSKey - Starting process: " + pathAuto);
					JXTrain.KillProcces("G4VNVLBS.exe");
					Process process = new Process
					{
						StartInfo = 
						{
							FileName = pathAuto,
							WorkingDirectory = Path.GetDirectoryName(pathAuto),
							UseShellExecute = false,
							RedirectStandardOutput = true,
							RedirectStandardError = true
						}
					};
					process.Start();
					Console.WriteLine("DEBUG: GetVLBSKey - Process started, PID: " + process.Id);
					
					// 增加等待时间，从 5秒 增加到 15秒
					SystemWindow[] array = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "AutoVLBS 1.9" | w.Title == JXTrain.textDomain);
					Console.WriteLine("DEBUG: GetVLBSKey - Waiting for main window (AutoVLBS 1.9 or " + JXTrain.textDomain + ")...");
					for (int i = 0; i < 300; i++)
					{
						array = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "AutoVLBS 1.9" | w.Title == JXTrain.textDomain);
						if (array.Length != 0)
						{
							Console.WriteLine("DEBUG: GetVLBSKey - Main window found! Title: " + array[0].Title);
							break;
						}
						if (i % 20 == 0 && i > 0)
						{
							Console.WriteLine("DEBUG: GetVLBSKey - Still waiting for main window... (" + (i * 50 / 1000) + " seconds)");
						}
						Thread.Sleep(50);
					}
					if (array.Length == 0)
					{
						Console.WriteLine("DEBUG: GetVLBSKey - FAILED: Main window not found after 15 seconds");
						// 列出所有可见窗口以便调试
						Console.WriteLine("DEBUG: GetVLBSKey - Listing all top-level windows:");
						foreach (SystemWindow w in SystemWindow.FilterToplevelWindows((SystemWindow w) => true))
						{
							if (!string.IsNullOrEmpty(w.Title))
							{
								Console.WriteLine("DEBUG: GetVLBSKey - Found window: \"" + w.Title + "\"");
							}
						}
						JXTrain.KillProcces("G4VNVLBS.exe");
						result = "";
					}
					else
					{
						Console.WriteLine("DEBUG: GetVLBSKey - Looking for button '§¨ng ký'...");
						SystemWindow[] array2 = array[0].FilterDescendantWindows(false, (SystemWindow w) => w.Title == "§¨ng ký");
						for (int j = 0; j < 200; j++)
						{
							array2 = array[0].FilterDescendantWindows(false, (SystemWindow w) => w.Title == "§¨ng ký");
							if (array2.Length != 0)
							{
								Console.WriteLine("DEBUG: GetVLBSKey - Button '§¨ng ký' found!");
								break;
							}
							if (j % 20 == 0 && j > 0)
							{
								Console.WriteLine("DEBUG: GetVLBSKey - Still waiting for button... (" + (j * 50 / 1000) + " seconds)");
							}
							Thread.Sleep(50);
						}
						if (array2.Length == 0)
						{
							Console.WriteLine("DEBUG: GetVLBSKey - FAILED: Button '§¨ng ký' not found");
							JXTrain.KillProcces("G4VNVLBS.exe");
							result = "";
						}
						else
						{
							Console.WriteLine("DEBUG: GetVLBSKey - Clicking button '§¨ng ký'...");
							JXTrain.PostMessage(array2[0].HWnd, 513, 1, JXTrain.lParam);
							Thread.Sleep(50);
							JXTrain.PostMessage(array2[0].HWnd, 514, 0, JXTrain.lParam);
							Thread.Sleep(100);
							JXTrain.PostMessage(array2[0].HWnd, 513, 1, JXTrain.lParam);
							Thread.Sleep(50);
							JXTrain.PostMessage(array2[0].HWnd, 514, 0, JXTrain.lParam);
							Console.WriteLine("DEBUG: GetVLBSKey - Waiting for 'Dang ky' window...");
							SystemWindow[] array3 = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "Dang ky");
							for (int k = 0; k < 200; k++)
							{
								array3 = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "Dang ky");
								if (array3.Length != 0)
								{
									Console.WriteLine("DEBUG: GetVLBSKey - 'Dang ky' window found!");
									break;
								}
								if (k % 20 == 0 && k > 0)
								{
									Console.WriteLine("DEBUG: GetVLBSKey - Still waiting for 'Dang ky' window... (" + (k * 50 / 1000) + " seconds)");
								}
								Thread.Sleep(50);
							}
							if (array3.Length == 0)
							{
								Console.WriteLine("DEBUG: GetVLBSKey - FAILED: 'Dang ky' window not found");
								JXTrain.KillProcces("G4VNVLBS.exe");
								result = "";
							}
							else
							{
								Console.WriteLine("DEBUG: GetVLBSKey - Looking for Edit control...");
								SystemWindow[] array4 = array3[0].FilterDescendantWindows(false, (SystemWindow w) => w.ClassName == "Edit");
								for (int l = 0; l < 200; l++)
								{
									array4 = array3[0].FilterDescendantWindows(false, (SystemWindow w) => w.ClassName == "Edit");
									if (array4.Length != 0)
									{
										Console.WriteLine("DEBUG: GetVLBSKey - Edit control found! Count: " + array4.Length);
										break;
									}
									if (l % 20 == 0 && l > 0)
									{
										Console.WriteLine("DEBUG: GetVLBSKey - Still waiting for Edit control... (" + (l * 50 / 1000) + " seconds)");
									}
									Thread.Sleep(50);
								}
								if (array4.Length == 0)
								{
									Console.WriteLine("DEBUG: GetVLBSKey - FAILED: Edit control not found");
									JXTrain.KillProcces("G4VNVLBS.exe");
									result = "";
								}
								else
								{
									Console.WriteLine("DEBUG: GetVLBSKey - Reading text from Edit control...");
									string textBoxText = JXTrain.GetTextBoxText(array4[0].HWnd);
									Console.WriteLine("Raw HWID = " + textBoxText);
									if (string.IsNullOrEmpty(textBoxText))
									{
										Console.WriteLine("DEBUG: GetVLBSKey - WARNING: TextBox text is empty!");
									}
									JXTrain.KillProcces("G4VNVLBS.exe");
									string hwid = JXTrain.GetHWID(textBoxText);
									Console.WriteLine("Process HWID = " + hwid);
									result = hwid;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("DEBUG: GetVLBSKey - EXCEPTION: " + ex.Message);
				Console.WriteLine("DEBUG: GetVLBSKey - Stack trace: " + ex.StackTrace);
				result = "";
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004DE0 File Offset: 0x00004DE0
		internal void CrackVLBS19Full()
		{
			JXTrain.GetConfig(JXTrain.pathConfig);
			string idcomputer = JXTrain.GetIDComputer();
			Console.WriteLine("idComputer= " + idcomputer);
			if (!(idcomputer != JXTrain.IDConfig | JXTrain.IDVLBS == ""))
			{
				new CrackVLBS19v3().Crack(JXTrain.pathAutoVLBS19, JXTrain.pathZip, JXTrain.textDomain, JXTrain.IDVLBS, false);
				return;
			}
			if (idcomputer != JXTrain.IDConfig)
			{
				JXTrain.EditIDConfig(idcomputer, JXTrain.pathConfig);
			}
			JXTrain.IDVLBS = JXTrain.GetVLBSKey(JXTrain.pathAutoVLBS19);
			Console.WriteLine("IDVLBS= " + JXTrain.IDVLBS);
			JXTrain.EditVLBSConfig(JXTrain.IDVLBS, JXTrain.pathConfig);
			if (JXTrain.IDVLBS == "")
			{
				MessageBox.Show("破解失败，请重新运行！");
				return;
			}
			Thread.Sleep(10);
			new CrackVLBS19v3().Crack(JXTrain.pathAutoVLBS19, JXTrain.pathZip, JXTrain.textDomain, JXTrain.IDVLBS, false);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004EDC File Offset: 0x00004EDC
		private void JXTrain_Load(object sender, EventArgs e)
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			string directoryName = Path.GetDirectoryName(assembly.Location);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assembly.Location);
			string extension = Path.GetExtension(assembly.Location);
			Path.Combine(directoryName, fileNameWithoutExtension + extension);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			JXTrain.WriteConfig(JXTrain.pathConfig);
			this.label1.Text = JXTrain.nameServer;
			this.linkLabel1.Text = JXTrain.textLink;
			this.killprocess(fileNameWithoutExtension);
			this.killprocess(fileNameWithoutExtension + "2");
			this.killprocess("AutoTrainJX");
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004F84 File Offset: 0x00004F84
		public static void KillProcces(string processname)
		{
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = "taskkill",
					Arguments = "/F /IM " + processname
				};
				process.Start();
				process.WaitForExit(10000);
			}
			catch
			{
			}
			try
			{
				Process[] processesByName = Process.GetProcessesByName(processname.Replace(".exe", ""));
				for (int i = 0; i < processesByName.Length; i++)
				{
					processesByName[i].Kill();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000502C File Offset: 0x0000502C
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(JXTrain.textLink);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005039 File Offset: 0x00005039
		private void button_0_Click(object sender, EventArgs e)
		{
			new Thread(delegate()
			{
				this.CrackVLBS19Full();
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005058 File Offset: 0x00005058
		private void button1_Click(object sender, EventArgs e)
		{
			new Thread(delegate()
			{
				new CrackVLBS19v3().Crack13(JXTrain.pathAutoVLBS13, JXTrain.textDomain);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x04000018 RID: 24
		public static string textDomain = "剑侠1辅助";

		// Token: 0x04000019 RID: 25
		public static string textLink = "https://hoiuc.g4vn.net/";

		// Token: 0x0400001A RID: 26
		public static string nameServer = "剑侠1辅助";

		// Token: 0x0400001B RID: 27
		public static string pathAutoVLBS19 = AppDomain.CurrentDomain.BaseDirectory + "\\VLBS19\\G4VNVLBS.exe";

		// Token: 0x0400001C RID: 28
		public static string pathAutoVLBS13 = AppDomain.CurrentDomain.BaseDirectory + "\\VLBS13\\G4VNVLBS.exe";

		// Token: 0x0400001D RID: 29
		public static string pathZip = AppDomain.CurrentDomain.BaseDirectory + "VLBS19.zip";

		// Token: 0x0400001E RID: 30
		public static string titleAutoVLBS13 = "AutoVLBS 1.3";

		// Token: 0x0400001F RID: 31
		public static string titleAutoVLBS19 = "AutoVLBS 1.9";

		// Token: 0x04000020 RID: 32
		public static string pathConfig = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

		// Token: 0x04000021 RID: 33
		public static string IDConfig = "";

		// Token: 0x04000022 RID: 34
		public static string IDVLBS = "";

		// Token: 0x04000023 RID: 35
		private static int lParam = 65537;
	}
}
