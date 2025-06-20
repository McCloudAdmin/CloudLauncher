using CloudLauncher.forms.dashboard;
using CloudLauncher.utils;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using System.Data;
using XboxAuthNet.Game.Accounts;

namespace CloudLauncher.forms.auth
{
    public partial class FrmLogin : Form
    {
        private JELoginHandler loginHandler;
        private bool isAuthenticating = false;
        private List<IXboxGameAccount> savedAccounts = new List<IXboxGameAccount>();
        public bool IsLoggingOut { get; private set; }

        public FrmLogin()
        {
            InitializeComponent();
            UIStyler.ApplyStyles(this, true);
            Logger.Info("Login form initialized");
            InitializeLoginHandler();
            InitializeAccountSelector();
        }

        private void InitializeLoginHandler()
        {
            try
            {
                loginHandler = JELoginHandlerBuilder.BuildDefault();
                Logger.Info("Login handler initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize login handler: {ex.Message}");
                Alert.Error("Failed to initialize login system. Please check the logs for details.");
            }
        }

        private void InitializeAccountSelector()
        {
            try
            {
                // Add offline mode option
                dropAccounts.Items.Clear();
                dropAccounts.Items.Add("Offline Mode");
                dropAccounts.SelectedIndex = 0;

                // Load offline accounts from registry
                string offlineAccounts = RegistryConfig.GetUserPreference<string>("OfflineAccounts", string.Empty);
                if (!string.IsNullOrEmpty(offlineAccounts))
                {
                    foreach (string username in offlineAccounts.Split(','))
                    {
                        if (!string.IsNullOrEmpty(username))
                        {
                            dropAccounts.Items.Add($"{username} (Offline)");
                        }
                    }
                }

                // Load online accounts
                var accounts = loginHandler.AccountManager.GetAccounts();
                foreach (var account in accounts)
                {
                    savedAccounts.Add(account);
                    // Try to get the username from the account
                    string displayName = account.Identifier;
                    try
                    {
                        var session = loginHandler.AuthenticateSilently(account).GetAwaiter().GetResult();
                        if (session != null)
                        {
                            displayName = $"{session.Username} (Premium)";
                        }
                    }
                    catch
                    {
                        // If we can't get the username, just use the identifier
                    }
                    dropAccounts.Items.Add(displayName);
                }

                // Select the last used account if available
                string lastUsername = RegistryConfig.GetUserPreference<string>("LastUsername");
                if (!string.IsNullOrEmpty(lastUsername))
                {
                    // Try to find exact match first
                    int index = dropAccounts.Items.IndexOf(lastUsername);
                    if (index == -1)
                    {
                        // Try to find match with (Offline) or (Premium) suffix
                        index = dropAccounts.Items.IndexOf($"{lastUsername} (Offline)");
                        if (index == -1)
                        {
                            index = dropAccounts.Items.IndexOf($"{lastUsername} (Premium)");
                        }
                    }
                    if (index != -1)
                    {
                        dropAccounts.SelectedIndex = index;
                    }
                }
                else
                {
                    dropAccounts.SelectedIndex = 0;
                }

                // Restore keep logged in state
                cbKeepLogin.Checked = RegistryConfig.GetUserPreference("KeepLoggedIn", false);

                Logger.Info($"Loaded {accounts.Count} saved accounts");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading accounts: {ex.Message}");
                Alert.Error("Failed to load saved accounts. Please try again.");
            }
        }

        private void HandleSuccessfulLogin(MSession session)
        {
            if (session != null)
            {
                Logger.Info($"Successfully authenticated as {session.Username}");

                // Save the account info to registry
                RegistryConfig.SaveUserPreference("LastUsername", session.Username);
                RegistryConfig.SaveUserPreference("LastUUID", session.UUID);
                RegistryConfig.SaveUserPreference("KeepLoggedIn", cbKeepLogin.Checked);
                RegistryConfig.SaveUserPreference("WasOffline", session.AccessToken == "0"); // "0" indicates offline mode


                // Create and show GameLaunch form with the session
                GameLaunch gameLaunch = new GameLaunch(session);
                gameLaunch.FormClosed += (s, args) =>
                {
                    if (!IsLoggingOut) // Only show login form if not logging out
                    {
                        this.Show();
                        InitializeAccountSelector(); // Refresh the account list

                    }
                };
                gameLaunch.Show();
                this.Hide(); // Hide the login form
            }
        }

        private void dropAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show/hide username textbox based on offline mode selection
            txtOfflineName.Enabled = dropAccounts.SelectedIndex == 0;
            lblOfflineUsername.Enabled = dropAccounts.SelectedIndex == 0;
            btnRemove.Enabled = dropAccounts.SelectedIndex > 0;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (isAuthenticating)
                return;

