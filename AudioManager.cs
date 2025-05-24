using System.Windows.Media;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp1.Properties;

public static class AudioManager
{
    private static readonly MediaPlayer mediaPlayer = new MediaPlayer();
    private static readonly MediaPlayer sfxPlayer = new MediaPlayer();
    private static bool isInitialized = false;
    private static List<string> playlist = new List<string>();
    private static int currentTrackIndex = 0;
    private static readonly string musicFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
    private static readonly string soundsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

    public static void PlayBackgroundMusic()
    {
        LoadPlaylist();

        if (playlist.Count == 0) return;

        if (!isInitialized)
        {
            var lastAddedTrack = playlist
                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                .FirstOrDefault(); if (currentTrackIndex == -1) currentTrackIndex = 0;

            PlayTrack(currentTrackIndex);

            mediaPlayer.MediaEnded += (sender, e) => NextTrack();
            isInitialized = true;

            string sfxPath = Path.Combine(soundsFolderPath, "sfxsound.mp3");
            if (File.Exists(sfxPath))
            {
                sfxPlayer.Open(new Uri(sfxPath, UriKind.Absolute));
            }
        }
    }

    private static void LoadPlaylist()
    {
        if (Directory.Exists(musicFolderPath))
        {
            playlist = Directory.GetFiles(musicFolderPath, "*.mp3").ToList();
        }
    }

    private static void PlayTrack(int index)
    {
        if (playlist.Count == 0 || index < 0 || index >= playlist.Count) return;

        mediaPlayer.Open(new Uri(playlist[index], UriKind.Absolute));
        mediaPlayer.Play();
    }

    public static void NextTrack()
    {
        if (playlist.Count == 0) return;
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
        PlayTrack(currentTrackIndex);
    }

    public static void UpdateVolume()
    {
        mediaPlayer.Volume = Settings.Default.IsMuted
            ? 0
            : Settings.Default.MusicVolume / 100.0;

        sfxPlayer.Volume = Settings.Default.IsMuted
            ? 0
            : Settings.Default.SfxVolume / 100.0;
       
        if (mediaPlayer.Volume > 0 && mediaPlayer.Position == TimeSpan.Zero)
        {
            mediaPlayer.Play();
        }
    }

    public static void PlayButtonClickSound()
    {
        if (sfxPlayer.Volume > 0)
        {
            sfxPlayer.Position = TimeSpan.Zero;
            sfxPlayer.Play();
        }
    }

    public static void PlaySound(string soundFileName, float volumeModifier = 1.0f)
    {
        string soundPath = Path.Combine(soundsFolderPath, soundFileName);

        if (File.Exists(soundPath))
        {
            var player = new MediaPlayer
            {
                Volume = Settings.Default.IsMuted ? 0 : Settings.Default.SfxVolume / 100.0 * volumeModifier
            };
            player.Open(new Uri(soundPath, UriKind.Absolute));
            player.Play();
            player.MediaEnded += (s, e) => player.Close();
        }
    }

    public static void AddButtonSound(UIElement element)
    {
        switch (element)
        {
            case Button button:
                button.Click += (sender, e) => PlayButtonClickSound();
                break;
            case CheckBox checkBox:
                checkBox.Checked += (sender, e) => PlayButtonClickSound();
                checkBox.Unchecked += (sender, e) => PlayButtonClickSound();
                break;
            case ComboBox comboBox:
                comboBox.DropDownOpened += (sender, e) => PlayButtonClickSound();
                comboBox.DropDownClosed += (sender, e) => PlayButtonClickSound();
                comboBox.SelectionChanged += (sender, e) => PlayButtonClickSound();
                break;
            case RadioButton radioButton:
                radioButton.Checked += (sender, e) => PlayButtonClickSound();
                break;
            case Slider slider:
                slider.ValueChanged += (sender, e) => PlayButtonClickSound();
                break;
            case ScrollViewer scrollViewer:
                scrollViewer.ScrollChanged += (sender, e) => PlayButtonClickSound();
                break;
        }
    }
   
    public static void AddButtonsSound(params UIElement[] elements)
    {
        foreach (var element in elements)
        {
            AddButtonSound(element);
        }
    }

    public static void Cleanup()
    {
        mediaPlayer.Close();
        sfxPlayer.Close();
    }
}