using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestartSession
{
    public partial class MainForm : Form
    {
        string AppStart = Path.Combine(Application.StartupPath, "EnvironmentWebview");
        readonly string RestartFile = Path.Combine(Application.StartupPath, "restartsession.oob");
        string IdSession = "";

        public MainForm()
        {
            InitializeComponent();

            InputSessionName.GotFocus += new EventHandler(InputSessionName_GotFocus);
            InputSessionName.LostFocus += new EventHandler(InputSessionName_LostFocus);
        }

        async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadPathAsync(AppStart);
        }

        void InputSessionName_GotFocus(object sender, EventArgs e)
        {
            if (InputSessionName.Text == "Choose a session name...")
                InputSessionName.Text = "";
        }

        void InputSessionName_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputSessionName.Text))
                InputSessionName.Text = "Choose a session name...";
        }

        async Task LoadPathAsync(string path)
        {
            try
            {
                SessionPathList.Items.Clear();

                if (!Directory.Exists(path))
                {
                    MessageBox.Show("The EnvironmentWebview directory does not exist! You can continue; it will be created automatically.",
                        "Not exist", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var rep = await Task.Run(() => Directory.GetDirectories(path));

                foreach (var doss in rep)
                {
                    SessionPathList.Items.Add(Path.GetFileName(doss));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void SessionPathList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SessionPathList.SelectedIndex != -1)
            {
                IdSession = Path.Combine(AppStart, SessionPathList.SelectedItem.ToString(), "WebData");
            }
        }

        void CreateRestartFile()
        {
            try
            {
                if (SessionPathList.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a session first.", "Select", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (StreamWriter file_create = new StreamWriter(RestartFile))
                {
                    file_create.Write(IdSession);
                }
                MessageBox.Show("Upon the next restart, Ostium will use the selected session. This behavior occurs only once; " +
                    "to repeat the process, you must configure the restart for the desired session again. Otherwise, Ostium will " +
                    "restart using a new, unique session. You can reuse the sessions as long as you do not delete them.", "Restart session",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        async void SelectPathEnvBtn_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fB = new FolderBrowserDialog()
                {
                    RootFolder = Environment.SpecialFolder.Desktop,
                    Description = "Select directory"
                };
                fB.ShowDialog();
                if (string.IsNullOrEmpty(fB.SelectedPath))
                    return;
                else
                {
                    if (fB.SelectedPath != Path.Combine(fB.SelectedPath, "EnvironmentWebview"))
                    {
                        string addEnv = Path.Combine(fB.SelectedPath, "EnvironmentWebview");
                        Directory.CreateDirectory(addEnv);
                        EnvWebviewPATH.Text = addEnv;
                    }
                    else
                        EnvWebviewPATH.Text = fB.SelectedPath;

                    AppStart = EnvWebviewPATH.Text;
                    await LoadPathAsync(EnvWebviewPATH.Text);
                }

                fB.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        async void CreateSessionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (InputSessionName.Text == "Choose a session name..." || string.IsNullOrEmpty(InputSessionName.Text))
                    return;

                if (!Directory.Exists(Path.Combine(AppStart, InputSessionName.Text)))
                {
                    Directory.CreateDirectory(Path.Combine(AppStart, InputSessionName.Text));

                    await LoadPathAsync(AppStart);

                    InputSessionName.Text = "Choose a session name...";
                }
                else
                {
                    MessageBox.Show("The name already exists!", "Name exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        async void RestartSessionBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() => CreateRestartFile());
        }

        void CancelRestartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(RestartFile))
                {
                    File.Delete(RestartFile);
                    MessageBox.Show("The session restart was cancelled.", "Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
