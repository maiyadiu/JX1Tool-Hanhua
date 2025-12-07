using System;
using System.IO;
using System.Text;

namespace CrackVLBS
{
	/// <summary>
	/// 多文本写入器 - 同时写入多个TextWriter
	/// </summary>
	internal class MultiTextWriter : TextWriter
	{
		private TextWriter[] writers;

		public MultiTextWriter(params TextWriter[] writers)
		{
			this.writers = writers;
		}

		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}

		public override void Write(char value)
		{
			foreach (var writer in writers)
			{
				try
				{
					writer.Write(value);
				}
				catch
				{
					// 忽略错误，继续写入其他writer
				}
			}
		}

		public override void Write(string value)
		{
			foreach (var writer in writers)
			{
				try
				{
					writer.Write(value);
				}
				catch
				{
					// 忽略错误，继续写入其他writer
				}
			}
		}

		public override void WriteLine(string value)
		{
			foreach (var writer in writers)
			{
				try
				{
					writer.WriteLine(value);
				}
				catch
				{
					// 忽略错误，继续写入其他writer
				}
			}
		}

		public override void Flush()
		{
			foreach (var writer in writers)
			{
				try
				{
					writer.Flush();
				}
				catch
				{
					// 忽略错误
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var writer in writers)
				{
					try
					{
						writer.Flush();
					}
					catch
					{
						// 忽略错误
					}
				}
			}
			base.Dispose(disposing);
		}
	}
}
