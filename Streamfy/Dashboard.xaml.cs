using Firebase.Storage;
using Newtonsoft.Json;
using Streamfy.ViewModels;
using System.Net.Http;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Streamfy.Models;

namespace Streamfy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        private FirebaseViewModel storageHelper;
        private DeezerViewModel deezerViewModel;

        public Dashboard()
        {
            InitializeComponent();
            //GetProfileInfo();
            deezerViewModel = new DeezerViewModel();
            BindingContext = deezerViewModel; 
        }

        //private void GetProfileInfo()
        //{
        //    var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
        //    UserEmail.Text = userInfo.User.Email;
        //}

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
                                    Link = result.Link,
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
                            Console.WriteLine($"Error en la solicitud a la API de Deezer. Código de estado: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la búsqueda: {ex.Message}");
                }
            }
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {         
            SearchBar.Unfocus();
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
                    throw new Exception($"Error en la solicitud a la API de Deezer. Código de estado: {response.StatusCode}");
                }
            }
        }
    }
}