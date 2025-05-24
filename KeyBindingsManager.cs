using System.Collections.Generic;
using System.Windows.Input;
using System;
using WpfApp1.Properties;

public static class KeyBindingsManager
{
    public static event Action KeyBindingsChanged;

    public static void NotifyKeyBindingsChanged()
    {
        KeyBindingsChanged?.Invoke();
    }

    public static Dictionary<string, Key> LoadKeyBindings()
    {
        var defaultBindings = new Dictionary<string, Key>
        {
            ["Forward"] = Key.W,
            ["Backward"] = Key.S,
            ["Left"] = Key.A,
            ["Right"] = Key.D
        };

        if (Settings.Default.ForwardKey == null)
        {
            SaveKeyBindings(defaultBindings);
            return defaultBindings;
        }

        try
        {
            var keyBindings = new Dictionary<string, Key>
            {
                ["Forward"] = (Key)Enum.Parse(typeof(Key), Settings.Default.ForwardKey),
                ["Backward"] = (Key)Enum.Parse(typeof(Key), Settings.Default.BackwardKey),
                ["Left"] = (Key)Enum.Parse(typeof(Key), Settings.Default.LeftKey),
                ["Right"] = (Key)Enum.Parse(typeof(Key), Settings.Default.RightKey)
            };
            return keyBindings;
        }
        catch
        {
            return defaultBindings;
        }
    }

    public static void SaveKeyBindings(Dictionary<string, Key> keyBindings)
    {
        Settings.Default.ForwardKey = keyBindings["Forward"].ToString();
        Settings.Default.BackwardKey = keyBindings["Backward"].ToString();
        Settings.Default.LeftKey = keyBindings["Left"].ToString();
        Settings.Default.RightKey = keyBindings["Right"].ToString();
        Settings.Default.Save();
        NotifyKeyBindingsChanged();
    }

    public static void ResetToDefaults()
    {
        var defaultBindings = new Dictionary<string, Key>
        {
            ["Forward"] = Key.W,
            ["Backward"] = Key.S,
            ["Left"] = Key.A,
            ["Right"] = Key.D
        };
        SaveKeyBindings(defaultBindings);
    }
}