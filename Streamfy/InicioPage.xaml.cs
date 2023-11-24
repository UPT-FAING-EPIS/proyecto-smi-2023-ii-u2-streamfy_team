using Microsoft.Maui.Controls;

namespace Streamfy
{
    public partial class InicioPage : ContentPage
    {
        
        public InicioPage()
        {
            InitializeComponent();
        }
  
        private async void OnExploreMusicClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Dashboard());
        }

    }
}
