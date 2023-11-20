using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using Streamfy.ViewModels;

namespace Streamfy.Views
{
    public partial class Reproductor : ContentPage
    {
        private ReproductorDeezerViewModel viewModel;

        public Reproductor(string deezerLink)
        {
            InitializeComponent();

            viewModel = new ReproductorDeezerViewModel(deezerLink);
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