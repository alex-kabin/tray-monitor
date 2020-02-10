using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TrayMonitor
{
    partial class LogRecordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.loggerLabel = new System.Windows.Forms.Label();
            this.threadLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.messageRichTextBox = new System.Windows.Forms.RichTextBox();
            this.detailsRichTextBox = new System.Windows.Forms.RichTextBox();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.label4);
            panel1.Controls.Add(this.label3);
            panel1.Controls.Add(this.loggerLabel);
            panel1.Controls.Add(this.threadLabel);
            panel1.Controls.Add(this.timeLabel);
            panel1.Controls.Add(this.levelLabel);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(8, 8);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(560, 73);
            panel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Logger:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Thread:";
            // 
            // loggerLabel
            // 
            this.loggerLabel.AutoSize = true;
            this.loggerLabel.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loggerLabel.Location = new System.Drawing.Point(60, 46);
            this.loggerLabel.Name = "loggerLabel";
            this.loggerLabel.Size = new System.Drawing.Size(43, 15);
            this.loggerLabel.TabIndex = 0;
            this.loggerLabel.Text = "logger";
            // 
            // threadLabel
            // 
            this.threadLabel.AutoSize = true;
            this.threadLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.threadLabel.Location = new System.Drawing.Point(60, 27);
            this.threadLabel.Name = "threadLabel";
            this.threadLabel.Size = new System.Drawing.Size(44, 15);
            this.threadLabel.TabIndex = 0;
            this.threadLabel.Text = "thread";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timeLabel.Location = new System.Drawing.Point(6, 6);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(36, 17);
            this.timeLabel.TabIndex = 0;
            this.timeLabel.Text = "time";
            // 
            // levelLabel
            // 
            this.levelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.levelLabel.BackColor = System.Drawing.Color.Black;
            this.levelLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.levelLabel.ForeColor = System.Drawing.Color.White;
            this.levelLabel.Location = new System.Drawing.Point(460, 0);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Padding = new System.Windows.Forms.Padding(4);
            this.levelLabel.Size = new System.Drawing.Size(100, 32);
            this.levelLabel.TabIndex = 0;
            this.levelLabel.Text = "level";
            this.levelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.messageRichTextBox);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point(8, 81);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(6);
            groupBox1.Size = new System.Drawing.Size(560, 88);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Message";
            // 
            // messageRichTextBox
            // 
            this.messageRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageRichTextBox.Location = new System.Drawing.Point(6, 19);
            this.messageRichTextBox.Name = "messageRichTextBox";
            this.messageRichTextBox.ReadOnly = true;
            this.messageRichTextBox.Size = new System.Drawing.Size(548, 63);
            this.messageRichTextBox.TabIndex = 0;
            this.messageRichTextBox.Text = "";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.detailsRichTextBox);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(8, 169);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(6);
            groupBox2.Size = new System.Drawing.Size(560, 197);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Details";
            // 
            // detailsRichTextBox
            // 
            this.detailsRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailsRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsRichTextBox.Location = new System.Drawing.Point(6, 19);
            this.detailsRichTextBox.Name = "detailsRichTextBox";
            this.detailsRichTextBox.ReadOnly = true;
            this.detailsRichTextBox.Size = new System.Drawing.Size(548, 172);
            this.detailsRichTextBox.TabIndex = 0;
            this.detailsRichTextBox.Text = "";
            // 
            // LogRecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 374);
            this.Controls.Add(groupBox2);
            this.Controls.Add(groupBox1);
            this.Controls.Add(panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogRecordForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LogRecordForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label loggerLabel;
        private System.Windows.Forms.Label threadLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.RichTextBox messageRichTextBox;
        private System.Windows.Forms.RichTextBox detailsRichTextBox;
    }
}