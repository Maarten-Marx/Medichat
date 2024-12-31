using API.Repositories;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/doctors")]
[ApiController]
public class DoctorController(IRepository<Doctor> doctorRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
    {
        var doctors = await doctorRepository.GetAsync();
        return doctors.ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(int id)
    {
        var doctor = await doctorRepository.GetByIdAsync(id);

        if (doctor == null)
        {
            return NotFound();
        }

        return doctor;
    }
}