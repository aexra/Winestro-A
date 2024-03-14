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
    public string Author;
    public string AudioUrl;
    public TimeSpan? Duration;
}
