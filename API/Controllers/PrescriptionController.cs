using API.Repositories;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/prescriptions")]
[ApiController]
public class PrescriptionController(IRepository<Prescription> prescriptionRepository) : Controller
{
    [HttpPost]
    public async Task<ActionResult> PlacePrescription(Prescription prescription)
    {
        prescriptionRepository.Insert(prescription);
        await prescriptionRepository.SaveAsync();

        return Created(HttpContext.Request.Path.ToString(), prescription);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions([FromQuery] string name)
    {
        var prescriptions = await prescriptionRepository.GetAsync(
            filter: p => p.PatientName.ToLower() == name.ToLower(),
            includes: p => p.Medicine
        );

        return prescriptions.ToList();
    }
}