using Newtonsoft.Json;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace GitHubReleaseUpdater
{
    public partial class MainForm : Form
    {
        GitHubClient githubClient;
        HttpClient httpClient;

        readonly string configPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "config.json");

        public MainForm()
        {
            InitializeComponent();
            InitializeEventHandler();
            LoadConfiguration();
            SetupClients();
        }

        void InitializeEventHandler()
        {
            btnManualUpdate.Click += BtnManualUpdate_Click;
            btnCheckUpdate.Click += BtnCheckUpdate_Click;
            btnForceUpdate.Click += BtnForceUpdate_Click;
        }

        void SetupClients()
        {
            githubClient = new GitHubClient(new ProductHeaderValue("GitHubReleaseUpdater"));
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubReleaseUpdater");
        }

        void LoadConfiguration()
        {
            if (!File.Exists(configPath))
                return;

            try
            {
                string json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<AppConfig>(json);
                if (config != null)
                {
                    txtInstallDirectory.Text = config.InstallDirectory;
                    txtRepoOwner.Text = config.RepoOwner;
                    txtRepoName.Text = config.RepoName;
                    txtCurrentVersion.Text = config.CurrentVersion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        async void BtnManualUpdate_Click(object sender, EventArgs e)
        {
            string owner = txtRepoOwner.Text.Trim();
            string repo = txtRepoName.Text.Trim();

            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
            {
                MessageBox.Show("Please provide the owner and the repository name.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string url = $"https://github.com/{owner}/{repo}/releases/latest";

            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open the browser: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async void BtnCheckUpdate_Click(object sender, EventArgs e)
        {
            await CheckAndUpdateAsync(promptIfNewer: true);
        }

        async void BtnForceUpdate_Click(object sender, EventArgs e)
        {
            await CheckAndUpdateAsync(promptIfNewer: false);
        }

        async Task CheckAndUpdateAsync(bool promptIfNewer)
        {
            if (!ValidateInputs())
                return;

            SetBusy(true);
            try
            {
                lblStatus.Text = "Retrieving the latest release...";
                var release = await githubClient.Repository.Release.GetLatest(txtRepoOwner.Text.Trim(), txtRepoName.Text.Trim());

                string latestVersion = release.TagName?.TrimStart('v') ?? release.Name;
                string currentVersion = txtCurrentVersion.Text.Trim();

                lblStatus.Text = $"Latest version found: {latestVersion}";

                if (promptIfNewer)
                {
                    bool isNewer = IsNewerVersion(currentVersion, latestVersion);
                    if (!isNewer)
                    {
                        lblStatus.Text = "No updates available.";
                        MessageBox.Show("You are already on the latest version.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var result = MessageBox.Show(
                        $"A new version ({latestVersion}) is available. Do you want to download and install it?",
                        "Update available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        lblStatus.Text = "Update cancelled.";
                        return;
                    }
                }

                await DownloadAndInstallAsync(release);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error.";
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        async Task DownloadAndInstallAsync(Release release)
        {
            ReleaseAsset asset = release.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                                ?? release.Assets.FirstOrDefault();

            if (asset == null)
            {
                MessageBox.Show("No archive file found in this release.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string downloadUrl = asset.BrowserDownloadUrl;
            string tempZipPath = Path.Combine(Path.GetTempPath(), $"{asset.Name}");

            lblStatus.Text = "Download in progress...";
            lblProgress.Text = "0%";
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Continuous;

            using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                long? totalBytes = response.Content.Headers.ContentLength;
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(tempZipPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    byte[] buffer = new byte[81920];
                    long totalRead = 0;
                    int bytesRead;
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                        if (totalBytes.HasValue)
                        {
                            int percent = (int)((double)totalRead / totalBytes.Value * 100);
                            progressBar.Value = Math.Min(percent, 100);
                            lblProgress.Text = $"{percent}% ({FormatBytes(totalRead)} / {FormatBytes(totalBytes.Value)})";
                        }
                        else
                        {
                            lblProgress.Text = $"{FormatBytes(totalRead)} downloaded";
                        }
                    }
                }
            }

            lblProgress.Text = "Download complete.";

            string tempExtractPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempExtractPath);

            lblStatus.Text = "Extracting the archive...";
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;

            await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath));

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
            lblProgress.Text = "Complete extraction.";

            string sourcePath = tempExtractPath;
            var rootEntries = Directory.GetFileSystemEntries(tempExtractPath);

            if (rootEntries.Length == 1 && Directory.Exists(rootEntries[0]))
            {
                sourcePath = rootEntries[0];
            }

            lblStatus.Text = "Installing files...";

            string installDir = txtInstallDirectory.Text.Trim();
            if (!Directory.Exists(installDir))
                Directory.CreateDirectory(installDir);

            await Task.Run(() => CopyDirectory(sourcePath, installDir, true));

            File.Delete(tempZipPath);
            Directory.Delete(tempExtractPath, true);

            txtCurrentVersion.Text = release.TagName?.TrimStart('v') ?? release.Name;

            lblStatus.Text = "Installation completed successfully..";
            lblProgress.Text = "100%";
            progressBar.Value = 100;

            MessageBox.Show("The update has been successfully installed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetFilePath, overwrite);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestDir = Path.Combine(destDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestDir, overwrite);
            }
        }

        bool IsNewerVersion(string current, string latest)
        {
            if (string.IsNullOrEmpty(current) || string.IsNullOrEmpty(latest))
                return true;

            string cur = current.Trim().TrimStart('v', 'V');
            string lat = latest.Trim().TrimStart('v', 'V');

            bool curParsed = Version.TryParse(cur, out Version curVersion);
            bool latParsed = Version.TryParse(lat, out Version latVersion);

            if (curParsed && latParsed)
                return latVersion > curVersion;

            return string.Compare(lat, cur, StringComparison.OrdinalIgnoreCase) > 0;
        }

        bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtInstallDirectory.Text))
            {
                MessageBox.Show("Please specify the installation directory.", "Missing field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtRepoOwner.Text) || string.IsNullOrWhiteSpace(txtRepoName.Text))
            {
                MessageBox.Show("Please provide the owner and the repository name.", "Missing field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCurrentVersion.Text))
            {
                MessageBox.Show("Please provide the current version.", "Missing field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        void SetBusy(bool busy)
        {
            btnManualUpdate.Enabled = !busy;
            btnCheckUpdate.Enabled = !busy;
            btnForceUpdate.Enabled = !busy;
            txtInstallDirectory.Enabled = !busy;
            txtRepoOwner.Enabled = !busy;
            txtRepoName.Enabled = !busy;
            txtCurrentVersion.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        string FormatBytes(long bytes)
        {
            string[] suffixes = { "o", "Ko", "Mo", "Go", "To" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < suffixes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {suffixes[order]}";
        }
    }

    public class AppConfig
    {
        public string InstallDirectory { get; set; }
        public string RepoOwner { get; set; }
        public string RepoName { get; set; }
        public string CurrentVersion { get; set; }
    }
}