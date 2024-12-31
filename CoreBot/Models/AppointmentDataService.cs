using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Services;
using Data.Models;

namespace CoreBot.Models;

public class AppointmentDataService(ApiService apiService)
{
    public async Task<List<Appointment>> GetAppointmentsByName(string name)
    {
        return await apiService.GetAsync<List<Appointment>>($"appointments?name={name}");
    }

    public async Task InsertAppointmentAsync(Appointment appointment)
    {
        await apiService.PostAsync("appointments", appointment);
    }
}