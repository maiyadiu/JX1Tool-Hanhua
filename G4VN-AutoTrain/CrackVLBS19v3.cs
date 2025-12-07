using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagedWinapi.Windows;

namespace CrackVLBS
{
	// Token: 0x02000003 RID: 3
	internal class CrackVLBS19v3
	{
		// Token: 0x06000018 RID: 24
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		// Token: 0x06000019 RID: 25
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

		// Token: 0x0600001A RID: 26
		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		// Token: 0x0600001B RID: 27
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		public static extern int SendMessage_1(IntPtr hwndControl, uint Msg, int wParam, StringBuilder strBuffer);

		// Token: 0x0600001C RID: 28
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

		// Token: 0x0600001D RID: 29
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		// Token: 0x0600001E RID: 30
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

		// Token: 0x0600001F RID: 31
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
		private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x06000020 RID: 32
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
		private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x06000021 RID: 33 RVA: 0x00002102 File Offset: 0x00002102
		private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size != 8)
			{
				return CrackVLBS19v3.SetWindowLong32(hWnd, nIndex, dwNewLong);
			}
			return CrackVLBS19v3.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		// Token: 0x06000022 RID: 34
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x06000023 RID: 35 RVA: 0x0000211D File Offset: 0x0000211D
		private static IntPtr ButtonSubclassProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			if (msg == 12U)
			{
				return IntPtr.Zero;
			}
			return CrackVLBS19v3.CallWindowProc(CrackVLBS19v3._originalButtonWndProc, hWnd, msg, wParam, lParam);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002138 File Offset: 0x00002138
		internal static void PatchButton(IntPtr btn, string newText)
		{
			if (btn == IntPtr.Zero || !CrackVLBS19v3.IsWindow(btn))
			{
				return;
			}
		// 保存按钮的原始启用状态
		bool wasEnabled = ApiHelper.IsWindowEnabled(btn);
		
		// 临时禁用按钮以进行补丁（某些控件需要禁用才能修改文本）
		ApiHelper.EnableWindow(btn, false);
		CrackVLBS19v3.SendMessage(btn, 12U, IntPtr.Zero, newText);
		CrackVLBS19v3._textToBlock = newText;
		CrackVLBS19v3._originalButtonWndProc = CrackVLBS19v3.SetWindowLongPtr(btn, -4, Marshal.GetFunctionPointerForDelegate<CrackVLBS19v3.WndProc>(new CrackVLBS19v3.WndProc(CrackVLBS19v3.ButtonSubclassProc)));
		
		// 立即恢复按钮的原始启用状态（如果原来是启用的）
		if (wasEnabled)
		{
			ApiHelper.EnableWindow(btn, true);
			// 使用SendMessage发送WM_ENABLE消息，确保启用状态被应用
			// WM_ENABLE = 0x000A, wParam=1表示启用
			// 注意：使用ApiHelper.SendMessage，它接受IntPtr参数
			ApiHelper.SendMessage(btn, 0x000AU, new IntPtr(1), IntPtr.Zero);
		}
		
		Task.Run(delegate()
		{
			Thread.Sleep(1000);
			CrackVLBS19v3.SetWindowLongPtr(btn, -4, CrackVLBS19v3._originalButtonWndProc);
			// 再次确保按钮保持启用状态（如果原来是启用的）
			if (wasEnabled && !ApiHelper.IsWindowEnabled(btn))
			{
				ApiHelper.EnableWindow(btn, true);
				// 使用ApiHelper.SendMessage，它接受IntPtr参数
				ApiHelper.SendMessage(btn, 0x000AU, new IntPtr(1), IntPtr.Zero); // WM_ENABLE
			}
		});
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000021CE File Offset: 0x000021CE
		private static bool IsWindow(IntPtr hWnd)
		{
			return ApiHelper.IsWindow(hWnd);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000021D8 File Offset: 0x000021D8
		private static void ForceButtonText(IntPtr hWnd, string newText)
		{
			if (!CrackVLBS19v3.IsWindow(hWnd))
			{
				return;
			}
			bool flag = (CrackVLBS19v3.GetWindowLong(hWnd, -16) & 134217728) != 0;
			if (flag)
			{
				ApiHelper.EnableWindow(hWnd, true);
			}
			for (int i = 0; i < 5; i++)
			{
				ApiHelper.SetWindowText(hWnd, newText);
				Thread.Sleep(20);
				if (Helper.GetWindowTextDebug(hWnd) == newText)
				{
					break;
				}
			}
			if (flag)
			{
				ApiHelper.EnableWindow(hWnd, false);
			}
		}

		// Token: 0x06000027 RID: 39
		public void Crack13(string pathAuto, string tittleNew)
		{
			if (File.Exists(pathAuto))
			{
				JXTrain.KillProcces("AutoPKG4VN.exe");
				this.EditHost();
				Console.WriteLine("Dang Crack VLBS 1.3");
				JXTrain.KillProcces("G4VNVLBS.exe");
				Process process = new Process();
				process.StartInfo.FileName = pathAuto;
				process.StartInfo.WorkingDirectory = Path.GetDirectoryName(pathAuto);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				Console.WriteLine("Crack thanh cong");
				for (int i = 0; i < 100; i++)
				{
					Thread.Sleep(50);
					if (CrackVLBS19v3.IsWindowVisible(process.MainWindowHandle))
					{
						ApiHelper.MoveWindowToTopRight(process.MainWindowHandle);
						ApiHelper.SetWindowText(process.MainWindowHandle, tittleNew);
						CrackVLBS19v3.PatchButton(Helper.FindWindowRecursive(process.MainWindowHandle, "Button", "vulanpro.net"), "剑侠1辅助");
						CrackVLBS19v3.PatchButton(Helper.FindWindowRecursive(process.MainWindowHandle, "Button", "§¨ng ký"), "hoiuc.g4vn.net");
						
						// 启动界面中文补丁
						Console.WriteLine("DEBUG: Starting UI Chinese patcher for VLBS 1.3...");
						Console.WriteLine("DEBUG: Process state - HasExited: " + process.HasExited + ", MainWindowHandle: " + process.MainWindowHandle);
						try
						{
							UIChinesePatcher.PatchAllWindows(process, 50);
							Console.WriteLine("DEBUG: UIChinesePatcher.PatchAllWindows called successfully");
						}
						catch (Exception ex)
						{
							Console.WriteLine("DEBUG: ERROR calling PatchAllWindows: " + ex.Message);
						}
						
						try
						{
							UIChinesePatcher.StartContinuousPatching(process, 2000);
							Console.WriteLine("DEBUG: UIChinesePatcher.StartContinuousPatching called successfully");
						}
						catch (Exception ex)
						{
							Console.WriteLine("DEBUG: ERROR calling StartContinuousPatching: " + ex.Message);
						}
						
						// 启动内存字符串补丁（需要重新打开进程句柄）
						Thread memoryPatchThread = new Thread(() =>
						{
							Console.WriteLine("DEBUG: Waiting 2 seconds before opening process handle for memory patching...");
							Thread.Sleep(2000);
							Console.WriteLine("DEBUG: Attempting to open process handle for PID=" + process.Id);
							int processHandle13 = ApiHelper.OpenProcess(2035711, false, process.Id);
							Console.WriteLine("DEBUG: OpenProcess result: " + processHandle13);
							if (processHandle13 != 0)
							{
								Console.WriteLine("DEBUG: Starting memory string patcher for VLBS 1.3...");
								Console.WriteLine("DEBUG: Process handle for memory patching: " + processHandle13);
								try
								{
									MemoryStringPatcher.PatchMemoryStrings(process, processHandle13);
									MemoryStringPatcher.StartContinuousMemoryPatching(process, processHandle13, 5000);
									Console.WriteLine("DEBUG: Memory string patcher started successfully for VLBS 1.3");
									
									// 启动激进文本补丁
									Console.WriteLine("DEBUG: Starting aggressive text patcher for VLBS 1.3...");
									AggressiveTextPatcher.StartAggressivePatching(process, 1000);
									Console.WriteLine("DEBUG: Aggressive text patcher started successfully for VLBS 1.3");
								}
								catch (Exception ex)
								{
									Console.WriteLine("DEBUG: ERROR starting memory string patcher for VLBS 1.3: " + ex.Message);
									Console.WriteLine("DEBUG: Stack trace: " + ex.StackTrace);
								}
							}
							else
							{
								Console.WriteLine("DEBUG: ERROR - Failed to open process handle for memory patching! Error code: " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
							}
						})
						{
							IsBackground = true
						};
						memoryPatchThread.Start();
						
						return;
					}
					uint num = 4634900U;
					string inputString = "Vo Lam Truyen Ky";
					byte[] array = new byte[16];
					int int_ = process.Handle.ToInt32();
					int num2 = 0;
					if (ApiHelper.ReadProcessMemory(int_, num, array, array.Length, ref num2))
					{
						string arg = Helper.ByteArrayToString(array);
						Console.WriteLine(string.Format("Original string at 0x{0:X}: {1}", num, arg));
						byte[] array2 = Helper.StringToByteArray(inputString, false);
						if (array2.Length > 16)
						{
							Array.Resize<byte>(ref array2, 16);
						}
						if (ApiHelper.WriteProcessMemory(int_, num, array2, array2.Length, ref num2))
						{
							Console.WriteLine("Successfully wrote new string to memory.");
						}
						else
						{
							Console.WriteLine("Failed to write to process memory.");
						}
					}
					else
					{
						Console.WriteLine("Failed to read process memory.");
					}
				}
				return;
			}
			MessageBox.Show("找不到自动挂机文件！");
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000240C File Offset: 0x0000240C
		public void EditHost()
		{
			try
			{
				string[] array = new string[]
				{
					"127.0.0.1 a.agtool.net",
					"127.0.0.1 a.agtool.net/*",
					"127.0.0.1 update.agtool.net",
					"127.0.0.1 update.agtool.net/*",
					"127.0.0.1 agtool.net",
					"127.0.0.1 agtool.net/*",
					"127.0.0.1 */agtool.net"
				};
				if (File.Exists(Environment.SystemDirectory + "\\drivers\\etc\\hosts.ics"))
				{
					File.Delete(Environment.SystemDirectory + "\\drivers\\etc\\hosts.ics");
				}
				if (File.Exists(Environment.SystemDirectory + "\\drivers\\etc\\hosts"))
				{
					List<string> list = File.ReadAllLines(Environment.SystemDirectory + "\\drivers\\etc\\hosts").ToList<string>();
					bool flag = false;
					foreach (string text in array)
					{
						bool flag2 = false;
						foreach (string b in list)
						{
							if (text == b)
							{
								flag2 = true;
							}
						}
						if (!flag2)
						{
							list.Add(text);
							flag = true;
						}
					}
					if (flag)
					{
						File.WriteAllLines(Environment.SystemDirectory + "\\drivers\\etc\\hosts", list);
					}
				}
				else
				{
					string text2 = "";
					foreach (string str in array)
					{
						text2 = text2 + str + Environment.NewLine;
					}
					File.WriteAllText(Environment.SystemDirectory + "\\drivers\\etc\\hosts", text2);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000029 RID: 41
		public void Crack(string pathAuto, string pathZip, string tittleNew, string hwidvlbs, bool bool_0 = false)
		{
			if (!bool_0)
			{
				JXTrain.KillProcces("AutoPKG4VN.exe");
				Console.WriteLine("DEBUG: Step 0 - Initial Setup");
				Console.WriteLine("DEBUG: Calling EditHost to clear hosts file");
				this.EditHost();
				Console.WriteLine("DEBUG: Killing process 'G4VNVLBS.exe'");
				JXTrain.KillProcces("G4VNVLBS.exe");
			}
			Console.WriteLine("true");
			Console.WriteLine("Step 1");
			uint num = 4194304U;
			uint num2 = 4319353U;
			uint num3 = 4318750U;
			uint num4 = 4316631U;
			Console.WriteLine(string.Concat(new string[]
			{
				"DEBUG: Step 1 - Defined Offsets: baseOffset=",
				Helper.NumberToHexString(num, 8, true, false),
				", f1Offset=",
				Helper.NumberToHexString(num2, 8, true, false),
				", f2Offset=",
				Helper.NumberToHexString(num3, 8, true, false),
				", hwidOffset=",
				Helper.NumberToHexString(num4, 8, true, false)
			}));
			if (CrackVLBS19v3._findWindowAAddress == 0U)
			{
				Console.WriteLine("Step 2");
				Console.WriteLine("DEBUG: Step 2 - Hooking FindWindowA");
				try
				{
					int id = Process.GetCurrentProcess().Id;
					Console.WriteLine(string.Format("DEBUG: Current Process ID: {0}", id));
					CrackVLBS19v3._findWindowAAddress = ApiHelper.GetProcAddress(Helper.GetModuleBaseAddress(id, "user32.dll"), "FindWindowA");
					Console.WriteLine("DEBUG: FindWindowA address: " + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress, 8, true, false));
					if (CrackVLBS19v3._findWindowAAddress == 0U)
					{
						Console.WriteLine("DEBUG: Failed to get FindWindowA address");
						MessageBox.Show("无法加载user32.dll模块，结束！");
						return;
					}
					int num5 = ApiHelper.OpenProcess(2035711, false, id);
					Console.WriteLine(string.Format("DEBUG: Opened process handle: {0}", num5));
					int num6 = 0;
					byte[] array = new byte[1];
					ApiHelper.ReadProcessMemory(num5, CrackVLBS19v3._findWindowAAddress + 5U, array, 1, ref num6);
					CrackVLBS19v3._originalFindWindowByte = array[0];
					Console.WriteLine("DEBUG: Original byte at FindWindowA+5 (" + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress + 5U, 8, true, false) + "): " + Helper.NumberToHexString(CrackVLBS19v3._originalFindWindowByte, 2, true, false));
					ApiHelper.CloseHandle((IntPtr)num5);
				}
				catch (Exception ex)
				{
					Console.WriteLine("DEBUG: Exception in Step 2: " + ex.Message);
					MessageBox.Show(ex.Message);
				}
			}
			int num7 = 0;
			int num8 = 0;
			byte[] array2 = new byte[1];
			uint num9 = 0U;
			Process process = null;
			bool flag = false;
			if (Helper.IsProcessExited(process))
			{
				Console.WriteLine("DEBUG: Starting process at path: " + pathAuto);
				process = Helper.StartProcess(pathAuto, Path.GetDirectoryName(pathAuto), "", 0, false, false);
				Console.WriteLine(string.Format("DEBUG: Target process started, PID: {0}", (process != null) ? new int?(process.Id) : null));
			}
			IntPtr mainWindowHandle = process.MainWindowHandle;
			Console.WriteLine(string.Format("DEBUG: Target process MainWindowHandle: {0}", mainWindowHandle));
			if (!Helper.IsApplicationStopped && !Helper.IsProcessExited(process))
			{
				if (!flag)
				{
					Console.WriteLine(string.Format("DEBUG: Suspending target process (PID: {0})", process.Id));
					Helper.SuspendProcess(process);
				}
				num7 = ApiHelper.OpenProcess(2035711, false, process.Id);
				Console.WriteLine(string.Format("DEBUG: Opened target process handle: {0}", num7));
				int num10 = 0;
				while (!Helper.IsApplicationStopped)
				{
					num9 = Helper.AllocateMemory(num7, 512U, 64);
					Console.WriteLine("DEBUG: Allocated memory at: " + Helper.NumberToHexString(num9, 8, true, false));
					if (num9 != 0U)
					{
						break;
					}
					num10++;
					if (!flag)
					{
						if (num10 > 600)
						{
							Console.WriteLine(string.Format("DEBUG: Failed to allocate memory after {0} retries", num10));
							Helper.AddStringToArray(ref Helper.ErrorMessages, "无法初始化缓存，结束。");
							goto IL_14D9;
						}
						Console.WriteLine(string.Format("DEBUG: Retry {0}/600 - Resuming and suspending process", num10));
						Helper.ResumeProcess(process);
						Thread.Sleep(1);
						Helper.SuspendProcess(process);
					}
				}
				int num11 = 0;
				while (!Helper.IsApplicationStopped)
				{
					ApiHelper.ReadProcessMemory(num7, CrackVLBS19v3._findWindowAAddress + 5U, array2, 1, ref num8);
					Console.WriteLine("DEBUG: Read byte at FindWindowA+5 (" + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress + 5U, 8, true, false) + "): " + Helper.NumberToHexString(array2[0], 2, true, false));
					if (array2[0] == CrackVLBS19v3._originalFindWindowByte)
					{
						break;
					}
					if (!flag)
					{
						num11++;
						if (num11 > 3000)
						{
							Console.WriteLine("DEBUG: Timeout waiting for FindWindowA patch verification");
							Helper.AddStringToArray(ref Helper.ErrorMessages, "打开游戏超时，结束。");
							goto IL_14D9;
						}
						Console.WriteLine(string.Format("DEBUG: Wait cycle {0}/3000 - Resuming and suspending process", num11));
						Helper.ResumeProcess(process);
						Thread.Sleep(1);
						Helper.SuspendProcess(process);
					}
				}
				byte[] array3 = Helper.HexStringToByteArray(string.Concat(new string[]
				{
					"3E 8D 44 24 04 8B 00 85 C0 74 21 81 38 56 42 6F 78 75 19 81 78 04 54 72 61 79 75 10A3",
					Helper.NumberToHexString(num9, 8, false, true),
					"90 9080 3D",
					Helper.NumberToHexString(num9, 8, false, true),
					"0075 F7 8B FF 55 8B ECE9 00 00 00 00"
				}), false);
				Console.WriteLine(string.Format("DEBUG: Stage 1 - Writing shellcode to {0}, length: {1} bytes", Helper.NumberToHexString(num9 + 4U, 8, true, false), array3.Length));
				ApiHelper.WriteProcessMemory(num7, num9 + 4U, array3, array3.Length, ref num8);
				Console.WriteLine(string.Format("DEBUG: Stage 1 - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num9 + 4U, 8, true, false)));
				uint num12 = (uint)(array3.Length + 16);
				uint num13 = num9 + (uint)array3.Length;
				uint num14 = CrackVLBS19v3._findWindowAAddress - num13 + 1U;
				Console.WriteLine("DEBUG: Stage 1 - Writing jump back at " + Helper.NumberToHexString(num13, 8, true, false) + ", value: " + Helper.NumberToHexString(num14, 8, true, false));
				Helper.WriteProcessMemoryUInt32(num13, num7, num14, 4);
				byte[] array4 = new byte[]
				{
					233
				};
				uint num15 = num9 - (CrackVLBS19v3._findWindowAAddress + 1U);
				Console.WriteLine("DEBUG: Stage 1 - Patching FindWindowA at " + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress, 8, true, false) + " with jmp (0xE9), relative jump: " + Helper.NumberToHexString(num15, 8, true, false));
				ApiHelper.WriteProcessMemory(num7, CrackVLBS19v3._findWindowAAddress, array4, 1, ref num8);
				Console.WriteLine(string.Format("DEBUG: Stage 1 - Wrote jmp opcode to {0}, bytes written: {1}", Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress, 8, true, false), num8));
				if (Helper.WriteProcessMemoryUInt32(CrackVLBS19v3._findWindowAAddress + 1U, num7, num15, 4))
				{
					Console.WriteLine("DEBUG: Stage 1 - Wrote relative jump " + Helper.NumberToHexString(num15, 8, true, false) + " to " + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress + 1U, 8, true, false));
					if (!flag)
					{
						Console.WriteLine(string.Format("DEBUG: Resuming target process (PID: {0})", process.Id));
						Helper.ResumeProcess(process);
					}
					int num16 = 0;
					while (!Helper.IsApplicationStopped && !flag)
					{
						uint num17 = Helper.ReadProcessMemoryUInt32(num9, num7, 4);
						Console.WriteLine("DEBUG: Polling remote buffer at " + Helper.NumberToHexString(num9, 8, true, false) + ", value: " + Helper.NumberToHexString(num17, 8, true, false));
						if (num17 == 0U)
						{
							num16++;
							Thread.Sleep(1);
							if (!flag && num16 > 5000)
							{
								Console.WriteLine(string.Format("DEBUG: Timeout during remote buffer check after {0} polls", num16));
								Helper.AddStringToArray(ref Helper.ErrorMessages, "检查超时，结束。");
								goto IL_14D9;
							}
							if (num16 % 300 == 0 && Helper.IsProcessExited(process))
							{
								Console.WriteLine("DEBUG: Target process exited during polling");
								goto IL_14D9;
							}
						}
						else
						{
							if (!flag)
							{
								Console.WriteLine("DEBUG: Suspending target process after successful poll");
								Helper.SuspendProcess(process);
								break;
							}
							break;
						}
					}
					uint num18;
					try
					{
						num18 = (uint)((int)process.MainModule.BaseAddress);
						Console.WriteLine("DEBUG: Module base address: " + Helper.NumberToHexString(num18, 8, true, false));
					}
					catch
					{
						num18 = 4194304U;
						Console.WriteLine("DEBUG: Failed to get module base, using default: " + Helper.NumberToHexString(num18, 8, true, false));
					}
					uint num19 = num18 + (num2 - num);
					Console.WriteLine("DEBUG: Stage 2 - Calculated F1 address: " + Helper.NumberToHexString(num19, 8, true, false));
					while (!Helper.IsApplicationStopped)
					{
						ApiHelper.ReadProcessMemory(num7, num19 + 5U, array4, 1, ref num8);
						Console.WriteLine("DEBUG: Stage 2 - Read byte at F1+5 (" + Helper.NumberToHexString(num19 + 5U, 8, true, false) + "): " + Helper.NumberToHexString(array4[0], 2, true, false));
						if (array4[0] != 87)
						{
							if (!flag)
							{
								Console.WriteLine("DEBUG: Stage 2 - F1 check failed, byte != 0x57");
								Helper.AddStringToArray(ref Helper.ErrorMessages, "无法检查F1，结束。");
								goto IL_14D9;
							}
							Thread.Sleep(100);
						}
						else
						{
							uint num20 = num9 + num12;
							byte[] array5 = Helper.StringToByteArray("http://chipchipchip.info/auto/vlbs19.txt", true);
							Console.WriteLine("DEBUG: Stage 2 - Writing F1 URL to " + Helper.NumberToHexString(num20, 8, true, false) + ", URL: http://chipchipchip.info/auto/vlbs19.txt");
							ApiHelper.WriteProcessMemory(num7, num20, array5, array5.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 2 - Wrote {0} bytes (F1 URL) to {1}", num8, Helper.NumberToHexString(num20, 8, true, false)));
							uint num21 = num12 + (uint)(array5.Length + 16);
							byte[] array6 = Helper.HexStringToByteArray("B8" + Helper.NumberToHexString(num20, 8, false, true) + "3E 8B 4D E0 53 8A 18 88 19 40 41 84 DB 75 F6 5B 68 01 00 00 04E9 00 00 00 00", true);
							Console.WriteLine(string.Format("DEBUG: Stage 2 - Writing shellcode to {0}, length: {1} bytes", Helper.NumberToHexString(num9 + num21, 8, true, false), array6.Length));
							ApiHelper.WriteProcessMemory(num7, num9 + num21, array6, array6.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 2 - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num9 + num21, 8, true, false)));
							uint num22 = num9 + num21 - (num19 + 5U);
							uint num23 = num9 + num21 + (uint)array6.Length - 4U;
							uint num24 = num19 - num23 + 1U;
							uint num25 = num21 + (uint)(array6.Length + 16);
							byte[] array7 = new byte[]
							{
								233
							};
							Console.WriteLine("DEBUG: Stage 2 - Patching F1 at " + Helper.NumberToHexString(num19, 8, true, false) + " with jmp (0xE9), relative jump: " + Helper.NumberToHexString(num22, 8, true, false));
							ApiHelper.WriteProcessMemory(num7, num19, array7, 1, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 2 - Wrote jmp opcode to {0}, bytes written: {1}", Helper.NumberToHexString(num19, 8, true, false), num8));
							Helper.WriteProcessMemoryUInt32(num19 + 1U, num7, num22, 4);
							Console.WriteLine("DEBUG: Stage 2 - Wrote relative jump " + Helper.NumberToHexString(num22, 8, true, false) + " to " + Helper.NumberToHexString(num19 + 1U, 8, true, false));
							Helper.WriteProcessMemoryUInt32(num23, num7, num24, 4);
							Console.WriteLine("DEBUG: Stage 2 - Wrote jump back " + Helper.NumberToHexString(num24, 8, true, false) + " to " + Helper.NumberToHexString(num23, 8, true, false));
							uint num26 = num18 + (num3 - num);
							Console.WriteLine("DEBUG: Stage 3 - Calculated F2 address: " + Helper.NumberToHexString(num26, 8, true, false));
							ApiHelper.ReadProcessMemory(num7, num26 + 5U, array7, 1, ref num8);
							Console.WriteLine("DEBUG: Stage 3 - Read byte at F2+5 (" + Helper.NumberToHexString(num26 + 5U, 8, true, false) + "): " + Helper.NumberToHexString(array7[0], 2, true, false));
							if (array7[0] != 106)
							{
								Console.WriteLine("DEBUG: Stage 3 - F2 check failed, byte != 0x6A");
								Helper.AddStringToArray(ref Helper.ErrorMessages, "无法检查F2，结束。");
								goto IL_14A0;
							}
							uint num27 = num9 + num25;
							byte[] array8 = Helper.StringToByteArray("http://chipchipchip.info/auto/vlbs19log.txt", true);
							Console.WriteLine("DEBUG: Stage 3 - Writing F2 URL to " + Helper.NumberToHexString(num27, 8, true, false) + ", URL: http://chipchipchip.info/auto/vlbs19log.txt");
							ApiHelper.WriteProcessMemory(num7, num27, array8, array8.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 3 - Wrote {0} bytes (F2 URL) to {1}", num8, Helper.NumberToHexString(num27, 8, true, false)));
							uint num28 = num25 + (uint)(array8.Length + 16);
							byte[] array9 = Helper.HexStringToByteArray("B8" + Helper.NumberToHexString(num27, 8, false, true) + "3E 8D 8D 98 F7 FF FF 53 8A 18 88 19 40 41 84 DB 75 F6 5B 68 01 00 00 04E9 00 00 00 00", true);
							Console.WriteLine(string.Format("DEBUG: Stage 3 - Writing shellcode to {0}, length: {1} bytes", Helper.NumberToHexString(num9 + num28, 8, true, false), array9.Length));
							ApiHelper.WriteProcessMemory(num7, num9 + num28, array9, array9.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 3 - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num9 + num28, 8, true, false)));
							uint num29 = num9 + num28 - (num26 + 5U);
							uint num30 = num9 + num28 + (uint)array9.Length - 4U;
							uint num31 = num26 - num30 + 1U;
							uint num32 = num28 + (uint)(array9.Length + 16);
							byte[] byte_ = new byte[]
							{
								233
							};
							Console.WriteLine("DEBUG: Stage 3 - Patching F2 at " + Helper.NumberToHexString(num26, 8, true, false) + " with jmp (0xE9), relative jump: " + Helper.NumberToHexString(num29, 8, true, false));
							ApiHelper.WriteProcessMemory(num7, num26, byte_, 1, ref num8);
							Console.WriteLine(string.Format("DEBUG: Stage 3 - Wrote jmp opcode to {0}, bytes written: {1}", Helper.NumberToHexString(num26, 8, true, false), num8));
							Helper.WriteProcessMemoryUInt32(num26 + 1U, num7, num29, 4);
							Console.WriteLine("DEBUG: Stage 3 - Wrote relative jump " + Helper.NumberToHexString(num29, 8, true, false) + " to " + Helper.NumberToHexString(num26 + 1U, 8, true, false));
							Helper.WriteProcessMemoryUInt32(num30, num7, num31, 4);
							Console.WriteLine("DEBUG: Stage 3 - Wrote jump back " + Helper.NumberToHexString(num31, 8, true, false) + " to " + Helper.NumberToHexString(num30, 8, true, false));
							uint num33 = num9 + num32;
							uint num34 = num32 + 80U;
							uint num35 = num9 + num34;
							string text = hwidvlbs;
							if (text == string.Empty)
							{
								text = "MA_O_CUNG_AUTO";
							}
							Console.WriteLine("DEBUG: HWID Patch - Using HWID: " + text);
							byte[] array10 = Helper.StringToByteArray(text, true);
							Console.WriteLine(string.Format("DEBUG: HWID Patch - Writing HWID to {0}, length: {1} bytes", Helper.NumberToHexString(num33, 8, true, false), array10.Length));
							ApiHelper.WriteProcessMemory(num7, num33, array10, array10.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: HWID Patch - Wrote {0} bytes (HWID) to {1}", num8, Helper.NumberToHexString(num33, 8, true, false)));
							uint num36 = num18 + (num4 - num);
							Console.WriteLine("DEBUG: HWID Patch - Calculated HWID address: " + Helper.NumberToHexString(num36, 8, true, false));
							byte[] array11 = new byte[5];
							ApiHelper.ReadProcessMemory(num7, num36, array11, 5, ref num8);
							Console.WriteLine("DEBUG: HWID Patch - Original bytes at " + Helper.NumberToHexString(num36, 8, true, false) + ": " + BitConverter.ToString(array11).Replace("-", " "));
							byte[] array12 = Helper.HexStringToByteArray("E9" + Helper.NumberToHexString(num35 - (num36 + 5U), 8, false, true), false);
							Console.WriteLine("DEBUG: HWID Patch - Writing jump to " + Helper.NumberToHexString(num36, 8, true, false) + ", target: " + Helper.NumberToHexString(num35, 8, true, false));
							ApiHelper.WriteProcessMemory(num7, num36, array12, array12.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: HWID Patch - Wrote {0} bytes (jump) to {1}", num8, Helper.NumberToHexString(num36, 8, true, false)));
							string text7 = "60 8B 4C 24 28 80 39 00 74 2C 33 C0 8A 19 40 41 84 DB 75 F8 83 F8 0F 7E 1D 48 49 8A 19 85 C0 74 15 80 FB 7C 75 F341BF" + Helper.NumberToHexString(num33, 8, false, true) + "8A 1F 88 19 41 47 84 DB 75 F6 61 E9";
							uint num37 = (uint)(text7.Replace(" ", string.Empty).Length / 2);
							byte[] array13 = Helper.HexStringToByteArray(text7 + Helper.NumberToHexString(num36 - (num35 + num37) + 1U, 8, false, true), true);
							Console.WriteLine(string.Format("DEBUG: HWID Patch - Writing shellcode to {0}, length: {1} bytes", Helper.NumberToHexString(num35, 8, true, false), array13.Length));
							ApiHelper.WriteProcessMemory(num7, num35, array13, array13.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: HWID Patch - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num35, 8, true, false)));
							uint num38 = 4650880U;
							string text2 = "Vo Lam Truyen Ky";
							byte[] array14 = new byte[16];
							ApiHelper.ReadProcessMemory(num7, num38, array14, array14.Length, ref num8);
							string text3 = Helper.ByteArrayToString(array14);
							Console.WriteLine(string.Concat(new string[]
							{
								"DEBUG: Static String Patch - Original content at ",
								Helper.NumberToHexString(num38, 8, true, false),
								": ",
								text3,
								" (Bytes: ",
								BitConverter.ToString(array14).Replace("-", " "),
								")"
							}));
							byte[] array15 = Helper.StringToByteArray(text2, false);
							if (array15.Length > 16)
							{
								Console.WriteLine(string.Format("DEBUG: Static String Patch - Warning: String '{0}' is {1} bytes, truncating to 13 bytes", text2, array15.Length));
								Array.Resize<byte>(ref array15, 16);
							}
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Writing string to {0}, string: {1}, length: {2} bytes", Helper.NumberToHexString(num38, 8, true, false), text2, array15.Length));
							ApiHelper.WriteProcessMemory(num7, num38, array15, array15.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num38, 8, true, false)));
							uint num39 = 4624104U;
							string text4 = "剑侠1免费自动挂机";
							byte[] array16 = new byte[22];
							ApiHelper.ReadProcessMemory(num7, num39, array16, array16.Length, ref num8);
							Helper.ByteArrayToString(array16);
							Console.WriteLine(string.Concat(new string[]
							{
								"DEBUG: Static String Patch - Original content at ",
								Helper.NumberToHexString(num39, 8, true, false),
								": ",
								text3,
								" (Bytes: ",
								BitConverter.ToString(array14).Replace("-", " "),
								")"
							}));
							byte[] array17 = Helper.StringToByteArray(text4, false);
							if (array17.Length > 22)
							{
								Console.WriteLine(string.Format("DEBUG: Static String Patch - Warning: String '{0}' is {1} bytes, truncating to 13 bytes", text4, array17.Length));
								Array.Resize<byte>(ref array17, 22);
							}
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Writing string to {0}, string: {1}, length: {2} bytes", Helper.NumberToHexString(num39, 8, true, false), text4, array17.Length));
							ApiHelper.WriteProcessMemory(num7, num39, array17, array17.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num39, 8, true, false)));
							uint num40 = 4624128U;
							string text5 = "剑侠1辅助 - 自动挂机";
							byte[] array18 = new byte[18];
							ApiHelper.ReadProcessMemory(num7, num40, array18, array18.Length, ref num8);
							Helper.ByteArrayToString(array18);
							Console.WriteLine(string.Concat(new string[]
							{
								"DEBUG: Static String Patch - Original content at ",
								Helper.NumberToHexString(num40, 8, true, false),
								": ",
								text3,
								" (Bytes: ",
								BitConverter.ToString(array14).Replace("-", " "),
								")"
							}));
							byte[] array19 = Helper.StringToByteArray(text5, false);
							if (array19.Length > 18)
							{
								Console.WriteLine(string.Format("DEBUG: Static String Patch - Warning: String '{0}' is {1} bytes, truncating to 13 bytes", text5, array19.Length));
								Array.Resize<byte>(ref array19, 18);
							}
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Writing string to {0}, string: {1}, length: {2} bytes", Helper.NumberToHexString(num40, 8, true, false), text5, array19.Length));
							ApiHelper.WriteProcessMemory(num7, num40, array19, array19.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num40, 8, true, false)));
							uint num41 = 4624076U;
							string text6 = "剑侠1永久免费自动挂机";
							byte[] array20 = new byte[27];
							ApiHelper.ReadProcessMemory(num7, num41, array20, array20.Length, ref num8);
							Helper.ByteArrayToString(array20);
							Console.WriteLine(string.Concat(new string[]
							{
								"DEBUG: Static String Patch - Original content at ",
								Helper.NumberToHexString(num41, 8, true, false),
								": ",
								text3,
								" (Bytes: ",
								BitConverter.ToString(array14).Replace("-", " "),
								")"
							}));
							byte[] array21 = Helper.StringToByteArray(text6, false);
							if (array21.Length > 27)
							{
								Console.WriteLine(string.Format("DEBUG: Static String Patch - Warning: String '{0}' is {1} bytes, truncating to 13 bytes", text6, array21.Length));
								Array.Resize<byte>(ref array21, 27);
							}
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Writing string to {0}, string: {1}, length: {2} bytes", Helper.NumberToHexString(num41, 8, true, false), text6, array21.Length));
							ApiHelper.WriteProcessMemory(num7, num41, array21, array21.Length, ref num8);
							Console.WriteLine(string.Format("DEBUG: Static String Patch - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(num41, 8, true, false)));
							goto IL_14A0;
						}
					}
				}
				Console.WriteLine("DEBUG: Killing target process due to version incompatibility");
				Helper.KillProcess(process);
				Helper.AddStringToArray(ref Helper.ErrorMessages, "此版本无法打开自动挂机。");
				IL_14A0:
				Console.WriteLine("DEBUG: Setting window title to: " + tittleNew);
				ApiHelper.SetWindowText(process.MainWindowHandle, tittleNew);
			}
			else
			{
				Console.WriteLine("DEBUG: Failed to open target process");
				Helper.AddStringToArray(ref Helper.ErrorMessages, "无法打开自动挂机。");
			}
			IL_14D9:
			if (num9 != 0U)
			{
				byte[] array22 = new byte[]
				{
					139,
					byte.MaxValue,
					85,
					139,
					236
				};
				Console.WriteLine("DEBUG: Cleanup - Restoring FindWindowA at " + Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress, 8, true, false) + ", original bytes: " + BitConverter.ToString(array22).Replace("-", " "));
				ApiHelper.WriteProcessMemory(num7, CrackVLBS19v3._findWindowAAddress, array22, array22.Length, ref num8);
				Console.WriteLine(string.Format("DEBUG: Cleanup - Wrote {0} bytes to {1}", num8, Helper.NumberToHexString(CrackVLBS19v3._findWindowAAddress, 8, true, false)));
				Console.WriteLine("DEBUG: Cleanup - Clearing remote buffer at " + Helper.NumberToHexString(num9, 8, true, false));
				Helper.WriteProcessMemoryUInt32(num9, num7, 0U, 4);
			}
			Console.WriteLine(string.Format("DEBUG: Resuming target process (PID: {0})", (process != null) ? new int?(process.Id) : null));
			Helper.ResumeProcess(process);
			
			// 启动内存字符串补丁（在关闭句柄之前，但需要保持句柄打开）
			Console.WriteLine("DEBUG: Starting memory string patcher...");
			Console.WriteLine("DEBUG: Process handle for memory patching: " + num7);
			// 注意：保持进程句柄打开，让内存补丁系统使用
			// 内存补丁系统会在后台线程中运行，需要保持句柄有效
			try
			{
				MemoryStringPatcher.PatchMemoryStrings(process, num7);
				MemoryStringPatcher.StartContinuousMemoryPatching(process, num7, 5000);
				Console.WriteLine("DEBUG: Memory string patcher started successfully");
				
				// 启动激进文本补丁（最有效的方案）
				Console.WriteLine("DEBUG: Starting aggressive text patcher...");
				AggressiveTextPatcher.StartAggressivePatching(process, 1000);
				Console.WriteLine("DEBUG: Aggressive text patcher started successfully");
			}
			catch (Exception ex)
			{
				Console.WriteLine("DEBUG: ERROR starting memory string patcher: " + ex.Message);
				Console.WriteLine("DEBUG: Stack trace: " + ex.StackTrace);
			}
			
			// 延迟关闭句柄，给内存补丁系统一些时间
			Thread.Sleep(2000);
			Console.WriteLine(string.Format("DEBUG: Note - Process handle {0} kept open for memory patching", num7));
			// 不立即关闭句柄，让内存补丁系统继续使用
			// Helper.CloseProcessHandle(num7);
			Console.WriteLine("DEBUG: Filtering windows for 'AutoVLBS 1.9'");
			SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "AutoVLBS 1.9");
			for (int i = 0; i < 100; i++)
			{
				SystemWindow[] array23 = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "Login");
				if (array23.Length != 0)
				{
					Console.WriteLine("DEBUG: Found Login window, filtering for 'Bo qua' button");
					array23[0].FilterDescendantWindows(false, (SystemWindow w) => w.Title == "Bo qua");
					for (int j = 0; j < 100; j++)
					{
						SystemWindow[] array24 = array23[0].FilterDescendantWindows(false, (SystemWindow w) => w.Title == "Bo qua");
						if (array24.Length != 0 && CrackVLBS19v3.IsWindowVisible(array24[0].HWnd))
						{
							Console.WriteLine(string.Format("DEBUG: Simulating clicks on 'Bo qua' button, HWND: {0}", array24[0].HWnd));
							CrackVLBS19v3.PostMessage(array24[0].HWnd, 513, 1, CrackVLBS19v3._mouseLparam);
							Thread.Sleep(50);
							CrackVLBS19v3.PostMessage(array24[0].HWnd, 514, 0, CrackVLBS19v3._mouseLparam);
							Thread.Sleep(50);
							CrackVLBS19v3.PostMessage(array24[0].HWnd, 513, 1, CrackVLBS19v3._mouseLparam);
							Thread.Sleep(50);
							CrackVLBS19v3.PostMessage(array24[0].HWnd, 514, 0, CrackVLBS19v3._mouseLparam);
							break;
						}
						Thread.Sleep(50);
					}
				}
				SystemWindow[] array25 = SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "AutoVLBS 1.9");
				if ((array25.Length != 0 & array23.Length == 0) && CrackVLBS19v3.IsWindowVisible(array25[0].HWnd))
				{
					break;
				}
				Thread.Sleep(50);
			}
			foreach (SystemWindow systemWindow in SystemWindow.FilterToplevelWindows((SystemWindow w) => w.Title == "New version!"))
			{
				Console.WriteLine(string.Format("DEBUG: Hiding 'New version!' window, HWND: {0}", systemWindow.HWnd));
				CrackVLBS19v3.ShowWindow(systemWindow.HWnd, 0);
			}
			for (int k = 0; k < 100; k++)
			{
				Thread.Sleep(100);
				if (CrackVLBS19v3.IsWindowVisible(process.MainWindowHandle))
				{
					ApiHelper.MoveWindowToTopRight(process.MainWindowHandle);
					Console.WriteLine("DEBUG: Setting main window title to: " + tittleNew);
					if (!ApiHelper.SetWindowText(process.MainWindowHandle, tittleNew))
					{
						Console.WriteLine(string.Format("DEBUG: Failed to set window title. Error: {0}", Marshal.GetLastWin32Error()));
					}
					IntPtr intPtr = Helper.FindWindowRecursive(process.MainWindowHandle, "Button", "agtool.net");
					Console.WriteLine(string.Format("DEBUG: Patching button 'agtool.net' to '剑侠1辅助', HWND: {0}", intPtr));
					CrackVLBS19v3.PatchButton(intPtr, "剑侠1辅助");
					IntPtr intPtr2 = Helper.FindWindowRecursive(process.MainWindowHandle, "Button", "§¨ng ký");
					Console.WriteLine(string.Format("DEBUG: Patching button '§¨ng ký' to 'hoiuc.g4vn.net', HWND: {0}", intPtr2));
					CrackVLBS19v3.PatchButton(intPtr2, "hoiuc.g4vn.net");
					
					// 启动界面中文补丁（延迟启动，等待窗口完全加载）
					Thread uiPatchThread = new Thread(() =>
					{
						Thread.Sleep(3000); // 等待3秒让窗口完全加载
						Console.WriteLine("DEBUG: Starting UI Chinese patcher for VLBS 1.9...");
						Console.WriteLine("DEBUG: Process state - HasExited: " + process.HasExited + ", MainWindowHandle: " + process.MainWindowHandle);
						try
						{
							UIChinesePatcher.PatchAllWindows(process, 50);
							Console.WriteLine("DEBUG: UIChinesePatcher.PatchAllWindows called successfully for VLBS 1.9");
						}
						catch (Exception ex)
						{
							Console.WriteLine("DEBUG: ERROR calling PatchAllWindows for VLBS 1.9: " + ex.Message);
							Console.WriteLine("DEBUG: Stack trace: " + ex.StackTrace);
						}
						
						try
						{
							UIChinesePatcher.StartContinuousPatching(process, 2000);
							Console.WriteLine("DEBUG: UIChinesePatcher.StartContinuousPatching called successfully for VLBS 1.9");
						}
						catch (Exception ex)
						{
							Console.WriteLine("DEBUG: ERROR calling StartContinuousPatching for VLBS 1.9: " + ex.Message);
							Console.WriteLine("DEBUG: Stack trace: " + ex.StackTrace);
						}
					})
					{
						IsBackground = true
					};
					uiPatchThread.Start();
					
					return;
				}
			}
		}

		// Token: 0x04000004 RID: 4
		private const int MOUSE_DOWN = 513;

		// Token: 0x04000005 RID: 5
		private const int MOUSE_UP = 514;

		// Token: 0x04000006 RID: 6
		private const uint WM_SETTEXT = 12U;

		// Token: 0x04000007 RID: 7
		private const uint BM_SETSTATE = 243U;

		// Token: 0x04000008 RID: 8
		private const int GWL_STYLE = -16;

		// Token: 0x04000009 RID: 9
		private const int GWL_WNDPROC = -4;

		// Token: 0x0400000A RID: 10
		private const int WS_DISABLED = 134217728;

		// Token: 0x0400000B RID: 11
		private static int _mouseLparam = 65537;

		// Token: 0x0400000C RID: 12
		private static uint _findWindowAAddress;

		// Token: 0x0400000D RID: 13
		private static byte _originalFindWindowByte;

		// Token: 0x0400000E RID: 14
		private static IntPtr _originalButtonWndProc;

		// Token: 0x0400000F RID: 15
		private static string _textToBlock;

		// Token: 0x0200000C RID: 12
		// (Invoke) Token: 0x06000066 RID: 102
		private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
	}
}
