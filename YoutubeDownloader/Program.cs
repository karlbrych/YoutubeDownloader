using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MediaToolkit;
using MediaToolkit.Model;

namespace YouTubeDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(URL());
                string url = Console.ReadLine();
                Console.WriteLine(Format());
                string format = Console.ReadLine();
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(url);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                if (streamInfo != null)
                {
                    string videoPath = Path.Combine(Environment.CurrentDirectory, video.Title + "." + streamInfo.Container.Name);
                    await DownloadWithProgressAsync(youtube, streamInfo, videoPath);
                    Console.WriteLine("");



                    // Convert to another format (e.g., mp3)
                    var inputFile = new MediaFile { Filename = videoPath };
                    var outputFile = new MediaFile { Filename = $"{Path.GetFileNameWithoutExtension(videoPath)}.{format}" };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);
                        engine.Convert(inputFile, outputFile);
                        File.Delete(inputFile.Filename);
                    }

                    Console.WriteLine($"Video Downloaded: {video}.{format}");
                    await Console.Out.WriteLineAsync();
                    await Console.Out.WriteLineAsync();
                    await Console.Out.WriteLineAsync();
                }
                else
                {
                    Console.WriteLine("No suitable stream found.");
                }
            }
        }

        private static async Task DownloadWithProgressAsync(YoutubeClient youtube, IStreamInfo streamInfo, string filePath)
        {
            var progress = new Progress<double>(p => DisplayProgress(p));
            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);
        }

        private static void DisplayProgress(double progress)
        {
            int width = 50; // Width of the progress bar
            int newProgress = (int)(progress * 100);
            int filledSlots = (int)(progress * width);
            string progressBar = new string('=', filledSlots) + new string(' ', width - filledSlots);
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"[{progressBar}] {newProgress}%");
            
        }
        private static string URL()
        {
            Console.ForegroundColor= ConsoleColor.Yellow;
            return "Enter the url: ";
        }
        private static string Format()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            return "Enter the format";
        }
    }
}
