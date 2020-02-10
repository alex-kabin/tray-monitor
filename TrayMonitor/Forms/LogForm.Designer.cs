using System.ComponentModel;
using System.Windows.Forms;

namespace TrayMonitor
{
    partial class LogForm
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                    this.columnHeader1, this.columnHeader2, this.columnHeader6, this.columnHeader3, this.columnHeader4,
                    this.columnHeader5
            });
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(6, 6);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(1479, 585);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Level";
            this.columnHeader2.Width = 54;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Thread";
            this.columnHeader6.Width = 24;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Logger";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Message";
            this.columnHeader4.Width = 440;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Details";
            this.columnHeader5.Width = 440;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1491, 597);
            this.Controls.Add(this.listView1);
            this.MaximizeBox = false;
            this.Name = "LogForm";
            this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}