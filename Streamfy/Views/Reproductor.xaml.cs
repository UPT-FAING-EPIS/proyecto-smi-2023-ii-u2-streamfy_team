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

            var pauseTapGestureRecognizer = new TapGestureRecognizer();
            pauseTapGestureRecognizer.Tapped += OnPauseTapped;
            PauseButton.GestureRecognizers.Add(pauseTapGestureRecognizer);
        }

        private void OnPlayPauseTapped(object sender, EventArgs e)
        {
            viewModel.PlayCommand.Execute(null);
        }

        private void OnPauseTapped(object sender, EventArgs e)
        {
            viewModel.StopCommand.Execute(null);
        }

    }
}