using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CrackVLBS
{
	// Token: 0x02000002 RID: 2
	internal class ApiHelper
	{
		// Token: 0x06000001 RID: 1
		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		// Token: 0x06000002 RID: 2
		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out ApiHelper.RECT lpRect);

		// Token: 0x06000003 RID: 3 RVA: 0x00002050 File Offset: 0x00002050
		public static bool MoveWindowToTopRight(IntPtr hwnd)
		{
			bool result;
			try
			{
				Screen primaryScreen = Screen.PrimaryScreen;
				int width = primaryScreen.Bounds.Width;
				int height = primaryScreen.Bounds.Height;
				ApiHelper.RECT rect;
				if (!ApiHelper.GetWindowRect(hwnd, out rect))
				{
					result = false;
				}
				else
				{
					int num = rect.Right - rect.Left;
					int x = width - num;
					int y = 0;
					result = ApiHelper.SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, 69U);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error moving window: " + ex.Message);
				result = false;
			}
			return result;
		}

		// Token: 0x06000004 RID: 4
		[DllImport("kernel32.dll")]
		public static extern uint SuspendThread(IntPtr intptr_2);

		// Token: 0x06000005 RID: 5
		[DllImport("kernel32.dll")]
		public static extern int ResumeThread(IntPtr intptr_2);

		// Token: 0x06000006 RID: 6
		[DllImport("kernel32.dll")]
		public static extern int OpenProcess(int int_8, bool bool_0, int int_9);

		// Token: 0x06000007 RID: 7
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr intptr_2);

		// Token: 0x06000008 RID: 8 RVA: 0x000020EC File Offset: 0x000020EC
		internal static void CloseProcessHandle(int hProcess)
		{
			ApiHelper.CloseHandle((IntPtr)hProcess);
		}

		// Token: 0x06000009 RID: 9
		[DllImport("kernel32.dll")]
		public static extern uint VirtualAllocEx(int int_8, uint uint_0, uint uint_1, int enum2_0, int enum1_0);

		// Token: 0x0600000A RID: 10
		[DllImport("kernel32", CharSet = CharSet.Ansi)]
		public static extern uint GetProcAddress(uint uint_0, string string_0);

		// Token: 0x0600000B RID: 11
		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int int_8, uint uint_0, byte[] byte_0, int int_9, ref int int_10);

		// Token: 0x0600000C RID: 12
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(int int_8, uint uint_0, byte[] byte_0, int int_9, ref int int_10);

		// Token: 0x0600000D RID: 13
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenThread(int enum0_0, bool bool_0, uint uint_0);

		// Token: 0x0600000E RID: 14
		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool SetWindowText(IntPtr hwnd, string lpString);

		// Token: 0x0600000F RID: 15
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool IsWindow(IntPtr hWnd);

		// Token: 0x06000010 RID: 16
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		// Token: 0x06000011 RID: 17
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

		// Token: 0x06000011A RID: 17A
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsWindowEnabled(IntPtr hWnd);

		// Token: 0x06000012 RID: 18
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		// Token: 0x06000013 RID: 19
		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

		// Token: 0x06000014 RID: 20
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06000015 RID: 21
		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hWndParent, ApiHelper.EnumChildProc lpEnumFunc, IntPtr lParam);

		// Token: 0x06000016 RID: 22
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x04000001 RID: 1
		private const uint SWP_NOSIZE = 1U;

		// Token: 0x04000002 RID: 2
		private const uint SWP_NOZORDER = 4U;

		// Token: 0x04000003 RID: 3
		private const uint SWP_SHOWWINDOW = 64U;

		// Token: 0x0200000A RID: 10
		public struct RECT
		{
			// Token: 0x0400002D RID: 45
			public int Left;

			// Token: 0x0400002E RID: 46
			public int Top;

			// Token: 0x0400002F RID: 47
			public int Right;

			// Token: 0x04000030 RID: 48
			public int Bottom;
		}

		// Token: 0x0200000B RID: 11
		// (Invoke) Token: 0x06000062 RID: 98
		public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
	}
}
