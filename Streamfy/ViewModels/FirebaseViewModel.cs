using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamfy.ViewModels
{
    public class FirebaseViewModel
    {
        private const string BaseUrl = "https://streamfy-fb7bb.firebaseapp.com/"; // Ruta del dominio de Firebase
        private const string ApiKey = "AIzaSyAj9XyHXUn0KPsQVRYjZnxc_kHrrSdZH6Y"; // Clave de API de Firebase
        private const string StoragePath = "streamfy-fb7bb.appspot.com"; // Ruta del almacenamiento de Firebase

        public async Task<List<string>> ListFilesInFolderAsync(string folderPath)
        {
            try
            {
                // Componer la URL de la solicitud
                string listUrl = $"{BaseUrl}/v0/b/{StoragePath}/o/{folderPath}?key={ApiKey}";

                // Realizar la solicitud HTTP GET
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(listUrl);

                    // Deserializar la respuesta JSON
                    var items = JsonConvert.DeserializeObject<FirebaseStorageResponse>(response);

                    List<string> fileNames = new List<string>();
                    foreach (var item in items.Items)
                    {
                        fileNames.Add(item.Name);
                    }

                    return fileNames;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine($"Error al listar archivos: {ex.Message}");
                return null;
            }
        }

        private class FirebaseStorageResponse
        {
            public List<FirebaseStorageItem> Items { get; set; }
        }

        private class FirebaseStorageItem
        {
            public string Name { get; set; }
        }
    }
}
