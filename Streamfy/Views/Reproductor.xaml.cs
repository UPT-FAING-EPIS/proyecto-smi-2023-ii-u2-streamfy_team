using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using Streamfy.ViewModels;

namespace Streamfy.Views
{
    public partial class Reproductor : ContentPage
    {
        private ReproductorDeezerViewModel viewModel;

        public Reproductor()
        {
            InitializeComponent();

            viewModel = new ReproductorDeezerViewModel("https://cdns-preview-c.dzcdn.net/stream/c-c45ae335d3f89e153c37217f4495cefc-4.mp3");
            BindingContext = viewModel;

            
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnPlayPauseTapped;
            PlayPauseButton.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnPlayPauseTapped(object sender, EventArgs e)
        {
            
            viewModel.PlayCommand.Execute(null);

            
        }
    }
}