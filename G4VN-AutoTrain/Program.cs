using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CrackVLBS
{
	// Token: 0x02000006 RID: 6
	internal static class Program
	{
		// Token: 0x06000058 RID: 88
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06000059 RID: 89 RVA: 0x0000546C File Offset: 0x0000546C
		[STAThread]
		private static void Main(string[] args)
		{
			// 分配控制台窗口，用于显示调试信息
			AllocConsole();
			
			// 设置日志文件
			try
			{
				string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patcher_log.txt");
				StreamWriter logWriter = new StreamWriter(logFile, false, System.Text.Encoding.UTF8);
				logWriter.AutoFlush = true;
				
				// 创建自定义TextWriter，同时输出到控制台和文件
				TextWriter originalOut = Console.Out;
				Console.SetOut(new MultiTextWriter(originalOut, logWriter));
				
				Console.WriteLine("=== G4VN自动挂机启动器 ===");
				Console.WriteLine("控制台已启用，可以查看调试信息");
				Console.WriteLine("日志文件: " + logFile);
				Console.WriteLine("时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				Console.WriteLine();
				
				// 确保日志立即刷新
				logWriter.Flush();
			}
			catch (Exception ex)
			{
				Console.WriteLine("警告: 无法创建日志文件: " + ex.Message);
			}
			
			if (args.Length != 0)
			{
				Console.WriteLine("Got ya!!");
				if (args[0] == "vlbs19")
				{
					Console.WriteLine("vlbs19 start");
					if (!File.Exists(JXTrain.pathConfig))
					{
						JXTrain.WriteConfig(JXTrain.pathConfig);
					}
					JXTrain.GetConfig(JXTrain.pathConfig);
					string idcomputer = JXTrain.GetIDComputer();
					if (idcomputer != JXTrain.IDConfig | JXTrain.IDVLBS == "")
					{
						if (idcomputer != JXTrain.IDConfig)
						{
							JXTrain.EditIDConfig(idcomputer, JXTrain.pathConfig);
						}
						JXTrain.IDVLBS = JXTrain.GetVLBSKey(JXTrain.pathAutoVLBS19);
						JXTrain.EditVLBSConfig(JXTrain.IDVLBS, JXTrain.pathConfig);
						if (JXTrain.IDVLBS == "")
						{
							MessageBox.Show("破解失败，请重新运行！");
						}
						if (JXTrain.IDVLBS.Contains("_"))
						{
							MessageBox.Show("您的电脑只能运行1.3版本。请暂时使用，等待更新。");
							new CrackVLBS19v3().Crack13(JXTrain.pathAutoVLBS13, JXTrain.textDomain);
							return;
						}
						new CrackVLBS19v3().Crack(JXTrain.pathAutoVLBS19, JXTrain.pathZip, JXTrain.textDomain, JXTrain.IDVLBS, false);
						return;
					}
					else
					{
						if (JXTrain.IDVLBS.Contains("_"))
						{
							MessageBox.Show("您的电脑只能运行1.3版本。请暂时使用，等待更新。");
							new CrackVLBS19v3().Crack13(JXTrain.pathAutoVLBS13, JXTrain.textDomain);
							return;
						}
						new CrackVLBS19v3().Crack(JXTrain.pathAutoVLBS19, JXTrain.pathZip, JXTrain.textDomain, JXTrain.IDVLBS, false);
						return;
					}
				}
				else if (args[0] == "vlbs13")
				{
					Console.WriteLine("vlbs13 start");
					new CrackVLBS19v3().Crack13(JXTrain.pathAutoVLBS13, JXTrain.textDomain);
					return;
				}
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new JXTrain());
			}
		}
	}
}
