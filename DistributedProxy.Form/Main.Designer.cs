﻿namespace DistributedProxy.Form
{
    partial class Main
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
            this.proxyToggleBtn = new System.Windows.Forms.Button();
            this.runProxyLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // proxyToggleBtn
            // 
            this.proxyToggleBtn.Location = new System.Drawing.Point(12, 25);
            this.proxyToggleBtn.Name = "proxyToggleBtn";
            this.proxyToggleBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.proxyToggleBtn.Size = new System.Drawing.Size(100, 23);
            this.proxyToggleBtn.TabIndex = 0;
            this.proxyToggleBtn.Text = "Run Proxy";
            this.proxyToggleBtn.UseVisualStyleBackColor = true;
            this.proxyToggleBtn.Click += new System.EventHandler(this.proxyToggleBtn_Click);
            // 
            // runProxyLbl
            // 
            this.runProxyLbl.AutoSize = true;
            this.runProxyLbl.Location = new System.Drawing.Point(12, 9);
            this.runProxyLbl.Name = "runProxyLbl";
            this.runProxyLbl.Size = new System.Drawing.Size(89, 13);
            this.runProxyLbl.TabIndex = 1;
            this.runProxyLbl.Text = "Proxy not running";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.runProxyLbl);
            this.Controls.Add(this.proxyToggleBtn);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button proxyToggleBtn;
        private System.Windows.Forms.Label runProxyLbl;
    }
}
