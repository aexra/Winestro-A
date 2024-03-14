using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace Winestro_A.Structures;

public struct MusicItem
{
    public string Title;
    public string Description;
    public string Duration;
    public string Author;
    public string AudioUrl;

    public MusicItem(string title, string description, string duration, string author, string audioUrl)
    {
        Title = title;
        Description = description;
        Duration = duration;
        Author = author;
        AudioUrl = audioUrl;
    }
}
