using System;
using System.IO;
using ShellProgressBar;

namespace PartnerCenter.Console.MicrosoftSample
{
    public class ConsoleStorageProgress : IProgress<long>, IDisposable
    {
        private readonly string _filePath;
        private ProgressBar? _succeededProgress;

        public ConsoleStorageProgress(string filePath)
        {
            _filePath = filePath;
        }
        
        public void Report(long bytesTransferred)
        {
            if(_succeededProgress == null)
            {
                
                var options = new ProgressBarOptions
                              {
                                  ForegroundColor = ConsoleColor.Yellow,
                                  ForegroundColorDone = ConsoleColor.DarkGreen,
                                  BackgroundColor = ConsoleColor.DarkGray,
                                  ProgressCharacter = '\u2593',
                                  ProgressBarOnBottom = true
                              };
                var fileSizeInBytes = new FileInfo(_filePath).Length;
                var fileSizeInMb = fileSizeInBytes/1024;
                _succeededProgress = new ProgressBar((int)fileSizeInMb, $"Uploading {Path.GetFileName(_filePath)}", options);
            }
            var progressInMb = (int) (bytesTransferred / 1024);
            _succeededProgress.Tick(progressInMb);
        }

        public void Dispose()
        {
            _succeededProgress?.Dispose();
        }
    }
}