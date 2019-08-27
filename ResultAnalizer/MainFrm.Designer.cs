namespace ResultAnalizer
{
    partial class MainFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.tbxResults = new System.Windows.Forms.TextBox();
            this.btnSplit = new System.Windows.Forms.Button();
            this.textDir = new System.Windows.Forms.TextBox();
            this.fsd = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(333, 24);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(12, 27);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(315, 20);
            this.txtFileName.TabIndex = 1;
            // 
            // ofd
            // 
            this.ofd.FileName = "openFileDialog1";
            // 
            // tbxResults
            // 
            this.tbxResults.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbxResults.Location = new System.Drawing.Point(18, 101);
            this.tbxResults.Multiline = true;
            this.tbxResults.Name = "tbxResults";
            this.tbxResults.ReadOnly = true;
            this.tbxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxResults.Size = new System.Drawing.Size(389, 435);
            this.tbxResults.TabIndex = 2;
            // 
            // btnSplit
            // 
            this.btnSplit.Location = new System.Drawing.Point(333, 59);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(75, 23);
            this.btnSplit.TabIndex = 0;
            this.btnSplit.Text = "Split To";
            this.btnSplit.UseVisualStyleBackColor = true;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // textDir
            // 
            this.textDir.Location = new System.Drawing.Point(12, 62);
            this.textDir.Name = "textDir";
            this.textDir.ReadOnly = true;
            this.textDir.Size = new System.Drawing.Size(315, 20);
            this.textDir.TabIndex = 1;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 552);
            this.Controls.Add(this.tbxResults);
            this.Controls.Add(this.textDir);
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnOpen);
            this.Name = "MainFrm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.TextBox tbxResults;
        private System.Windows.Forms.Button btnSplit;
        private System.Windows.Forms.TextBox textDir;
        private System.Windows.Forms.FolderBrowserDialog fsd;
    }
}

