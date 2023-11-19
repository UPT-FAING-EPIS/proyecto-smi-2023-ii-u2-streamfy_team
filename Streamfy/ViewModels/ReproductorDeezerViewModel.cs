using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using System.Net.Http;

namespace Streamfy.ViewModels
{
    public class ReproductorDeezerViewModel : INotifyPropertyChanged
    {
        private IAudioManager audioManager;
        private IAudioPlayer audioPlayer;

        private bool isLoadComplete = false;
        public bool IsLoadComplete
        {
            get { return this.isLoadComplete; }
            set
            {
                if (this.isLoadComplete != value)
                {
                    this.isLoadComplete = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string status = "Wait";
        public string Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Command PlayCommand { get; set; }
        public Command PauseCommand { get; set; }
        public Command StopCommand { get; set; }

        private string deezerLink;

        public ReproductorDeezerViewModel(string link)
        {
            this.audioManager = new AudioManager();
            this.deezerLink = link;

            PlayCommand = new Command(OnPlayCommand);
            PauseCommand = new Command(OnPauseCommand);
            StopCommand = new Command(OnStopCommand);

            Initialize();
        }

        private async void Initialize()
        {
            Task.Run(async () =>
            {
                var deezerStream = await GetDeezerStream(deezerLink);

                // Crea el reproductor de audio con el Stream
                audioPlayer = audioManager.CreatePlayer(deezerStream);

                if (audioPlayer != null)
                {
                    IsLoadComplete = true;
                }
            });
        }

        private async Task<Stream> GetDeezerStream(string deezerLink)
        {
           
            using (var client = new HttpClient())
            {
                var response = await client.GetStreamAsync(new Uri(deezerLink));
                return response;
            }
        }

        private void OnPlayCommand()
        {
            if (audioPlayer != null)
            {
                audioPlayer.Play();
                Status = "Play";
            }
        }

        private void OnPauseCommand()
        {
            if (audioPlayer != null)
            {
                if (audioPlayer.IsPlaying)
                {
                    audioPlayer.Pause();
                    Status = "Pause";
                }
                else
                {
                    audioPlayer.Play();
                    Status = "Play";
                }
            }
        }

        private void OnStopCommand()
        {
            if (audioPlayer != null)
            {
                if (audioPlayer.IsPlaying)
                {
                    audioPlayer.Stop();
                    Status = "Stop";
                }
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
