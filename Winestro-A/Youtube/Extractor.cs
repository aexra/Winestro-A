using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Winestro_A.Youtube;

public static class Extractor
{
    private static YoutubeClient client;

    public static void Init()
    {
        client = new();
    }

    public static async Task<Video> GetVideoAsync(string url)
    {
        return await client.Videos.GetAsync(url);
    }
    public static async Task<StreamManifest> GetVideoStreamManifest(string url)
    {
        return await client.Videos.Streams.GetManifestAsync(url);
    }
    public static async Task<IStreamInfo> GetVideoAudioStreamHighestQuality(string url)
    {
        var manifest = await GetVideoStreamManifest(url);
        return manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    }
}
