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
    // Opciones del menú con imágenes
    var options = new ObservableCollection<OptionWithImage>
    {
        new OptionWithImage { Option = "Inicio", ImageSource = "home.png" },
        new OptionWithImage { Option = "Búsqueda", ImageSource = "search.png" },
        new OptionWithImage { Option = "Cerrar sesión", ImageSource = "logout.png" }
    };

    // Crear la colección de vista con las opciones
    var menuCollectionView = new CollectionView
    {
        ItemsSource = options,
        ItemTemplate = new DataTemplate(() =>
        {
            // Crear una cuadrícula para cada elemento de la colección
            var grid = new Grid();

            // Definir la estructura de la cuadrícula
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Crear la imagen
            var image = new Image
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Aspect = Aspect.AspectFit // Ajustar la relación de aspecto de la imagen
            };
            image.SetBinding(Image.SourceProperty, "ImageSource");

            // Crear la etiqueta de texto
            var label = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(10, 0, 0, 0) // Agregar margen a la izquierda de la etiqueta
            };
            label.SetBinding(Label.TextProperty, "Option");

            // Agregar la imagen y la etiqueta a la cuadrícula
            grid.Children.Add(image);
            grid.Children.Add(label);

            // Configurar el reconocimiento de gestos
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, args) =>
            {
                var selectedOption = (OptionWithImage)((View)s).BindingContext;
                if (selectedOption != null)
                {
                    // Manejar la selección de la opción
                    await HandleMenuOptionSelection(selectedOption.Option);
                }
            };

            // Agregar el reconocimiento de gestos a la cuadrícula
            grid.GestureRecognizers.Add(tapGestureRecognizer);

            // Devolver la cuadrícula como el contenido del elemento de la colección
            return grid;
        })
    };

    // Crear un marco para el menú
    var frame = new Frame
    {
        Content = menuCollectionView,
        Padding = new Thickness(10),
        HasShadow = true,
        CornerRadius = 10, // Aumentar el radio de la esquina para un aspecto más redondeado
    };

    // Crear un diseño de apilamiento para contener el marco
    var stackLayout = new StackLayout
    {
        Children = { frame },
        VerticalOptions = LayoutOptions.CenterAndExpand,
        HorizontalOptions = LayoutOptions.CenterAndExpand
    };

    // Crear un visor de desplazamiento que contiene el diseño de apilamiento
    var scrollView = new ScrollView { Content = stackLayout };

    // Crear una página de contenido para el menú emergente
    var popup = new ContentPage
    {
        Content = scrollView
    };

    // Mostrar el menú emergente como una página modal
    await Navigation.PushModalAsync(popup);
}

// Método para manejar la selección de opciones del menú
private async Task HandleMenuOptionSelection(string option)
{
    switch (option)
    {
        case "Inicio":
            await Navigation.PushAsync(new InicioPage());
            break;

        case "Búsqueda":
            await Navigation.PushAsync(new Dashboard());
            break;

        case "Cerrar sesión":
            await Navigation.PushAsync(new MainPage());
            break;
    }

    // Cerrar el menú emergente después de seleccionar una opción
    await Navigation.PopModalAsync();
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
                            Console.WriteLine($"Error en la solicitud a la API de Deezer. Codigo de estado: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la busqueda: {ex.Message}");
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
                    throw new Exception($"Error en la solicitud a la API de Deezer. Codigo de estado: {response.StatusCode}");
                }
            }
        }
    }
}