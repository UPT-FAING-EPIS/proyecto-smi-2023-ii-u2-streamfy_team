using Streamfy.ViewModels;

namespace Streamfy
{
    public partial class MainPage : ContentPage
    {        
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(Navigation);
        }
        
    }
}