            try
            {
                isAuthenticating = true;
                btnLogin.Enabled = false;
                btnLogin.Text = "Authenticating...";

                MSession session = null;

                if (dropAccounts.SelectedIndex == 0) // Offline Mode
                {
                    string username = txtOfflineName.Text.Trim();
                    if (string.IsNullOrEmpty(username))
                    {
                        Alert.Warning("Please enter a username for offline mode.");
                        return;
                    }

                    session = MSession.CreateOfflineSession(username);
                    Logger.Info($"Created offline session for user: {username}");

                    // Save offline account
                    string offlineAccounts = RegistryConfig.GetUserPreference<string>("OfflineAccounts", string.Empty);
                    if (!offlineAccounts.Contains(username))
                    {
                        offlineAccounts = string.IsNullOrEmpty(offlineAccounts) ? username : offlineAccounts + "," + username;
                        RegistryConfig.SaveUserPreference("OfflineAccounts", offlineAccounts);
                    }
                }
                else
                {
                    string selectedItem = dropAccounts.SelectedItem.ToString();
                    if (selectedItem.EndsWith("(Offline)"))
                    {
                        // Handle offline account selection
                        string username = selectedItem.Replace(" (Offline)", "");
                        session = MSession.CreateOfflineSession(username);
                        Logger.Info($"Created offline session for saved user: {username}");
                    }
                    else
                    {
                        // Try to authenticate with the selected account
                        try
                        {
                            var selectedAccount = savedAccounts[dropAccounts.SelectedIndex - 1];
                            session = await loginHandler.AuthenticateSilently(selectedAccount);
                        }
                        catch (Exception)
                        {
                            // If silent auth fails, try interactive login
                            Logger.Info("Silent authentication failed, trying interactive login");
                            session = await loginHandler.AuthenticateInteractively();
                        }
                    }
                }

                HandleSuccessfulLogin(session);
            }
            catch (Exception ex)
            {
                Logger.Error($"Authentication failed: {ex.Message}");
                Alert.Error("Authentication failed. Please try again.");
            }
            finally
            {
                isAuthenticating = false;
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private async void btnAddAccount_Click(object sender, EventArgs e)
        {
            try
            {
                isAuthenticating = true;
                btnAddAccount.Enabled = false;
                btnAddAccount.Text = "Adding Account...";

                // Start interactive login
                var session = await loginHandler.AuthenticateInteractively();
                if (session != null)
                {
                    Logger.Info($"Successfully added new account: {session.Username}");
                    InitializeAccountSelector();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to add account: {ex.Message}");
                Alert.Error("Failed to add account. Please try again.");
            }
            finally
            {
                isAuthenticating = false;
                btnAddAccount.Enabled = true;
                btnAddAccount.Text = "Add Account";
            }
        }

        private async void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dropAccounts.SelectedIndex > 0) // Not offline mode
                {
                    string selectedItem = dropAccounts.SelectedItem.ToString();
                    if (selectedItem.EndsWith("(Offline)"))
                    {
                        // Remove offline account
                        string username = selectedItem.Replace(" (Offline)", "");
                        string offlineAccounts = RegistryConfig.GetUserPreference<string>("OfflineAccounts", "");
                        offlineAccounts = string.Join(",", offlineAccounts.Split(',').Where(x => x != username));
                        RegistryConfig.SaveUserPreference("OfflineAccounts", offlineAccounts);
                        Logger.Info($"Successfully removed offline account: {username}");
                    }
                    else
                    {
                        // Remove online account
                        var selectedAccount = savedAccounts[dropAccounts.SelectedIndex - 1];
                        await loginHandler.Signout(selectedAccount);
                        Logger.Info($"Successfully removed account: {selectedAccount.Identifier}");
                    }

                    // Clear auto-login if this was the last logged in account
                    string lastUsername = RegistryConfig.GetUserPreference<string>("LastUsername");
                    if (selectedItem.Contains(lastUsername))
                    {
                        RegistryConfig.SaveUserPreference("KeepLoggedIn", false);
                        RegistryConfig.DeleteValue("LastUsername");
                        RegistryConfig.DeleteValue("LastUUID");
                        RegistryConfig.DeleteValue("WasOffline");
                    }

                    // Reload accounts
                    InitializeAccountSelector();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to remove account: {ex.Message}");
                Alert.Error("Failed to remove account. Please try again.");
            }
        }

        private void btnSwitchInstance_Click(object sender, EventArgs e)
        {
            RegistryConfig.SaveUserPreference("LastInstanceDirectory", "null");
            RegistryConfig.SaveUserPreference("RememberLastInstance", false);
            IsLoggingOut = true;
            Program.restart();
        }
    }
}
