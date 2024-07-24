using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public static class ApiService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<Root> GetWeather(double latitude, double longitude)
        {
            var response = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&units=metric&appid=94c0888517d4783129384ef49ea34b9f", latitude, longitude));
            return JsonConvert.DeserializeObject<Root>(response);
        }

        public static async Task<Root> GetWeatherByCity(string city)
        {
            var response = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?q={0}&units=metric&appid=94c0888517d4783129384ef49ea34b9f", city));
            return JsonConvert.DeserializeObject<Root>(response);
        }
    }
}
