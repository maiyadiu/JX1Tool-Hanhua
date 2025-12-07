using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CrackVLBS
{
	/// <summary>
	/// 文本绘制Hook类 - 通过Hook文本绘制API来替换越南文
	/// </summary>
	internal class TextDrawHook
	{
		// 越南文到中文的翻译映射表
		private static readonly Dictionary<string, string> TranslationMap = new Dictionary<string, string>
		{
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
			{ "Tìm", "查找" },
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
			{ "Level", "等级" },
			{ "S/L", "数量" },
			{ "N/L", "等级" },
		};

		// Hook相关
		private static IntPtr _originalDrawTextA = IntPtr.Zero;
		private static IntPtr _originalDrawTextW = IntPtr.Zero;
		private static IntPtr _originalTextOutA = IntPtr.Zero;
		private static IntPtr _originalTextOutW = IntPtr.Zero;
		private static IntPtr _originalExtTextOutA = IntPtr.Zero;
		private static IntPtr _originalExtTextOutW = IntPtr.Zero;
		private static bool _hooked = false;

		// API声明
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat);

		[DllImport("gdi32.dll", CharSet = CharSet.Ansi)]
		private static extern bool TextOutA(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		private static extern bool TextOutW(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		/// <summary>
		/// 翻译文本
		/// </summary>
		private static string TranslateText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			// 精确匹配
			if (TranslationMap.ContainsKey(text))
			{
				return TranslationMap[text];
			}

			// 部分匹配（处理带冒号的情况）
			foreach (var kvp in TranslationMap)
			{
				if (text.Contains(kvp.Key) || kvp.Key.Contains(text))
				{
					// 如果原文本有冒号，保留冒号
					if (text.Contains(":") && !kvp.Value.Contains(":"))
					{
						return kvp.Value + ":";
					}
					return kvp.Value;
				}
			}

			return text;
		}

		/// <summary>
		/// Hook DrawTextA
		/// </summary>
		private static int HookedDrawTextA(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat)
		{
			try
			{
				if (!string.IsNullOrEmpty(lpString) && nCount > 0)
				{
					string text = lpString.Substring(0, Math.Min(nCount, lpString.Length));
					string translated = TranslateText(text);
					if (translated != text)
					{
						Console.WriteLine("TextDrawHook: DrawTextA translated '" + text + "' -> '" + translated + "'");
						return DrawText(hDC, translated, translated.Length, ref lpRect, uFormat);
					}
				}
			}
			catch
			{
				// 忽略错误，使用原始文本
			}

			// 调用原始函数
			return DrawText(hDC, lpString, nCount, ref lpRect, uFormat);
		}

		/// <summary>
		/// Hook TextOutA
		/// </summary>
		private static bool HookedTextOutA(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString)
		{
			try
			{
				if (!string.IsNullOrEmpty(lpString) && cbString > 0)
				{
					string text = lpString.Substring(0, Math.Min(cbString, lpString.Length));
					string translated = TranslateText(text);
					if (translated != text)
					{
						Console.WriteLine("TextDrawHook: TextOutA translated '" + text + "' -> '" + translated + "'");
						return TextOutA(hdc, nXStart, nYStart, translated, translated.Length);
					}
				}
			}
			catch
			{
				// 忽略错误，使用原始文本
			}

			// 调用原始函数
			return TextOutA(hdc, nXStart, nYStart, lpString, cbString);
		}

		/// <summary>
		/// 安装Hook到目标进程
		/// </summary>
		public static void InstallHook(System.Diagnostics.Process process)
		{
			if (_hooked || process == null || process.HasExited)
			{
				Console.WriteLine("TextDrawHook: Cannot install hook - process invalid or already hooked");
				return;
			}

			Console.WriteLine("TextDrawHook: Installing hooks for process PID=" + process.Id);
			
			// 注意：实际的Hook安装需要使用DLL注入
			// 这里只是占位实现，真正的Hook需要在DLL中实现
			// 由于C#的限制，我们需要创建一个C++ DLL来实现Hook
			
			Console.WriteLine("TextDrawHook: Hook installation requires DLL injection - not implemented in C#");
			Console.WriteLine("TextDrawHook: Alternative: Using SetWindowText hook via message interception");
			
			// 使用消息拦截作为替代方案
			StartMessageInterception(process);
		}

		/// <summary>
		/// 通过消息拦截来替换文本（替代方案）
		/// </summary>
		private static void StartMessageInterception(System.Diagnostics.Process process)
		{
			Thread interceptThread = new Thread(() =>
			{
				Console.WriteLine("TextDrawHook: Starting message interception thread");
				
				while (!process.HasExited)
				{
					try
					{
						// 定期检查窗口文本并替换
						if (process.MainWindowHandle != IntPtr.Zero)
						{
							// 这里可以遍历窗口并替换文本
							// 但更有效的方法是Hook SetWindowText
						}
					}
					catch
					{
						// 忽略错误
					}

					Thread.Sleep(1000);
				}

				Console.WriteLine("TextDrawHook: Message interception thread ended");
			})
			{
				IsBackground = true
			};
			interceptThread.Start();
		}
	}
}
