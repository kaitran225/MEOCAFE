using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace POS.Avalonia.ViewModels;

public partial class MusicPlayerViewModel : ViewModelBase
{
    [ObservableProperty] private string _currentTrack = "Lofi Café Mornings";
    [ObservableProperty] private string _artist = "ChillHop Records";
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private double _progress = 35;
    [ObservableProperty] private TrackItem? _selectedTrack;

    public ObservableCollection<TrackItem> Playlist { get; } = new()
    {
        new("Lofi Café Mornings", "ChillHop Records", "3:42"),
        new("Espresso Dreams", "Jazz Café", "4:15"),
        new("Rainy Day Brew", "Ambient Sounds", "5:03"),
        new("Bossa Nova Blend", "Latin Lounge", "3:58"),
        new("Midnight Roast", "Deep House Café", "4:30"),
        new("Sunday Latte", "Acoustic Sessions", "3:21"),
        new("Matcha Vibes", "Lo-Fi Beats", "4:47"),
        new("Golden Hour Pour", "Sunset Sounds", "3:55"),
    };

    [RelayCommand]
    private void PlayPause() => IsPlaying = !IsPlaying;

    [RelayCommand]
    private void Next()
    {
        var idx = SelectedTrack is null ? 0 : Playlist.IndexOf(SelectedTrack);
        idx = (idx + 1) % Playlist.Count;
        SelectedTrack = Playlist[idx];
        CurrentTrack = SelectedTrack.Title;
        Artist = SelectedTrack.Artist;
        Progress = 0;
        IsPlaying = true;
    }

    [RelayCommand]
    private void Previous()
    {
        var idx = SelectedTrack is null ? 0 : Playlist.IndexOf(SelectedTrack);
        idx = (idx - 1 + Playlist.Count) % Playlist.Count;
        SelectedTrack = Playlist[idx];
        CurrentTrack = SelectedTrack.Title;
        Artist = SelectedTrack.Artist;
        Progress = 0;
        IsPlaying = true;
    }
}

public class TrackItem
{
    public string Title { get; }
    public string Artist { get; }
    public string Duration { get; }

    public TrackItem(string title, string artist, string duration)
    {
        Title = title;
        Artist = artist;
        Duration = duration;
    }
}
