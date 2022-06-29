using System;
using TwitchDownloaderCore;
using TwitchDownloaderCore.Options;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace TwitchTranscriber
{
  class Program
  {
    class Video
    {
      public string created_at {get; set;}
      public string description {get; set;}
      public string duration {get; set;}
      public string id {get; set;}
      public string language {get; set;}
      public string? muted_segments {get; set;}
      public string published_at {get; set;}
      public string stream_id {get; set;}
      public string thumbnail_url {get; set;}
      public string title {get; set;}
      public string type {get; set;}
      public string url {get; set;}
      public string user_id {get; set;}
      public string user_login {get; set;}
      public string user_name {get; set;}
      public int view_count {get; set;}
      public string viewable {get; set;}
    }
    class Pagination
    {
      public string cursor {get; set;}
    }
    class Videos
    {
      public Video[] data {get; set;}
      public Pagination pagination {get; set;}
    }
    static async Task Main(string[] args)
    {
      string jsonFileName = "Clips copy.json";
      string jsonString = File.ReadAllText(jsonFileName);
      Videos videos = JsonSerializer.Deserialize<Videos>(jsonString);
      
      List<VideoDownloadOptions> downloadOptions = CreateDownloadOptions(videos);
      
      await DownloadAllVideosAsync(downloadOptions);
    }
    static List<VideoDownloadOptions> CreateDownloadOptions(Videos videos)
    {
      string quality = "160p";
      bool cropBeginning = false;
      bool cropEnding = false;
      int downloadThreads = 10;
      string tempFolder = "D:\\Repos\\Projects\\Twitch Data\\Files";

      List<VideoDownloadOptions> downloadOptions = new List<VideoDownloadOptions>();
      
      for (int i = 0; i < videos.data.Length; i++)
      {
        VideoDownloadOptions videoDownloadOptions = new VideoDownloadOptions();
        videoDownloadOptions.Id = Int32.Parse(videos.data[i].id);
        videoDownloadOptions.Quality = quality;
        videoDownloadOptions.CropBeginning = cropBeginning;
        videoDownloadOptions.CropEnding = cropEnding;
        videoDownloadOptions.DownloadThreads = downloadThreads;
        videoDownloadOptions.TempFolder = tempFolder;
        videoDownloadOptions.Filename = $"{videos.data[i].id}_{videos.data[i].created_at}_{videos.data[i].title}";

        downloadOptions.Add(videoDownloadOptions);
      }
      return downloadOptions;
    }
    static async Task DownloadAllVideosAsync(List<VideoDownloadOptions> downloadOptions)
    {
      for (int i = 0; i < downloadOptions.Count; i++)
      {
        VideoDownloader videoDownloader = new VideoDownloader(downloadOptions[i]);
        CancellationToken cancel = new CancellationToken();
        Progress<ProgressReport> prog = new Progress<ProgressReport>();
        await videoDownloader.DownloadAsync(prog, cancel);
      }
    }
  }
}