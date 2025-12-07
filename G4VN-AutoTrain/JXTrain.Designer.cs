namespace CrackVLBS
{
	// Token: 0x02000005 RID: 5
	public partial class JXTrain : global::System.Windows.Forms.Form
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000508A File Offset: 0x0000508A
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000050AC File Offset: 0x000050AC
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::CrackVLBS.JXTrain));
			this.button_0 = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.linkLabel1 = new global::System.Windows.Forms.LinkLabel();
			this.button1 = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.button_0.Location = new global::System.Drawing.Point(15, 25);
			this.button_0.Name = "button_0";
			this.button_0.Size = new global::System.Drawing.Size(108, 23);
			this.button_0.TabIndex = 0;
			this.button_0.Text = "G4VN VLBS 1.9";
			this.button_0.UseVisualStyleBackColor = true;
			this.button_0.Click += new global::System.EventHandler(this.button_0_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(33, 9);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(72, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "剑侠1辅助";
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new global::System.Drawing.Point(12, 81);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new global::System.Drawing.Size(114, 13);
			this.linkLabel1.TabIndex = 4;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "https://hoiuc.g4vn.net";
			this.linkLabel1.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			this.button1.Location = new global::System.Drawing.Point(15, 55);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(108, 23);
			this.button1.TabIndex = 5;
			this.button1.Text = "G4VN VLBS 1.3";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(150, 101);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.linkLabel1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.button_0);
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.Name = "JXTrain";
			base.SizeGripStyle = global::System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "G4VN自动挂机";
			base.Load += new global::System.EventHandler(this.JXTrain_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000024 RID: 36
		private global::System.ComponentModel.IContainer components = null;

		// Token: 0x04000025 RID: 37
		private global::System.Windows.Forms.Button button_0;

		// Token: 0x04000026 RID: 38
		private global::System.Windows.Forms.Label label1;

		// Token: 0x04000027 RID: 39
		private global::System.Windows.Forms.LinkLabel linkLabel1;

		// Token: 0x04000028 RID: 40
		private global::System.Windows.Forms.Button button1;
	}
}
