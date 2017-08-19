namespace NetworkExample
{
    partial class MainWindow
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
            this.openClientButton = new System.Windows.Forms.Button();
            this.openServerButton = new System.Windows.Forms.Button();
            this.killAppButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openClientButton
            // 
            this.openClientButton.Location = new System.Drawing.Point(13, 13);
            this.openClientButton.Name = "openClientButton";
            this.openClientButton.Size = new System.Drawing.Size(146, 23);
            this.openClientButton.TabIndex = 0;
            this.openClientButton.Text = "Open Client";
            this.openClientButton.UseVisualStyleBackColor = true;
            this.openClientButton.Click += new System.EventHandler(this.OnOpenClientButtonClick);
            // 
            // openServerButton
            // 
            this.openServerButton.Location = new System.Drawing.Point(165, 13);
            this.openServerButton.Name = "openServerButton";
            this.openServerButton.Size = new System.Drawing.Size(146, 23);
            this.openServerButton.TabIndex = 1;
            this.openServerButton.Text = "Open Server";
            this.openServerButton.UseVisualStyleBackColor = true;
            this.openServerButton.Click += new System.EventHandler(this.OnOpenServerButtonClick);
            // 
            // killAppButton
            // 
            this.killAppButton.Location = new System.Drawing.Point(317, 12);
            this.killAppButton.Name = "killAppButton";
            this.killAppButton.Size = new System.Drawing.Size(146, 23);
            this.killAppButton.TabIndex = 2;
            this.killAppButton.Text = "Kill Application";
            this.killAppButton.UseVisualStyleBackColor = true;
            this.killAppButton.Click += new System.EventHandler(this.OnKillAppButtonClick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 46);
            this.Controls.Add(this.killAppButton);
            this.Controls.Add(this.openServerButton);
            this.Controls.Add(this.openClientButton);
            this.Name = "MainWindow";
            this.Text = "Network Example MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMainWindowClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openClientButton;
        private System.Windows.Forms.Button openServerButton;
        private System.Windows.Forms.Button killAppButton;
    }
}

