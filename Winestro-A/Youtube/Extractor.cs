using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Services;
using Winestro_A.Structures;
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

        _ = Task.Run(async () => await GetAudioStreamHighestQuality("https://www.youtube.com/watch?v=jKikelM3FWM"));
    }

    public static async Task<Video?> GetVideoAsync(string url)
    {
        return await client.Videos.GetAsync(url);
    }
    public static async Task<StreamManifest?> GetVideoStreamManifest(string url)
    {
        return await client.Videos.Streams.GetManifestAsync(url);
    }
    public static async Task<IStreamInfo?> GetAudioStreamHighestQuality(string url)
    {
        try
        {
            var manifest = await GetVideoStreamManifest(url);
            return manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static async Task<MusicItem?> GetMusicItemAsync(string url)
    {
        MusicItem item = new();
        Video? video = await GetVideoAsync(url);
        var streamInfo = await GetAudioStreamHighestQuality(url);

        if (video == null) return null;
        if (streamInfo == null) return null;

        item.Title = video.Title;
        item.Description = video.Description;
        item.Duration = video.Duration;
        item.Author = video.Author.ChannelTitle;
        item.AudioUrl = streamInfo.Url;
        item.Url = video.Url;

        return item;
    }
}
