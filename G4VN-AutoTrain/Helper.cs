using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace CrackVLBS
{
	// Token: 0x02000004 RID: 4
	internal class Helper
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00003F40 File Offset: 0x00003F40
		public static byte[] StringToByteArray(string inputString, bool addNullTerminator = true)
		{
			if (inputString != null && !(inputString == ""))
			{
				int num = inputString.Length;
				if (addNullTerminator)
				{
					num++;
				}
				byte[] array = new byte[num];
				if (inputString != "")
				{
					char[] array2 = inputString.ToCharArray();
					for (int i = 0; i < array2.Length; i++)
					{
						array[i] = (byte)array2[i];
					}
				}
				if (addNullTerminator)
				{
					array[num - 1] = 0;
				}
				return array;
			}
			if (!addNullTerminator)
			{
				return null;
			}
			return new byte[1];
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003FB4 File Offset: 0x00003FB4
		public static string ByteArrayToString(byte[] byteArray)
		{
			if (byteArray == null || byteArray.Length == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in byteArray)
			{
				if (b == 0)
				{
					break;
				}
				if (b >= 32 && b <= 126)
				{
					stringBuilder.Append((char)b);
				}
				else
				{
					stringBuilder.Append('.');
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00004010 File Offset: 0x00004010
		public static byte[] HexStringToByteArray(string hexString, bool allowWildcard = true)
		{
			hexString = hexString.Replace(" ", "");
			if (hexString.Length % 2 != 0 || hexString == "")
			{
				return null;
			}
			byte[] array = new byte[hexString.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				string text = hexString.Substring(i * 2, 2);
				array[i] = ((!allowWildcard || !(text == "??")) ? byte.Parse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture) : (byte)63);
			}
			return array;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004098 File Offset: 0x00004098
		public static string NumberToHexString(object value, int paddingLength = 8, bool addHexPrefix = true, bool reverseLittleEndian = false)
		{
			string text = "";
			string text2 = value.ToString();
			if (text2 == null || text2 == "")
			{
				text2 = "00";
			}
			try
			{
				text = uint.Parse(text2).ToString("x").ToUpper();
				goto IL_4E;
			}
			catch
			{
				goto IL_4E;
			}
			IL_42:
			text = "0" + text;
			IL_4E:
			if (text.Length < paddingLength)
			{
				goto IL_42;
			}
			if (reverseLittleEndian)
			{
				string text3 = "";
				if (text.Length % 2 != 0)
				{
					text = "0" + text;
				}
				while (text != "")
				{
					text3 += text.Substring(text.Length - 2, 2);
					text = text.Remove(text.Length - 2);
				}
				return text3;
			}
			if (addHexPrefix)
			{
				text = "0x" + text;
			}
			return text;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004174 File Offset: 0x00004174
		public static void AddStringToArray(ref string[] stringArray, string newString)
		{
			if (stringArray != null)
			{
				string[] array = new string[stringArray.Length + 1];
				for (int i = 0; i < stringArray.Length; i++)
				{
					if (stringArray[i] == newString)
					{
						return;
					}
					array[i] = stringArray[i];
				}
				array[array.Length - 1] = newString;
				stringArray = array;
				return;
			}
			stringArray = new string[]
			{
				newString
			};
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000041CC File Offset: 0x000041CC
		public static IntPtr FindWindowRecursive(IntPtr parentWindow, string className, string windowTitle)
		{
			IntPtr intPtr = ApiHelper.FindWindowEx(parentWindow, IntPtr.Zero, className, windowTitle);
			if (intPtr != IntPtr.Zero)
			{
				return intPtr;
			}
			IntPtr intPtr2 = ApiHelper.FindWindowEx(parentWindow, IntPtr.Zero, null, null);
			while (intPtr2 != IntPtr.Zero)
			{
				IntPtr intPtr3 = Helper.FindWindowRecursive(intPtr2, className, windowTitle);
				if (intPtr3 != IntPtr.Zero)
				{
					return intPtr3;
				}
				intPtr2 = ApiHelper.FindWindowEx(parentWindow, intPtr2, null, null);
			}
			return IntPtr.Zero;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000423C File Offset: 0x0000423C
		public static string GetWindowTextDebug(IntPtr windowHandle)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			ApiHelper.GetWindowText(windowHandle, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004268 File Offset: 0x00004268
		public static uint ReadMemoryUInt32(uint address, int processHandle, int bytesToRead = 4)
		{
			int num = 0;
			if (bytesToRead <= 0)
			{
				bytesToRead = 4;
			}
			byte[] array = new byte[bytesToRead];
			ApiHelper.ReadProcessMemory(processHandle, address, array, array.Length, ref num);
			if (bytesToRead < 4)
			{
				byte[] array2 = new byte[4];
				for (int i = 0; i < bytesToRead; i++)
				{
					array2[i] = array[i];
				}
				array = array2;
			}
			return BitConverter.ToUInt32(array, 0);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000042B9 File Offset: 0x000042B9
		public static uint AllocateMemory(int processHandle, uint size = 512U, int protection = 64)
		{
			return ApiHelper.VirtualAllocEx(processHandle, 0U, size, 12288, protection);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000042CC File Offset: 0x000042CC
		public static uint ReadProcessMemoryUInt32(uint address, int processHandle, int bytesToRead = 4)
		{
			int num = 0;
			if (bytesToRead <= 0)
			{
				bytesToRead = 4;
			}
			byte[] array = new byte[bytesToRead];
			ApiHelper.ReadProcessMemory(processHandle, address, array, array.Length, ref num);
			if (bytesToRead < 4)
			{
				byte[] array2 = new byte[4];
				for (int i = 0; i < bytesToRead; i++)
				{
					array2[i] = array[i];
				}
				array = array2;
			}
			return BitConverter.ToUInt32(array, 0);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004320 File Offset: 0x00004320
		public static bool WriteProcessMemoryUInt32(uint address, int processHandle, uint value, int bytesToWrite = -1)
		{
			int num = 0;
			byte[] array = BitConverter.GetBytes(value);
			int int_ = array.Length;
			if (bytesToWrite <= 0)
			{
				if (value >> 8 == 0U)
				{
					int_ = 1;
				}
				else if (value >> 16 == 0U)
				{
					int_ = 2;
				}
			}
			else
			{
				byte[] array2 = new byte[bytesToWrite];
				for (int i = 0; i < bytesToWrite; i++)
				{
					if (i < array.Length)
					{
						array2[i] = array[i];
					}
				}
				array = array2;
				int_ = bytesToWrite;
			}
			return ApiHelper.WriteProcessMemory(processHandle, address, array, int_, ref num);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004388 File Offset: 0x00004388
		public static void CloseProcessHandle(int processHandle)
		{
			try
			{
				ApiHelper.CloseHandle((IntPtr)processHandle);
			}
			catch
			{
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000043B8 File Offset: 0x000043B8
		public static uint GetModuleBaseAddress(int processId, string moduleName)
		{
			try
			{
				moduleName = moduleName.ToUpper();
				ProcessModuleCollection modules = Process.GetProcessById(processId).Modules;
				int count = modules.Count;
				for (int i = 0; i < count; i++)
				{
					if (modules[count - i - 1].ModuleName.ToUpper() == moduleName)
					{
						return (uint)((int)modules[count - i - 1].BaseAddress);
					}
				}
			}
			catch
			{
			}
			return 0U;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000443C File Offset: 0x0000443C
		public static Process StartProcess(string fileName, string workingDirectory = "", string arguments = "", byte waitMode = 0, bool hideWindow = false, bool useShellExecute = false)
		{
			Process process = new Process();
			Process result;
			try
			{
				process.StartInfo.FileName = fileName;
				if (workingDirectory != "")
				{
					process.StartInfo.WorkingDirectory = workingDirectory;
				}
				process.StartInfo.Arguments = arguments;
				if (hideWindow)
				{
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				}
				process.StartInfo.UseShellExecute = useShellExecute;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				if (waitMode <= 0)
				{
					result = process;
				}
				else
				{
					process.WaitForInputIdle();
					result = process;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000044E8 File Offset: 0x000044E8
		public static void SuspendProcess(Process process)
		{
			try
			{
				if (process.ProcessName != "")
				{
					foreach (object obj in process.Threads)
					{
						ProcessThread processThread = (ProcessThread)obj;
						IntPtr intPtr = ApiHelper.OpenThread(2, false, (uint)processThread.Id);
						if (!(intPtr == IntPtr.Zero))
						{
							ApiHelper.SuspendThread(intPtr);
							ApiHelper.CloseHandle(intPtr);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004588 File Offset: 0x00004588
		public static void ResumeProcess(Process process)
		{
			try
			{
				if (process.ProcessName != "")
				{
					foreach (object obj in process.Threads)
					{
						ProcessThread processThread = (ProcessThread)obj;
						IntPtr intPtr = ApiHelper.OpenThread(2, false, (uint)processThread.Id);
						if (!(intPtr == IntPtr.Zero))
						{
							while (ApiHelper.ResumeThread(intPtr) > 0)
							{
							}
							ApiHelper.CloseHandle(intPtr);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000462C File Offset: 0x0000462C
		public static bool IsProcessExited(Process process)
		{
			try
			{
				return process == null || process.HasExited;
			}
			catch
			{
			}
			return true;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004660 File Offset: 0x00004660
		public static void KillProcess(Process process)
		{
			if (process == null)
			{
				return;
			}
			try
			{
				process.Kill();
			}
			catch
			{
			}
		}

		// Token: 0x04000010 RID: 16
		public static bool IsApplicationStopped = false;

		// Token: 0x04000011 RID: 17
		public static string[] ErrorMessages;

		// Token: 0x04000012 RID: 18
		private const uint DEFAULT_BUFFER_SIZE = 512U;

		// Token: 0x04000013 RID: 19
		private const uint MEMORY_COMMIT = 4096U;

		// Token: 0x04000014 RID: 20
		private const uint MEMORY_RESERVE = 8192U;

		// Token: 0x04000015 RID: 21
		private const int MEMORY_EXECUTE_READWRITE = 64;

		// Token: 0x04000016 RID: 22
		private const int PROCESS_ALL_ACCESS = 2035711;

		// Token: 0x04000017 RID: 23
		private const int THREAD_SUSPEND_RESUME = 2;
	}
}
