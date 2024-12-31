using API.Repositories;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/medicines")]
[ApiController]
public class MedicineController(IRepository<Medicine> medicineRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicines()
    {
        var medicines = await medicineRepository.GetAsync();
        return medicines.ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Medicine>> GetMedicine(int id)
    {
        var medicine = await medicineRepository.GetByIdAsync(id);

        if (medicine == null)
        {
            return NotFound();
        }

        return medicine;
    }
}