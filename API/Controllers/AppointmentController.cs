using API.Repositories;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/appointments")]
[ApiController]
public class AppointmentController(IRepository<Appointment> appointmentRepository) : Controller
{
    [HttpPost]
    public async Task<ActionResult> PlaceAppointment(Appointment appointment)
    {
        appointmentRepository.Insert(appointment);
        await appointmentRepository.SaveAsync();

        return Created(HttpContext.Request.Path.ToString(), appointment);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments([FromQuery] string name)
    {
        var appointments = await appointmentRepository.GetAsync(
            filter: a => a.PatientName.ToLower() == name.ToLower(),
            includes: a => a.Doctor
        );

        return appointments.ToList();
    }
}