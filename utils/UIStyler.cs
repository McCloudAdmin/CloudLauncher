using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace CloudLauncher.utils
{
    public class UIStyler
    {
        private static string stylepath = Program.appWorkDir + @"\styles.yaml";
        private static Dictionary<string, Dictionary<string, string>> styles;

        private static void ReadYaml()
        {
            try
            {
                styles = new Dictionary<string, Dictionary<string, string>>();

                if (File.Exists(stylepath))
                {
                    using (StreamReader reader = new StreamReader(stylepath))
                    {
                        var dsl = new DeserializerBuilder().Build();
                        styles = dsl.Deserialize<Dictionary<string, Dictionary<string, string>>>(reader);
                    }
                }
                else
                {
                    string yamlContent = @"
# Global styles (applied to all forms)
Global:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  Font: Segoe UI, 9pt

# Form-specific styles
FrmMain:
  BackColor: 30,30,30
  ForeColor: 255,255,255
  FormBorderStyle: None
  StartPosition: CenterScreen
  Size: 800,600

# Component-specific styles
UserNavigationBar:
  BackColor: 40,40,40
  Dock: Top
  Height: 30

# Form-specific control styles (using dot notation)
FrmMain.lblAppName:
  Text: CloudLauncher (DEV)
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt, style=Bold
  Location: 10,5

FrmMain.lblClose:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
  Location: 770,5
  Cursor: Hand

FrmMain.lblMinimize:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
  Location: 740,5
  Cursor: Hand

# Global control styles (applied to all forms)
lblAppName:
  ForeColor: 255,255,255
  Font: Segoe UI, 10pt
";

                    File.AppendAllText(stylepath, yamlContent);
                    Logger.Warning("[THEME] Style file does not exist! Created default styles.");
                    Program.restart();
                }
            }
            catch (Exception ex)
            {
                Logger.Warning("[THEME] Failed to read yml file: " + ex.ToString());
            }
        }

        public static void ApplyStyles(Control form, bool debug)
        {
            Logger.Info("[THEME] Applying styles to " + form.Name);
            ReadYaml();
            try
            {
                // Apply global styles first
                if (styles.ContainsKey("Global"))
                {
                    Dictionary<string, string> globalStyles = styles["Global"];
                    foreach (KeyValuePair<string, string> style in globalStyles)
                    {
                        ApplyStyle(form, style.Key, style.Value, debug);
                    }
                }

                // Apply form-specific styles
                string formName = form.Name;
                if (styles.ContainsKey(formName))
                {
                    Dictionary<string, string> formStyles = styles[formName];
                    foreach (KeyValuePair<string, string> style in formStyles)
                    {
                        ApplyStyle(form, style.Key, style.Value, debug);
                    }
                }

                // Apply styles to all child controls
                foreach (Control childControl in form.Controls)
                {
                    ApplyControlStyles(childControl, formName, debug);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[THEME] Failed to apply styles for '{form.Name}'!\nPlease check the logs\n" + ex.ToString());
            }
        }

        private static void ApplyControlStyles(Control control, string parentFormName, bool debug)
        {
            try
            {
                string controlName = control.Name;
                
                // Try to apply form-specific control styles first
                string formSpecificKey = $"{parentFormName}.{controlName}";
                if (styles.ContainsKey(formSpecificKey))
                {
                    Dictionary<string, string> controlStyles = styles[formSpecificKey];
                    foreach (KeyValuePair<string, string> style in controlStyles)
                    {
                        ApplyStyle(control, style.Key, style.Value, debug);
                    }
                }
                // If no form-specific styles found, try global control styles
                else if (styles.ContainsKey(controlName))
                {
                    Dictionary<string, string> controlStyles = styles[controlName];
                    foreach (KeyValuePair<string, string> style in controlStyles)
                    {
                        ApplyStyle(control, style.Key, style.Value, debug);
                    }
                }

                // Recursively apply to child controls
                foreach (Control childControl in control.Controls)
                {
                    ApplyControlStyles(childControl, parentFormName, debug);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[THEME] Failed to apply styles for control '{control.Name}'!\n" + ex.ToString());
            }
        }

        private static void ApplyStyle(Control control, string property, string value, bool debug)
        {
            Logger.Info($"[THEME] Applying style '{property}' to '{control.Name}'.");
            PropertyInfo prop = control.GetType().GetProperty(property);
            if (prop != null)
            {
                try
                {
                    if (prop.PropertyType == typeof(Color))
                    {
                        string[] rgbParts = value.Split(',');
                        if (rgbParts.Length == 3 && int.TryParse(rgbParts[0], out int r) && int.TryParse(rgbParts[1], out int g) && int.TryParse(rgbParts[2], out int b))
                        {
                            Color color = Color.FromArgb(r, g, b);
                            prop.SetValue(control, color);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied RGB Color '{color}' to '{property}' of '{control.Name}'.");
                            }
                        }
                        else
                        {
                            Color color = Color.FromName(value);
                            prop.SetValue(control, color);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Color '{color}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(Size))
                    {
                        string[] sizeParts = value.Split(',');
                        if (sizeParts.Length == 2 && int.TryParse(sizeParts[0], out int width) && int.TryParse(sizeParts[1], out int height))
                        {
                            Size size = new Size(width, height);
                            prop.SetValue(control, size);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Size '{size}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(Point))
                    {
                        string[] pointParts = value.Split(',');
                        if (pointParts.Length == 2 && int.TryParse(pointParts[0], out int x) && int.TryParse(pointParts[1], out int y))
                        {
                            Point point = new Point(x, y);
                            prop.SetValue(control, point);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Point '{point}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(DockStyle))
                    {
                        if (Enum.TryParse(value, out DockStyle dockStyle))
                        {
                            prop.SetValue(control, dockStyle);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied DockStyle '{dockStyle}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        if (bool.TryParse(value, out bool boolValue))
                        {
                            prop.SetValue(control, boolValue);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Boolean '{boolValue}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(FormStartPosition))
                    {
                        if (Enum.TryParse(value, out FormStartPosition startPosition))
                        {
                            prop.SetValue(control, startPosition);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied StartPosition '{startPosition}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(FormBorderStyle))
                    {
                        if (Enum.TryParse(value, out FormBorderStyle borderStyle))
                        {
                            prop.SetValue(control, borderStyle);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied FormBorderStyle '{borderStyle}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(Font))
                    {
                        string[] fontParts = value.Split(',');
                        if (fontParts.Length >= 2)
                        {
                            string fontName = fontParts[0].Trim();
                            float fontSize = float.Parse(fontParts[1].Trim().Replace("pt", ""));
                            FontStyle fontStyle = FontStyle.Regular;

                            if (fontParts.Length > 2)
                            {
                                string styleStr = fontParts[2].Trim().ToLower();
                                if (styleStr.Contains("bold")) fontStyle |= FontStyle.Bold;
                                if (styleStr.Contains("italic")) fontStyle |= FontStyle.Italic;
                                if (styleStr.Contains("underline")) fontStyle |= FontStyle.Underline;
                            }

                            Font font = new Font(fontName, fontSize, fontStyle);
                            prop.SetValue(control, font);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Font '{font}' to '{property}' of '{control.Name}'.");
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(Cursor))
                    {
                        try
                        {
                            Cursor cursor = Cursors.Default;
                            switch (value.ToLower())
                            {
                                case "hand":
                                    cursor = Cursors.Hand;
                                    break;
                                case "arrow":
                                    cursor = Cursors.Arrow;
                                    break;
                                case "wait":
                                    cursor = Cursors.WaitCursor;
                                    break;
                                case "cross":
                                    cursor = Cursors.Cross;
                                    break;
                                case "ibeam":
                                    cursor = Cursors.IBeam;
                                    break;
                                default:
                                    cursor = Cursors.Default;
                                    break;
                            }
                            prop.SetValue(control, cursor);
                            if (debug)
                            {
                                Logger.Info($"[THEME] Applied Cursor '{cursor}' to '{property}' of '{control.Name}'.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warning($"[THEME] Failed to apply cursor '{value}' to '{control.Name}': {ex.Message}");
                        }
                    }
                    else
                    {
                        prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
                        if (debug)
                        {
                            Logger.Info($"[THEME] Applied Value '{value}' to '{property}' of '{control.Name}'.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning($"[THEME] Failed to apply style '{property}' with value '{value}' to '{control.Name}': {ex.Message}");
                }
            }
            else
            {
                Logger.Warning($"[THEME] Property '{property}' not found on '{control.Name}'.");
            }
        }
    }
}