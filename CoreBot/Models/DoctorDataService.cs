using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Services;
using Data.Models;

namespace CoreBot.Models;

public class DoctorDataService(ApiService apiService)
{
    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        return await apiService.GetAsync<List<Doctor>>("doctors");
    }

    public async Task<Doctor> GetDoctorByIdAsync(string id)
    {
        return await apiService.GetAsync<Doctor>($"doctors/{id}");
    }
}