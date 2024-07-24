using WeatherApp.Services;

namespace WeatherApp;

public partial class WeatherPage : ContentPage
{
    public List<Models.List> WeatherList;
    private double latitude;
    private double longitude;
    public WeatherPage()
    {
        InitializeComponent();
        WeatherList = new List<Models.List>();
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();


        var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (locationPermissionStatus != PermissionStatus.Granted)
        {
            locationPermissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (locationPermissionStatus != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Denied", "Location permission is required to get weather information.", "OK");
            return;
        }

        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);

    }

    public async Task GetLocation()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;
                // Konum bilgilerini kullanarak harita veya diðer verileri güncelleyin
            }
            else
            {
                await DisplayAlert("Error", "Unable to get location.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while getting location: {ex.Message}", "OK");
        }
    }

    private async void TapLocation_Tapped(object sender, EventArgs e)
    {
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);
    }

    public async Task GetWeatherDataByLocation(double latitude, double longitude)
    {
        var result = await ApiService.GetWeather(latitude, longitude);
        UpdateUI(result);



    }
    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var response = await DisplayPromptAsync(title: "", message: "", placeholder: "Search weather by city", accept: "Search", cancel: "Cancel");
        if (response != null)
        {
            try
            {
                await GetWeatherDataByCity(response);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while searching for weather data: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Cancelled", "Weather search was cancelled.", "OK");
        }
    }

    public async Task GetWeatherDataByCity(string city)
    {
        try
        {
            var result = await ApiService.GetWeatherByCity(city);
            if (result != null)
            {
                UpdateUI(result);
            }
            else
            {
                await DisplayAlert("Error", "No weather data found for the specified city.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while fetching weather data: {ex.Message}", "OK");
        }
    }

    public void UpdateUI(dynamic result)
    {
        try
        {
            WeatherList.Clear();
            foreach (var item in result.list)
            {
                WeatherList.Add(item);
            }
            CvWeather.ItemsSource = WeatherList;

            LblCity.Text = result.city.name;
            LblWeatherDescription.Text = result.list[0].weather[0].description;
            LblTemperature.Text = result.list[0].main.temperature + "°C";
            LblHumidity.Text = result.list[0].main.humidity + "%";
            LblWind.Text = result.list[0].wind.speed + "km/h";
            ImgWeatherIcon.Source = result.list[0].weather[0].CustomIcon;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"An error occurred while updating the UI: {ex.Message}", "OK");
        }
    }
}
