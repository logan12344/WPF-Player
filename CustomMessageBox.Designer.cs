namespace Player
{
    partial class CustomMessageBox
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.msbContent = new System.Windows.Forms.Label();
            this.msbButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.msbContent);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 59);
            this.panel1.TabIndex = 0;
            // 
            // msbContent
            // 
            this.msbContent.AutoSize = true;
            this.msbContent.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.msbContent.ForeColor = System.Drawing.Color.White;
            this.msbContent.Location = new System.Drawing.Point(12, 18);
            this.msbContent.Name = "msbContent";
            this.msbContent.Size = new System.Drawing.Size(0, 18);
            this.msbContent.TabIndex = 0;
            // 
            // msbButton
            // 
            this.msbButton.BackColor = System.Drawing.Color.Black;
            this.msbButton.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.msbButton.ForeColor = System.Drawing.Color.White;
            this.msbButton.Location = new System.Drawing.Point(180, 65);
            this.msbButton.Name = "msbButton";
            this.msbButton.Size = new System.Drawing.Size(75, 33);
            this.msbButton.TabIndex = 1;
            this.msbButton.Text = "OK";
            this.msbButton.UseVisualStyleBackColor = false;
            this.msbButton.Click += new System.EventHandler(this.msbButton_Click);
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(445, 110);
            this.Controls.Add(this.msbButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CustomMessageBox";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label msbContent;
        private System.Windows.Forms.Button msbButton;
    }
}