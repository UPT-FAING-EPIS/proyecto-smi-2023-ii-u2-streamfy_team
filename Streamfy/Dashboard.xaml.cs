using Firebase.Storage;
using Newtonsoft.Json;
using Streamfy.ViewModels;
using System.Net.Http;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Streamfy.Models;
using Streamfy.Views;
using Plugin.Maui.Audio;
using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Collections.Generic;



namespace Streamfy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        private FirebaseViewModel storageHelper;
        private DeezerViewModel deezerViewModel;
        private IAudioPlayer audioPlayer;
        public Dashboard()
        {
            InitializeComponent();
            deezerViewModel = new DeezerViewModel();
            BindingContext = deezerViewModel;

            // Obtén la referencia al ToolbarItem
            var menuToolbarItem = new ToolbarItem
            {
                Order = ToolbarItemOrder.Primary,
                IconImageSource = "menu.svg"
            };

            // Asigna un controlador de eventos para el evento Clicked
            menuToolbarItem.Clicked += OnMenuToolbarItemClicked;

            ToolbarItems.Add(menuToolbarItem);
        }

        private async void OnMenuToolbarItemClicked(object sender, EventArgs e)
        {
            var options = new ObservableCollection<OptionWithImage>
            {
                new OptionWithImage { Option = "Inicio", ImageSource = "home.png" },
                new OptionWithImage { Option = "Busqueda", ImageSource = "search.png" },
                new OptionWithImage { Option = "Cerrar sesion", ImageSource = "logout.png" }
            };

            var menuCollectionView = new CollectionView
            {
                ItemsSource = options,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    var image = new Image
                    {
                        WidthRequest=50,
                        HeightRequest=50,
                    };
                    image.SetBinding(Image.SourceProperty, "ImageSource");

                    var label = new Label { VerticalOptions = LayoutOptions.CenterAndExpand };
                    label.SetBinding(Label.TextProperty, "Option");

                    grid.Children.Add(image);
                    grid.Children.Add(label);

                    return grid;
                })
            };

            var frame = new Frame
            {
                Content = menuCollectionView,
                Padding = new Thickness(10),
                HasShadow = true,
                CornerRadius = 5
            };

            var stackLayout = new StackLayout
            {
                Children = { frame },
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var scrollView = new ScrollView { Content = stackLayout };

            var popup = new ContentPage
            {
                Content = scrollView,
            };

            await Navigation.PushModalAsync(popup);

            menuCollectionView.SelectionChanged += (o, args) =>
            {
                var selectedOption = (OptionWithImage)args.CurrentSelection.FirstOrDefault();
                if (selectedOption != null)
                {
                    switch (selectedOption.Option)
                    {
                        case "Inicio":
                            // Lógica para la opción "Inicio"
                            break;

                        case "Busqueda":
                            // Lógica para la opción "Busqueda"
                            break;

                        case "Cerrar sesion":
                            // Lógica para la opción "Cerrar sesion"
                            break;
                    }

                    Navigation.PopModalAsync();
                }
            };
        }

        public class OptionWithImage
    {
        public string Option { get; set; }
        public string ImageSource { get; set; }
    }

        public class DeezerViewModel : BaseViewModel
        {
            private ObservableCollection<DeezerData> _searchResults;
            private bool _noResultsVisible;
            public ObservableCollection<DeezerData> SearchResults
            {
                get { return _searchResults; }
                set
                {
                    _searchResults = value;
                    OnPropertyChanged(nameof(SearchResults));

                    NoResultsVisible = _searchResults == null || _searchResults.Count == 0;
                }
            }

            public ICommand SearchCommand => new Command<string>(async (searchText) => await PerformSearch(searchText));
            public bool NoResultsVisible
            {
                get { return _noResultsVisible; }
                set
                {
                    _noResultsVisible = value;
                    OnPropertyChanged(nameof(NoResultsVisible));
                }
            }
            private async Task PerformSearch(string searchText)
            {
                try
                {
                    string apiUrl = $"https://api.deezer.com/search?q={Uri.EscapeDataString(searchText)}";
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            var searchResponse = JsonConvert.DeserializeObject<DeezerSearchResponse>(content);

                            if (searchResponse != null && searchResponse.Data != null && searchResponse.Data.Any())
                            {
                                var mappedResults = searchResponse.Data.Select(result => new DeezerData
                                {
                                    Title = result.Title,
                                    Link = result.Preview,
                                    Duration = result.Duration,
                                    Artist = new DeezerArtist { Name = result.Artist.Name },
                                    
                                });

                                
                                SearchResults = new ObservableCollection<DeezerData>(mappedResults);
                            }
                            else
                            {
                                Console.WriteLine("La respuesta no contiene resultados.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error en la solicitud a la API de Deezer. C�digo de estado: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la b�squeda: {ex.Message}");
                }
            }
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {         
            SearchBar.Unfocus();
        }


        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is DeezerData selectedTrack)
            {
                
                if (audioPlayer != null)
                {
                    audioPlayer.Stop();
                }


                Reproductor reproductorPage = new Reproductor(selectedTrack.Link);

                var reproductorViewModel = new ReproductorDeezerViewModel(selectedTrack.Link);
                reproductorPage.BindingContext = reproductorViewModel;

                
                Navigation.PushAsync(reproductorPage);
            }
        }



        private async Task<DeezerData> ObtenerDatosDeezer()
        {
            string apiKey = "d1ea36ac5fmsh83bc4a0792f719dp120cf4jsnad3ed2ef8b1c";
            string apiUrl = "https://deezerdevs-deezer.p.rapidapi.com/infos";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "deezerdevs-deezer.p.rapidapi.com");

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DeezerData>(content);
                }
                else
                {
                    throw new Exception($"Error en la solicitud a la API de Deezer. C�digo de estado: {response.StatusCode}");
                }
            }
        }
    }
}