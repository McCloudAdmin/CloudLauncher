using CloudLauncher.Properties;
using CloudLauncher.utils;

namespace CloudLauncher.Components
{
    public partial class AlertComponent : Form
    {
        private const int MAX_ALERTS = 10;
        private const int ANIMATION_INTERVAL = 16; // ~60 FPS for smooth animation
        private const int DISPLAY_DURATION = 3000; // 3 seconds
        private const double OPACITY_STEP = 0.08; // Smaller step for smoother fade
        private const int SLIDE_STEP = 26; // Faster slide animation

        public enum AlertAction
        {
            Wait,
            Start,
            Close
        }

        public enum AlertType
        {
            Success,
            Warning,
            Error,
            Info
        }

        private AlertAction _currentAction;
        private int _targetX;
        private int _targetY;
        private readonly System.Windows.Forms.Timer _animationTimer;

        public AlertComponent()
        {
            InitializeComponent();
            UIStyler.ApplyStyles(this, true);
            
            // Hide from Alt+Tab and taskbar
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true; // Keep on top of other windows
            
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AlertComponent_Load(object sender, EventArgs e)
        {
            // Form load event handler - can be used for additional initialization
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            switch (_currentAction)
            {
                case AlertAction.Wait:
                    _animationTimer.Interval = DISPLAY_DURATION;
                    _currentAction = AlertAction.Close;
                    break;

                case AlertAction.Start:
                    _animationTimer.Interval = ANIMATION_INTERVAL;
                    
                    // Fade in
                    if (Opacity < 1.0)
                    {
                        Opacity = Math.Min(1.0, Opacity + OPACITY_STEP);
                    }
                    
                    // Slide in from right
                    if (Left > _targetX)
                    {
                        Left = Math.Max(_targetX, Left - SLIDE_STEP);
                    }
                    
                    // Check if animation is complete
                    if (Opacity >= 1.0 && Left <= _targetX)
                    {
                        _currentAction = AlertAction.Wait;
                    }
                    break;

                case AlertAction.Close:
                    _animationTimer.Interval = ANIMATION_INTERVAL;
                    
                    // Fade out
                    Opacity = Math.Max(0.0, Opacity - OPACITY_STEP);
                    
                    // Slide out to left
                    Left -= SLIDE_STEP;
                    
                    // Close when fully transparent
                    if (Opacity <= 0.0)
                    {
                        _animationTimer.Stop();
                        Close();
                    }
                    break;
            }
        }

        public void ShowAlert(string message, AlertType type)
        {
            try
            {
                // Initialize form properties
                Opacity = 0.0;
                StartPosition = FormStartPosition.Manual;
                
                // Find available position
                var position = CalculateAlertPosition();
                if (position == null)
                {
                    Logger.Warning("Maximum number of alerts reached");
                    return;
                }

                _targetX = position.Value.X;
                _targetY = position.Value.Y;
                Location = new Point(_targetX + Width, _targetY); // Start off-screen
                Name = position.Value.Name;

                // Configure appearance based on type
                ConfigureAlertAppearance(type);
                
                // Set message
                lblMsg.Text = message;

                // Start animation
                Show();
                _currentAction = AlertAction.Start;
                _animationTimer.Interval = ANIMATION_INTERVAL;
                _animationTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to show alert: {ex.Message}");
            }
        }

        private (int X, int Y, string Name)? CalculateAlertPosition()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            var baseX = screen.Width - Width - 15;
            var baseY = screen.Height - Height;

            for (int i = 1; i <= MAX_ALERTS; i++)
            {
                var alertName = $"alert{i}";
                var existingAlert = Application.OpenForms[alertName] as AlertComponent;

                if (existingAlert == null)
                {
                    var y = baseY - (Height * (i - 1));
                    return (baseX, y, alertName);
                }
            }

            return null; // No available position
        }

        private void ConfigureAlertAppearance(AlertType type)
        {
            switch (type)
            {
                case AlertType.Success:
                    icontype.Image = Resources.happycloud;
                    BackColor = Color.SeaGreen;
                    break;
                    
                case AlertType.Warning:
                    icontype.Image = Resources.warning;
                    BackColor = Color.DarkOrange;
                    break;
                    
                case AlertType.Error:
                    icontype.Image = Resources.error;
                    BackColor = Color.DarkRed;
                    break;
                    
                case AlertType.Info:
                    icontype.Image = Resources.info;
                    BackColor = Color.RoyalBlue;
                    break;
                    
                default:
                    icontype.Image = Resources.info;
                    BackColor = Color.RoyalBlue;
                    break;
            }
        }
    }
}
