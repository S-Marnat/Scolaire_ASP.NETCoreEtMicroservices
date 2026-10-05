using Front.ViewModels;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Front.Services
{
    public class PatientApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PatientListViewModel>> GetAllPatientsAsync()
        {
            var response = await SendWithRefreshAsync(() => _httpClient.GetAsync("/patient"));

            if (!response.IsSuccessStatusCode)
                return new List<PatientListViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PatientListViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PatientListViewModel>();
        }

        public async Task<PatientDetailsViewModel?> GetPatientByIdAsync(int id)
        {
            var response = await SendWithRefreshAsync(() => _httpClient.GetAsync($"/patient/{id}"));

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PatientDetailsViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> CreatePatientAsync(PatientCreateViewModel patient)
        {
            var response = await SendWithRefreshAsync(() => _httpClient.PostAsJsonAsync("/patient", patient));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientEditViewModel patient)
        {
            var response = await SendWithRefreshAsync(() => _httpClient.PutAsJsonAsync($"/patient/{id}", patient));
            return response.IsSuccessStatusCode;
        }


        // -- Méthodes utilitaires pour gérer le JWT et le RefreshToken --
        private void AddJwtHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<bool> TryRefreshTokenAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync("/account/refresh", new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TokenResponseViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
                return false;

            _httpContextAccessor.HttpContext.Session.SetString("AccessToken", data.Token);
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", data.RefreshToken);

            return true;
        }

        private async Task<HttpResponseMessage> SendWithRefreshAsync(Func<Task<HttpResponseMessage>> action)
        {
            AddJwtHeader();

            var response = await action();

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // Tentative de refresh
            var refreshed = await TryRefreshTokenAsync();

            if (!refreshed)
                return response;

            // Rejouer la requête
            AddJwtHeader();
            return await action();
        }
    }
}
