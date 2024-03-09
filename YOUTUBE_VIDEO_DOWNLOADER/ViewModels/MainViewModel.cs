using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YOUTUBE_VIDEO_DOWNLOADER2.Commands;
using YoutubeExplode;

namespace YOUTUBE_VIDEO_DOWNLOADER2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string? downloadUrl;

        public string? DownloadUrl
        {
            get { return downloadUrl; }
            set { downloadUrl = value; OnPropertyChanged(); }
        }

        public RelayCommand DownloadCommand { get; set; }
        public MainViewModel()
        {
            DownloadUrl = "https://www.youtube.com/watch?v=6Dh-RL__uN4";
            DownloadCommand = new RelayCommand((param) =>
            {
                //desktop  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                DownloadYouTubeVideo(DownloadUrl, @"C:\Users\murad\Desktop");
            });
        }

        static async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            // Sanitize the video title to remove invalid characters from the file name
            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            // Get all available muxed streams
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
                MessageBox.Show($"Video saved as: {outputFilePath}{datetime}");
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }


    }
}
