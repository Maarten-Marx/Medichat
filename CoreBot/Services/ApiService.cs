using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoreBot.Services;

public class ApiService(IConfiguration configuration)
{
    private readonly string _baseUrl = configuration["ApiHost"];
    private readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(60) };

    public async Task<T> GetAsync<T>(string endPoint)
    {
        string url = _baseUrl + endPoint;
        var response = await _client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            throw new Exception("Resource Not Found");
        }

        throw new Exception("Request failed with status code " + response.StatusCode);
    }

    public async Task PostAsync<T>(string endPoint, T data)
    {
        string url = _baseUrl + endPoint;
        var response = await _client.PostAsJsonAsync(url, data);
        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Request failed with status code " + response.StatusCode);
        }
    }
}