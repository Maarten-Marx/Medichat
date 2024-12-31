using System.Collections.Generic;
using CoreBot.Services;
using System.Threading.Tasks;
using Data.Models;

namespace CoreBot.Models;

public class MedicineDataService(ApiService apiService)
{
    public async Task<List<Medicine>> GetMedicineAsync()
    {
        return await apiService.GetAsync<List<Medicine>>("medicines");
    }
}