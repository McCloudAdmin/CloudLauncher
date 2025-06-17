using CloudLauncher.forms.auth;
using CloudLauncher.utils;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using System.Data;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace CloudLauncher.forms.dashboard
{
    public partial class GameLaunch : Form
    {
        private readonly MinecraftPath _minecraftPath;
        private readonly MinecraftLauncher _launcher;
        private MSession _currentSession;
        private bool _isLaunching = false;
        private List<IVersionMetadata> _allVersions = new List<IVersionMetadata>();
        private List<string> _installedVersions = new List<string>();
        private bool _showRelease = true;
        private bool _showSnapshot = false;
        private bool _showAlpha = false;
        private bool _showBeta = false;

        private class CustomVersionMetadata : IVersionMetadata
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public DateTimeOffset ReleaseTime { get; set; }

            public Task<IVersion> GetVersionAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException("Custom versions cannot be downloaded");
            }

            public Task<IVersion> GetAndSaveVersionAsync(MinecraftPath path, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException("Custom versions cannot be downloaded");
            }

            public Task SaveVersionAsync(MinecraftPath path, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException("Custom versions cannot be downloaded");
            }
        }

        public GameLaunch(MSession session = null)
        {
            InitializeComponent();
            UIStyler.ApplyStyles(this, true);

            // Initialize Minecraft launcher
            _minecraftPath = new MinecraftPath();
            _launcher = new MinecraftLauncher(_minecraftPath);

            // Set default filter states
            cbReleaseVersion.Checked = true; // Release
            cbSnapshotVersion.Checked = false; // Snapshot
            cbAlphaVersion.Checked = false; // Alpha
            cbBetaVersion.Checked = false; // Beta

            // Enable owner drawing for the combo box
            dDVersions.DrawMode = DrawMode.OwnerDrawVariable;
            dDVersions.DropDownStyle = ComboBoxStyle.DropDownList;

            // add event handlers
            _launcher.FileProgressChanged += (sender, args) =>
            {
                if (pbStatus.InvokeRequired)
                {
                    pbStatus.Invoke(new Action(() =>
                    {
                        UpdateProgress(args.ProgressedTasks, args.TotalTasks, $"Installing {args.Name} ({args.ProgressedTasks}/{args.TotalTasks})");
                    }));
                    return;
                }
                UpdateProgress(args.ProgressedTasks, args.TotalTasks, $"Installing {args.Name} ({args.ProgressedTasks}/{args.TotalTasks})");
            };
            _launcher.ByteProgressChanged += (sender, args) =>
            {
                if (pbStatus.InvokeRequired)
                {
                    pbStatus.Invoke(new Action(() =>
                    {
                        UpdateProgress(args.ProgressedBytes, args.TotalBytes, $"Downloading: {FormatBytes(args.ProgressedBytes)} / {FormatBytes(args.TotalBytes)}");
                    }));
                    return;
                }
                UpdateProgress(args.ProgressedBytes, args.TotalBytes, $"Downloading: {FormatBytes(args.ProgressedBytes)} / {FormatBytes(args.TotalBytes)}");
            };

            // Set the session
            _currentSession = session;

            // Load versions and settings
            LoadVersions();
            LoadSettings();

            lblUsername.Text = _currentSession?.Username ?? "Guest";
            lblType.Text = _currentSession?.UserType == "msa" ? "(Premium)" : "(Cracked)";

            // Load user avatar from mc-heads.net
            if (!string.IsNullOrEmpty(_currentSession?.Username))
            {
                _ = LoadUserAvatarAsync(_currentSession.Username);
            }
            else
            {
                // Set default avatar for guest users
                pbUserProfile.Image = Properties.Resources.error;
            }
        }

        private void UpdateProgress(long current, long total, string status)
        {
            if (total > 0)
            {
                int percentage = (int)((current * 100) / total);
                pbStatus.Value = percentage;
                lblReady.Text = status;
                pbStatus.Enabled = true;
                lblReady.Enabled = true;
            }
            else
            {
                pbStatus.Enabled = false;
                lblReady.Enabled = false;
            }
        }

        private void ScanInstalledVersions()
        {
            _installedVersions.Clear();
            var versionsDir = _minecraftPath.Versions;
            if (Directory.Exists(versionsDir))
            {
                foreach (var dir in Directory.GetDirectories(versionsDir))
                {
                    var versionName = Path.GetFileName(dir);
                    if (File.Exists(Path.Combine(dir, $"{versionName}.json")))
                    {
                        _installedVersions.Add(versionName);
                    }
                }
            }
            Logger.Info($"Found {_installedVersions.Count} installed versions");
        }

        private async void LoadVersions()
        {
            try
            {
                // First scan for installed versions
                ScanInstalledVersions();

                // Then get available versions from the launcher
                var versionCollection = await _launcher.GetAllVersionsAsync();
                _allVersions = versionCollection.ToList();

                // Add installed versions that aren't in the official list
                foreach (var installedVersion in _installedVersions)
                {
                    if (!_allVersions.Any(v => v.Name == installedVersion))
                    {
                        // Create a custom version metadata for installed versions
                        var customVersion = new CustomVersionMetadata
                        {
                            Name = installedVersion,
                            Type = "CUSTOM",
                            ReleaseTime = DateTimeOffset.Now
                        };
                        _allVersions.Add(customVersion);
                    }
                }

                Logger.Info($"Loaded {_allVersions.Count} total versions");
                UpdateVersionList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load versions: {ex.Message}");
                Alert.Error("Failed to load Minecraft versions. Please check the logs for details.");
            }
        }

        private void UpdateVersionList()
        {
            dDVersions.Items.Clear();
            var filteredVersions = _allVersions.Where(v =>
            {
                // Always show installed versions
                if (_installedVersions.Contains(v.Name))
                {
                    return true;
                }

                // For non-installed versions, apply filters
                var type = v.Type.ToLower();
                if (type == "release" && _showRelease) return true;
                if (type == "snapshot" && _showSnapshot) return true;
                if (type == "old_alpha" && _showAlpha) return true;
                if (type == "old_beta" && _showBeta) return true;
                return false;
            }).ToList();

            Logger.Info($"Filtered to {filteredVersions.Count} versions");
            foreach (var version in filteredVersions)
            {
                dDVersions.Items.Add(version);
            }

            // Select the last used version if available
            string lastVersion = RegistryConfig.GetUserPreference<string>("LastVersion", null);
            if (!string.IsNullOrEmpty(lastVersion))
            {
                int index = dDVersions.Items.Cast<IVersionMetadata>().ToList().FindIndex(v => v.Name == lastVersion);
                if (index != -1)
                {
                    dDVersions.SelectedIndex = index;
                }
            }
            else if (dDVersions.Items.Count > 0)
            {
                dDVersions.SelectedIndex = 0;
            }
        }

        private void dDVersions_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            var version = (IVersionMetadata)dDVersions.Items[e.Index];
            var isInstalled = _installedVersions.Contains(version.Name);
            var versionPath = Path.Combine(_minecraftPath.Versions, version.Name);
            var isModified = isInstalled && File.Exists(Path.Combine(versionPath, "mods"));

            // Create a more detailed display text
            var displayText = new StringBuilder();
            displayText.Append(version.Name);

            // Add version type indicator
            if (!string.IsNullOrEmpty(version.Type))
            {
                displayText.Append($" [{version.Type.ToUpper()}]");
            }

            // Add status indicators
            if (isInstalled)
            {
                displayText.Append(" ✓"); // Installed checkmark
            }
            if (isModified)
            {
                displayText.Append(" 🔧"); // Modified indicator
            }

            // Draw the text with different colors
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(displayText.ToString(), e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void dDVersions_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0) return;

            var version = (IVersionMetadata)dDVersions.Items[e.Index];
            var isInstalled = _installedVersions.Contains(version.Name);
            var versionPath = Path.Combine(_minecraftPath.Versions, version.Name);
            var isModified = isInstalled && File.Exists(Path.Combine(versionPath, "mods"));

            // Create the same display text as in DrawItem
            var displayText = new StringBuilder();
            displayText.Append(version.Name);
            if (!string.IsNullOrEmpty(version.Type))
            {
                displayText.Append($" [{version.Type.ToUpper()}]");
            }
            if (isInstalled) displayText.Append(" ✓");
            if (isModified) displayText.Append(" 🔧");

            // Measure the text size
            var size = TextRenderer.MeasureText(displayText.ToString(), dDVersions.Font);
            e.ItemHeight = size.Height + 4; // Add some padding
        }

        private void cbVersion_CheckedChanged(object sender, EventArgs e)
        {
            _showRelease = cbReleaseVersion.Checked;
            _showSnapshot = cbSnapshotVersion.Checked;
            _showAlpha = cbAlphaVersion.Checked;
            _showBeta = cbBetaVersion.Checked;
            UpdateVersionList();
            RegistryConfig.SaveUserPreference("ShowRelease", _showRelease);
            RegistryConfig.SaveUserPreference("ShowSnapshot", _showSnapshot);
            RegistryConfig.SaveUserPreference("ShowAlpha", _showAlpha);
            RegistryConfig.SaveUserPreference("ShowBeta", _showBeta);
        }

        private void LoadSettings()
        {
            Logger.Info("Loading user settings...");
            try
            {
                // Load RAM setting
                int ram = RegistryConfig.GetUserPreference("RAM", 2048);
                tBRam.Value = ram;
                txtRam.Text = ram.ToString();
                Logger.Info($"Loaded RAM setting: {ram}MB");

                // Load custom arguments
                txtCustomArgs.Text = RegistryConfig.GetUserPreference<string>("CustomArgs", string.Empty);
                Logger.Info($"Loaded custom arguments: {txtCustomArgs.Text}");
                // Load Java path
                txtGameJavaPath.Text = RegistryConfig.GetUserPreference<string>("JavaPath", string.Empty);
                Logger.Info($"Loaded Java path: {txtGameJavaPath.Text}");
                // Load screen settings
                txtGameScreenHeight.Text = RegistryConfig.GetUserPreference("ScreenWidth", 1280).ToString();
                txtGameScreenHeight.Text = RegistryConfig.GetUserPreference("ScreenHeight", 720).ToString();
                cbFullScreen.Checked = RegistryConfig.GetUserPreference("FullScreen", false);
                Logger.Info($"Loaded screen settings: {txtGameScreenHeight.Text}x{txtGameScreenHeight.Text}, FullScreen: {cbFullScreen.Checked}");
                // Load server settings
                txtJoinServerIP.Text = RegistryConfig.GetUserPreference<string>("ServerIP", string.Empty);
                txtJoinServerPort.Text = RegistryConfig.GetUserPreference("ServerPort", string.Empty);
                Logger.Info($"Loaded server settings: {txtJoinServerIP.Text}:{txtJoinServerPort.Text}");

                // Load version filter settings
                _showRelease = RegistryConfig.GetUserPreference("ShowRelease", true);
                _showSnapshot = RegistryConfig.GetUserPreference("ShowSnapshot", false);
                _showAlpha = RegistryConfig.GetUserPreference("ShowAlpha", false);
                _showBeta = RegistryConfig.GetUserPreference("ShowBeta", false);
                Logger.Info($"Loaded version filter settings: Release: {_showRelease}, Snapshot: {_showSnapshot}, Alpha: {_showAlpha}, Beta: {_showBeta}");
                // Apply filter settings to checkboxes
                cbReleaseVersion.Checked = _showRelease;
                cbSnapshotVersion.Checked = _showSnapshot;
                cbAlphaVersion.Checked = _showAlpha;
                cbBetaVersion.Checked = _showBeta;
                Logger.Info($"Applied filter settings to checkboxes");
                // Load username from session or registry
                if (_currentSession != null)
                {
                    Logger.Info($"Loaded username from session: {_currentSession.Username}");
                }
                else
                {
                    string username = RegistryConfig.GetUserPreference<string>("LastUsername", null);
                    if (!string.IsNullOrEmpty(username))
                    {
                        Logger.Info($"Loaded username from registry: {username}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load settings: {ex.Message}");
                Alert.Error("Failed to load user settings. Please check the logs for details.");
            }
        }
        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }

        private bool ValidateNumericInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate screen width
            if (!int.TryParse(txtGameScreenHeight.Text, out int width) || width <= 0)
            {
                errorMessage = "Please enter a valid screen width.";
                return false;
            }

            // Validate screen height
            if (!int.TryParse(txtGameScreenHeight.Text, out int height) || height <= 0)
            {
                errorMessage = "Please enter a valid screen height.";
                return false;
            }

            // Validate server port
            if (!string.IsNullOrEmpty(txtJoinServerPort.Text))
            {
                if (!int.TryParse(txtJoinServerPort.Text, out int port) || port <= 0 || port > 65535)
                {
                    errorMessage = "Please enter a valid server port (1-65535).";
                    return false;
                }
            }

            return true;
        }

        private void tBRam_ValueChanged(object sender, EventArgs e)
        {
            txtRam.Text = tBRam.Value.ToString();
            RegistryConfig.SaveUserPreference("RAM", tBRam.Value);
        }

        private void GameLaunch_Load(object sender, EventArgs e)
        {
            // Save settings when form is loaded (in case of any default values)
            LoadSettings();
        }

        private void txtCustomArgs_TextChanged(object sender, EventArgs e)
        {
            // Save custom arguments when text changes
            RegistryConfig.SaveUserPreference("CustomArgs", txtCustomArgs.Text);
        }

        // Add event handlers for other settings that should be saved automatically
        private void txtGameScreenWidth_TextChanged(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("ScreenWidth", int.Parse(txtGameScreenHeight.Text));
        }

        private void txtGameScreenHeight_TextChanged(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("ScreenHeight", int.Parse(txtGameScreenHeight.Text));
        }

        private void cbFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("FullScreen", cbFullScreen.Checked);
        }

        private void txtJoinServerIP_TextChanged(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("ServerIP", txtJoinServerIP.Text);
        }

        private void txtJoinServerPort_TextChanged(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("ServerPort", txtJoinServerPort.Text);
        }

        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            if (_isLaunching)
                return;

            try
            {
                // Validate numeric inputs
                if (!ValidateNumericInputs(out string errorMessage))
                {
                    Alert.Warning(errorMessage);
                    return;
                }

                _isLaunching = true;
                btnStartGame.Enabled = false;
                btnStartGame.Text = "Launching...";
                pbStatus.Enabled = true;
                lblReady.Enabled = true;
                lblReady.Text = "Preparing to launch...";

                string IconPath = Path.Combine(Application.StartupPath, "assets", "logo.ico");

                var version = dDVersions.SelectedItem as IVersionMetadata;
                if (version == null)
                {
                    Alert.Warning("Please select a Minecraft version.");
                    return;
                }

                // Save selected version
                RegistryConfig.SaveUserPreference("LastVersion", version.Name);

                // Create launch options with enhanced settings
                var launchOption = new MLaunchOption
                {
                    Session = _currentSession ?? MSession.CreateOfflineSession("Player" + "_"),
                    MaximumRamMb = tBRam.Value,
                    MinimumRamMb = Math.Min(1024, tBRam.Value / 2), // Set minimum RAM to half of maximum, but not more than 1GB

                    // Java settings
                    JavaPath = txtGameJavaPath.Text.Trim(),

                    // Screen settings
                    ScreenWidth = int.Parse(txtGameScreenHeight.Text),
                    ScreenHeight = int.Parse(txtGameScreenHeight.Text),
                    FullScreen = cbFullScreen.Checked,

                    // Server connection if specified
                    ServerIp = txtJoinServerIP.Text.Trim(),
                    ServerPort = string.IsNullOrEmpty(txtJoinServerPort.Text) ? 25565 : int.Parse(txtJoinServerPort.Text),


                    // Launcher identification
                    VersionType = "CloudLauncher",
                    GameLauncherName = "CloudLauncher",
                    GameLauncherVersion = "1.0",

                    // Additional JVM arguments for better performance
                    ExtraJvmArguments = new MArgument[]
                    {
                        MArgument.FromCommandLine(txtCustomArgs.Text)
                    }
                };

                // Install and launch
                await _launcher.InstallAsync(version.Name);
                var process = await _launcher.CreateProcessAsync(version.Name, launchOption);
                process.Start();
                this.WindowState = FormWindowState.Minimized; // Minimize the launcher window

                Logger.Info($"Launched Minecraft {version.Name} with {launchOption.MaximumRamMb}MB RAM");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to launch game: {ex.Message}");
                Alert.Error("Failed to launch Minecraft. Please check the logs for details.");
            }
            finally
            {
                _isLaunching = false;
                btnStartGame.Enabled = true;
                btnStartGame.Text = "Launch";
                pbStatus.Visible = false;
                lblReady.Visible = false;
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Clear session and settings
            _currentSession = null;
            RegistryConfig.DeleteValue("LastUsername");
            RegistryConfig.DeleteValue("LastUUID");

            // Show login form
            using (var loginForm = new FrmLogin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Get the session from registry
                    string username = RegistryConfig.GetUserPreference<string>("LastUsername", null);
                    string uuid = RegistryConfig.GetUserPreference<string>("LastUUID", null);

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(uuid))
                    {
                        _currentSession = new MSession(username, uuid, "0");
                        LoadSettings();
                    }
                }
            }
        }

        private void btnPickJava_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Java Executable|javaw.exe|All Files|*.*";
                openFileDialog.Title = "Select Java Executable";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtGameJavaPath.Text = openFileDialog.FileName;
                    RegistryConfig.SaveUserPreference("JavaPath", txtGameJavaPath.Text);
                }
            }
        }

        private void txtRam_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(txtRam.Text, out int ram) && ram > 0)
            {
                tBRam.Value = ram;
                RegistryConfig.SaveUserPreference("RAM", ram);
            }
            else
            {
                Alert.Warning("Please enter a valid RAM value.");
                txtRam.Text = tBRam.Value.ToString(); // Reset to current trackbar value
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Pages.SetPage(PageHome);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Pages.SetPage(PageSettings);
        }

        private async Task LoadUserAvatarAsync(string username)
        {
            try
            {
                string avatarUrl = $"https://mc-heads.net/avatar/{username}";

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(avatarUrl);

                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = Image.FromStream(stream);

                        // Ensure we're on the UI thread when setting the image
                        if (pbUserProfile.InvokeRequired)
                        {
                            pbUserProfile.Invoke(new Action(() => pbUserProfile.Image = image));
                        }
                        else
                        {
                            pbUserProfile.Image = image;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Logger.Warning($"Failed to load avatar for user '{username}': {ex.Message}");

                // Set default error image on UI thread
                if (pbUserProfile.InvokeRequired)
                {
                    pbUserProfile.Invoke(new Action(() => pbUserProfile.Image = Properties.Resources.error));
                }
                else
                {
                    pbUserProfile.Image = Properties.Resources.error;
                }
            }
        }

        private void lblType_Click(object sender, EventArgs e)
        {

        }

        private void btnSettings2_Click(object sender, EventArgs e)
        {
            Pages.SetPage(PageSettings);
        }
    }
}
