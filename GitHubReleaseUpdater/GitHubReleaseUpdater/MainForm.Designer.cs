namespace GitHubReleaseUpdater
{
    partial class MainForm
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
        void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtInstallDirectory = new System.Windows.Forms.TextBox();
            this.txtRepoOwner = new System.Windows.Forms.TextBox();
            this.txtRepoName = new System.Windows.Forms.TextBox();
            this.txtCurrentVersion = new System.Windows.Forms.TextBox();
            this.btnManualUpdate = new System.Windows.Forms.Button();
            this.btnCheckUpdate = new System.Windows.Forms.Button();
            this.btnForceUpdate = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblInstall = new System.Windows.Forms.Label();
            this.lblOwner = new System.Windows.Forms.Label();
            this.lblRepo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInstallDirectory
            // 
            this.txtInstallDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtInstallDirectory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInstallDirectory.ForeColor = System.Drawing.Color.White;
            this.txtInstallDirectory.Location = new System.Drawing.Point(170, 16);
            this.txtInstallDirectory.Name = "txtInstallDirectory";
            this.txtInstallDirectory.Size = new System.Drawing.Size(282, 24);
            this.txtInstallDirectory.TabIndex = 0;
            // 
            // txtRepoOwner
            // 
            this.txtRepoOwner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtRepoOwner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRepoOwner.ForeColor = System.Drawing.Color.White;
            this.txtRepoOwner.Location = new System.Drawing.Point(170, 51);
            this.txtRepoOwner.Name = "txtRepoOwner";
            this.txtRepoOwner.Size = new System.Drawing.Size(282, 24);
            this.txtRepoOwner.TabIndex = 1;
            // 
            // txtRepoName
            // 
            this.txtRepoName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtRepoName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRepoName.ForeColor = System.Drawing.Color.White;
            this.txtRepoName.Location = new System.Drawing.Point(170, 86);
            this.txtRepoName.Name = "txtRepoName";
            this.txtRepoName.Size = new System.Drawing.Size(282, 24);
            this.txtRepoName.TabIndex = 2;
            // 
            // txtCurrentVersion
            // 
            this.txtCurrentVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.txtCurrentVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurrentVersion.ForeColor = System.Drawing.Color.White;
            this.txtCurrentVersion.Location = new System.Drawing.Point(170, 121);
            this.txtCurrentVersion.Name = "txtCurrentVersion";
            this.txtCurrentVersion.Size = new System.Drawing.Size(282, 24);
            this.txtCurrentVersion.TabIndex = 3;
            // 
            // btnManualUpdate
            // 
            this.btnManualUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnManualUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnManualUpdate.FlatAppearance.BorderSize = 0;
            this.btnManualUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualUpdate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualUpdate.Location = new System.Drawing.Point(13, 177);
            this.btnManualUpdate.Name = "btnManualUpdate";
            this.btnManualUpdate.Size = new System.Drawing.Size(210, 33);
            this.btnManualUpdate.TabIndex = 4;
            this.btnManualUpdate.Text = "Update manually";
            this.btnManualUpdate.UseVisualStyleBackColor = false;
            // 
            // btnCheckUpdate
            // 
            this.btnCheckUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnCheckUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckUpdate.FlatAppearance.BorderSize = 0;
            this.btnCheckUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckUpdate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckUpdate.Location = new System.Drawing.Point(242, 177);
            this.btnCheckUpdate.Name = "btnCheckUpdate";
            this.btnCheckUpdate.Size = new System.Drawing.Size(210, 33);
            this.btnCheckUpdate.TabIndex = 5;
            this.btnCheckUpdate.Text = "Check for updates";
            this.btnCheckUpdate.UseVisualStyleBackColor = false;
            // 
            // btnForceUpdate
            // 
            this.btnForceUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnForceUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnForceUpdate.FlatAppearance.BorderSize = 0;
            this.btnForceUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForceUpdate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnForceUpdate.Location = new System.Drawing.Point(13, 226);
            this.btnForceUpdate.Name = "btnForceUpdate";
            this.btnForceUpdate.Size = new System.Drawing.Size(439, 33);
            this.btnForceUpdate.TabIndex = 6;
            this.btnForceUpdate.Text = "To update";
            this.btnForceUpdate.UseVisualStyleBackColor = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 269);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(439, 15);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.LightGray;
            this.lblStatus.Location = new System.Drawing.Point(13, 297);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(49, 14);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Ready.";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.ForeColor = System.Drawing.Color.LightGray;
            this.lblProgress.Location = new System.Drawing.Point(12, 328);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 14);
            this.lblProgress.TabIndex = 9;
            // 
            // lblInstall
            // 
            this.lblInstall.AutoSize = true;
            this.lblInstall.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstall.Location = new System.Drawing.Point(13, 21);
            this.lblInstall.Name = "lblInstall";
            this.lblInstall.Size = new System.Drawing.Size(151, 14);
            this.lblInstall.TabIndex = 10;
            this.lblInstall.Text = "Installation directory:";
            // 
            // lblOwner
            // 
            this.lblOwner.AutoSize = true;
            this.lblOwner.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOwner.Location = new System.Drawing.Point(13, 56);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(97, 14);
            this.lblOwner.TabIndex = 11;
            this.lblOwner.Text = "Depot owner:";
            // 
            // lblRepo
            // 
            this.lblRepo.AutoSize = true;
            this.lblRepo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRepo.Location = new System.Drawing.Point(13, 91);
            this.lblRepo.Name = "lblRepo";
            this.lblRepo.Size = new System.Drawing.Size(123, 14);
            this.lblRepo.TabIndex = 12;
            this.lblRepo.Text = "Repository name:";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(13, 126);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(115, 14);
            this.lblVersion.TabIndex = 13;
            this.lblVersion.Text = "Current version:";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(465, 360);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblRepo);
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.lblInstall);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnForceUpdate);
            this.Controls.Add(this.btnCheckUpdate);
            this.Controls.Add(this.btnManualUpdate);
            this.Controls.Add(this.txtCurrentVersion);
            this.Controls.Add(this.txtRepoName);
            this.Controls.Add(this.txtRepoOwner);
            this.Controls.Add(this.txtInstallDirectory);
            this.Font = new System.Drawing.Font("Verdana", 10F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GitHub Release Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TextBox txtInstallDirectory;
        private System.Windows.Forms.TextBox txtRepoOwner;
        private System.Windows.Forms.TextBox txtRepoName;
        private System.Windows.Forms.TextBox txtCurrentVersion;
        private System.Windows.Forms.Button btnManualUpdate;
        private System.Windows.Forms.Button btnCheckUpdate;
        private System.Windows.Forms.Button btnForceUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblInstall;
        private System.Windows.Forms.Label lblOwner;
        private System.Windows.Forms.Label lblRepo;
        private System.Windows.Forms.Label lblVersion;
    }
}

