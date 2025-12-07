using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CrackVLBS
{
	/// <summary>
	/// 内存字符串补丁类 - 通过内存补丁修改目标程序中的越南文字符串
	/// </summary>
	internal class MemoryStringPatcher
	{
		// 越南文到中文的翻译映射表（优先搜索较短的字符串，因为它们更容易找到）
		private static readonly Dictionary<string, string> MemoryTranslationMap = new Dictionary<string, string>
		{
			// 短字符串优先（更容易找到）
			{ "Tìm", "查找" },
			{ "S/L", "数量" },
			{ "N/L", "等级" },
			{ "Level", "等级" },
			{ "Login", "登录" },
			
			// 标签页
			{ "Tổng hợp", "综合" },
			{ "Đồ tẩu", "挂机" },
			{ "Thống kê", "统计" },
			{ "Bang hội", "帮会" },
			{ "Kỹ năng", "技能" },
			{ "Lọc đồ", "筛选" },
			{ "Phụ trợ", "辅助" },
			
			// 功能选项
			{ "Luyện công", "练功" },
			{ "Nhiệm vụ Dã tẩu", "日常任务" },
			{ "PT Nhiệm vụ", "PT任务" },
			{ "PT Nhóm", "PT任务" },
			{ "Túi vô", "背包" },
			{ "Tự vệ", "背包" },
			{ "Túi xếp cá", "鱼袋" },
			{ "Tự xếp đồ", "鱼袋" },
			{ "Nhặt trong thành", "城内拾取" },
			{ "Không nhặt vật phẩm (đen)", "不拾取物品(黑名单)" },
			{ "PT Tết", "春节PT" },
			{ "PT Tất cả", "春节PT" },
			{ "Danh vọng", "声望" },
			{ "Tín sứ", "信使" },
			{ "Ẩn Game", "隐藏游戏" },
			{ "Reset (>3h)", "重置(>3小时)" },
			
			// 按钮
			{ "Hành trang", "行囊" },
			{ "Đăng ký", "注册" },
			
			// 输入框标签
			{ "Thoát Game", "退出游戏" },
			{ "Tắt máy", "关闭电脑" },
			{ "Thoát Game (hh:mm)", "退出游戏 (时:分)" },
			{ "Tắt máy (hh:mm)", "关闭电脑 (时:分)" },
			
			// 统计信息
			{ "Về thành", "回城" },
			{ "Tử vong", "死亡" },
			{ "Thu nhặt", "拾取" },
			{ "Thu nhập", "拾取" },
			{ "Dã tẩu", "挂机" },
			{ "Nhiệm vụ", "任务" },
			
			// 表格标题
			{ "Tên nhân vật", "角色名" },
			{ "Số lượng", "数量" },
			{ "Niveau", "等级" },
		};

		/// <summary>
		/// 在目标进程内存中搜索并替换字符串
		/// </summary>
		public static void PatchMemoryStrings(Process process, int processHandle)
		{
			if (process == null || process.HasExited || processHandle == 0)
			{
				Console.WriteLine("MemoryStringPatcher: Invalid process or handle");
				return;
			}

			Thread patchThread = new Thread(() =>
			{
				Console.WriteLine("MemoryStringPatcher: Starting memory string patching, PID=" + process.Id + ", Handle=" + processHandle);
				
				// 等待进程完全加载
				Console.WriteLine("MemoryStringPatcher: Waiting 8 seconds for process to load...");
				Thread.Sleep(8000);
					Console.WriteLine("MemoryStringPatcher: Wait completed, starting search...");
					Console.WriteLine("MemoryStringPatcher: Checking process state...");
					Console.WriteLine("MemoryStringPatcher: Process HasExited = " + process.HasExited);
					
				try
				{
					// 检查进程句柄是否有效
					if (processHandle == 0)
					{
						Console.WriteLine("MemoryStringPatcher: ERROR - Invalid process handle!");
						return;
					}
					
					// 再次检查进程是否仍然运行
					if (process.HasExited)
					{
						Console.WriteLine("MemoryStringPatcher: ERROR - Process has exited!");
						return;
					}
					
					// 获取进程基址
					uint baseAddress = 0x00400000;
					try
					{
						baseAddress = (uint)((int)process.MainModule.BaseAddress);
						Console.WriteLine("MemoryStringPatcher: Got base address from MainModule: 0x" + baseAddress.ToString("X8"));
					}
					catch (Exception ex)
					{
						Console.WriteLine("MemoryStringPatcher: Could not get MainModule, using default: 0x00400000, Error: " + ex.Message);
						baseAddress = 0x00400000;
					}
					
					Console.WriteLine("MemoryStringPatcher: Base address = 0x" + baseAddress.ToString("X8"));
					
					// 先测试内存读取是否正常
					Console.WriteLine("MemoryStringPatcher: Testing memory read...");
					byte[] testBuffer = new byte[256];
					int testBytesRead = 0;
					bool testRead = ApiHelper.ReadProcessMemory(processHandle, baseAddress, testBuffer, 256, ref testBytesRead);
					if (testRead && testBytesRead > 0)
					{
						Console.WriteLine("MemoryStringPatcher: Memory read test SUCCESS - Read " + testBytesRead + " bytes from 0x" + baseAddress.ToString("X8"));
					}
					else
					{
						Console.WriteLine("MemoryStringPatcher: Memory read test FAILED - Cannot read from process memory!");
						Console.WriteLine("MemoryStringPatcher: This may indicate the process handle is invalid or the process has exited");
						return;
					}
					
					// 搜索内存范围（扩大到4MB，包含更多数据段）
					uint searchStart = baseAddress;
					uint searchEnd = baseAddress + 0x00400000; // 4MB范围
					
					Console.WriteLine("MemoryStringPatcher: Searching memory range 0x" + searchStart.ToString("X8") + " - 0x" + searchEnd.ToString("X8") + " (2MB)");
					Console.WriteLine("MemoryStringPatcher: Total translation entries: " + MemoryTranslationMap.Count);
					
					int totalPatched = 0;
					int searchCount = 0;
					
					// 遍历所有翻译映射
					foreach (var kvp in MemoryTranslationMap)
					{
						searchCount++;
						string vietnameseText = kvp.Key;
						string chineseText = kvp.Value;
						
						if (searchCount % 10 == 0)
						{
							Console.WriteLine("MemoryStringPatcher: Searching " + searchCount + "/" + MemoryTranslationMap.Count + " strings...");
						}
						
						// 搜索越南文字符串（可能有多处）
						int foundCount = 0;
						uint searchAddr = searchStart;
						
						// 搜索最多10个匹配项
						for (int i = 0; i < 10; i++)
						{
							uint foundAddress = SearchMemoryString(processHandle, searchAddr, searchEnd, vietnameseText);
							
							if (foundAddress != 0)
							{
								Console.WriteLine("MemoryStringPatcher: Found '" + vietnameseText + "' at 0x" + foundAddress.ToString("X8"));
								
								// 替换为中文
								if (PatchMemoryString(processHandle, foundAddress, vietnameseText, chineseText))
								{
									totalPatched++;
									foundCount++;
									Console.WriteLine("MemoryStringPatcher: Patched '" + vietnameseText + "' -> '" + chineseText + "' at 0x" + foundAddress.ToString("X8"));
								}
								
								// 继续搜索下一个匹配项
								searchAddr = foundAddress + 1;
							}
							else
							{
								break;
							}
						}
						
						if (foundCount == 0 && searchCount <= 5)
						{
							// 只输出前5个未找到的，避免日志太多
							Console.WriteLine("MemoryStringPatcher: Not found '" + vietnameseText + "' in memory");
						}
					}
					
					Console.WriteLine("MemoryStringPatcher: Search completed! Total patched " + totalPatched + " strings");
				}
				catch (Exception ex)
				{
					Console.WriteLine("MemoryStringPatcher Error: " + ex.Message + "\n" + ex.StackTrace);
				}
			})
			{
				IsBackground = true
			};
			patchThread.Start();
		}

		/// <summary>
		/// 在内存中搜索字符串
		/// </summary>
		private static uint SearchMemoryString(int processHandle, uint startAddress, uint endAddress, string searchString)
		{
			if (string.IsNullOrEmpty(searchString))
			{
				return 0;
			}

			try
			{
				// 尝试多种编码方式搜索
				byte[] searchBytes = null;
				
				// 首先尝试使用Helper.StringToByteArray（ANSI编码）
				searchBytes = Helper.StringToByteArray(searchString, false);
				
				// 如果失败，尝试UTF-8编码
				if (searchBytes == null || searchBytes.Length == 0)
				{
					try
					{
						searchBytes = Encoding.UTF8.GetBytes(searchString);
					}
					catch
					{
						searchBytes = null;
					}
				}
				
				if (searchBytes == null || searchBytes.Length == 0)
				{
					return 0;
				}

				// 每次读取64KB，但跳过不可读的内存区域
				uint chunkSize = 0x10000;
				byte[] buffer = new byte[chunkSize];
				uint address = startAddress;
				int readAttempts = 0;
				int successfulReads = 0;
				
				while (address < endAddress)
				{
					try
					{
						readAttempts++;
						int bytesRead = 0;
						uint readSize = Math.Min(chunkSize, endAddress - address);
						
						// 尝试读取内存
						bool readSuccess = ApiHelper.ReadProcessMemory(processHandle, address, buffer, (int)readSize, ref bytesRead);
						
						if (readSuccess && bytesRead > 0)
						{
							successfulReads++;
							
							// 在缓冲区中搜索
							for (int i = 0; i <= bytesRead - searchBytes.Length; i++)
							{
								bool found = true;
								for (int j = 0; j < searchBytes.Length; j++)
								{
									if (buffer[i + j] != searchBytes[j])
									{
										found = false;
										break;
									}
								}
								
								if (found)
								{
									return address + (uint)i;
								}
							}
							
							// 如果成功读取，继续下一个块
							address += chunkSize;
						}
						else
						{
							// 如果读取失败，跳过这个块（可能是不可读的内存区域）
							address += chunkSize;
						}
					}
					catch
					{
						// 读取错误，跳过这个块
						address += chunkSize;
						if (readAttempts % 1000 == 0)
						{
							// 每1000次尝试输出一次，避免日志太多
							Console.WriteLine("MemoryStringPatcher: Search progress - Address: 0x" + address.ToString("X8") + ", Successful reads: " + successfulReads + "/" + readAttempts);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("SearchMemoryString Error: " + ex.Message);
			}

			return 0;
		}

		/// <summary>
		/// 在内存地址处替换字符串
		/// </summary>
		private static bool PatchMemoryString(int processHandle, uint address, string oldString, string newString)
		{
			try
			{
				// 尝试多种编码方式
				byte[] oldBytes = null;
				byte[] newBytes = null;
				
				// 首先尝试使用Helper.StringToByteArray（ANSI编码，与搜索时一致）
				oldBytes = Helper.StringToByteArray(oldString, false);
				newBytes = Helper.StringToByteArray(newString, false);
				
				// 如果失败，尝试UTF-8编码
				if (oldBytes == null || oldBytes.Length == 0)
				{
					try
					{
						oldBytes = Encoding.UTF8.GetBytes(oldString);
					}
					catch
					{
						oldBytes = null;
					}
				}
				
				if (newBytes == null || newBytes.Length == 0)
				{
					try
					{
						newBytes = Encoding.UTF8.GetBytes(newString);
					}
					catch
					{
						newBytes = null;
					}
				}
				
				if (newBytes == null || newBytes.Length == 0)
				{
					return false;
				}
				
				// 如果新字符串比旧字符串长，需要检查是否有足够空间
				if (newBytes.Length > oldBytes.Length)
				{
					Console.WriteLine("MemoryStringPatcher: Warning - New string is longer than old string (" + newBytes.Length + " > " + oldBytes.Length + "), cannot patch at 0x" + address.ToString("X8"));
					return false;
				}
				
				// 如果新字符串较短，用0填充
				if (newBytes.Length < oldBytes.Length)
				{
					byte[] paddedBytes = new byte[oldBytes.Length];
					Array.Copy(newBytes, paddedBytes, newBytes.Length);
					// 剩余部分填充0
					for (int i = newBytes.Length; i < oldBytes.Length; i++)
					{
						paddedBytes[i] = 0;
					}
					newBytes = paddedBytes;
				}
				
				// 写入新字符串
				int bytesWritten = 0;
				if (ApiHelper.WriteProcessMemory(processHandle, address, newBytes, newBytes.Length, ref bytesWritten))
				{
					Console.WriteLine("MemoryStringPatcher: Successfully patched memory at 0x" + address.ToString("X8") + ", wrote " + bytesWritten + " bytes");
					return true;
				}
				else
				{
					Console.WriteLine("MemoryStringPatcher: Failed to write memory at 0x" + address.ToString("X8"));
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("PatchMemoryString Error: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 持续监控并补丁内存字符串（用于动态加载的字符串）
		/// </summary>
		public static void StartContinuousMemoryPatching(Process process, int processHandle, int intervalMs = 5000)
		{
			Thread monitorThread = new Thread(() =>
			{
				// 等待进程完全加载
				Thread.Sleep(15000);
				Console.WriteLine("MemoryStringPatcher: Continuous memory patching started, Handle=" + processHandle);
				
				int cycleCount = 0;
				while (!process.HasExited)
				{
					try
					{
						// 检查进程句柄是否有效
						if (processHandle == 0)
						{
							// 尝试重新打开进程句柄
							processHandle = ApiHelper.OpenProcess(2035711, false, process.Id);
							if (processHandle == 0)
							{
								Console.WriteLine("MemoryStringPatcher: Failed to reopen process handle");
								Thread.Sleep(intervalMs);
								continue;
							}
							Console.WriteLine("MemoryStringPatcher: Reopened process handle: " + processHandle);
						}
						
						// 重新搜索并补丁
						uint baseAddress = 0x00400000;
						try
						{
							baseAddress = (uint)((int)process.MainModule.BaseAddress);
						}
						catch
						{
							baseAddress = 0x00400000;
						}
						
						uint searchStart = baseAddress;
						uint searchEnd = baseAddress + 0x00400000; // 4MB范围（与初始搜索一致）
						
						int patchedCount = 0;
						foreach (var kvp in MemoryTranslationMap)
						{
							uint searchAddr = searchStart;
							// 搜索最多3个匹配项
							for (int i = 0; i < 3; i++)
							{
								uint foundAddress = SearchMemoryString(processHandle, searchAddr, searchEnd, kvp.Key);
								if (foundAddress != 0)
								{
									if (PatchMemoryString(processHandle, foundAddress, kvp.Key, kvp.Value))
									{
										patchedCount++;
									}
									searchAddr = foundAddress + 1;
								}
								else
								{
									break;
								}
							}
						}
						
						cycleCount++;
						if (patchedCount > 0 && cycleCount % 3 == 0)
						{
							Console.WriteLine("MemoryStringPatcher: Continuous patching cycle " + cycleCount + ", patched " + patchedCount + " strings");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("ContinuousMemoryPatching Error: " + ex.Message);
					}

					Thread.Sleep(intervalMs);
				}
				
				// 关闭进程句柄
				if (processHandle != 0)
				{
					Helper.CloseProcessHandle(processHandle);
				}
				
				Console.WriteLine("MemoryStringPatcher: Continuous memory patching ended");
			})
			{
				IsBackground = true
			};
			monitorThread.Start();
		}
	}
}
