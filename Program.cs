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
      public string? broadcaster_id {get; set;}
      public string? broadcaster_name {get; set;}
      public string created_at {get; set;}
      public string description {get; set;}
      public string duration {get; set;}
      public string id {get; set;}
      public string? video_id {get; set;}
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
      string jsonFileName = "ArchivedVideos.json";
      string jsonString = File.ReadAllText(jsonFileName);
      Videos videos = JsonSerializer.Deserialize<Videos>(jsonString);
      
      List<VideoDownloadOptions> downloadOptions = CreateDownloadOptions(videos, true);
      
      await DownloadAllVideosAsync(downloadOptions);
    }
    static List<VideoDownloadOptions> CreateDownloadOptions(Videos videos, bool test=false)
    {
      string quality = "160p";
      bool cropBeginning = false;
      bool cropEnding = false;
      int downloadThreads = 10;
      string tempFolder = "D:\\Repos\\Projects\\TwitchTranscriber\\Files\\";

      List<VideoDownloadOptions> downloadOptions = new List<VideoDownloadOptions>();
      
      int numberOfVidoesToProcess;
      if(test)
      {
        numberOfVidoesToProcess = 3;
      }
      else
      {
        numberOfVidoesToProcess = videos.data.Length;
      }

      for (int i = 0; i < numberOfVidoesToProcess; i++)
      {
        VideoDownloadOptions videoDownloadOptions = new VideoDownloadOptions();
        videoDownloadOptions.Id = Int32.Parse(videos.data[i].id);
        videoDownloadOptions.Quality = quality;
        videoDownloadOptions.CropBeginning = cropBeginning;

        if(test)
        {
          videoDownloadOptions.CropEnding = true;
          videoDownloadOptions.CropEndingTime = 5.0;
        }
        else
        {
          videoDownloadOptions.CropEnding = cropEnding;
        }

        videoDownloadOptions.DownloadThreads = downloadThreads;
        videoDownloadOptions.TempFolder = tempFolder;
        videoDownloadOptions.FfmpegPath = "ffmpeg.exe"; //using Xabe.FFmpeg.Downloader;
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