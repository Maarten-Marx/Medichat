using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Services;
using Data.Models;

namespace CoreBot.Models;

public class PrescriptionDataService(ApiService apiService)
{
    public async Task<List<Prescription>> GetPrescriptionsByName(string name)
    {
        return await apiService.GetAsync<List<Prescription>>($"prescriptions?name={name}");
    }

    public async Task InsertPrescriptionAsync(Prescription prescription)
    {
        await apiService.PostAsync("prescriptions", prescription);
    }
}