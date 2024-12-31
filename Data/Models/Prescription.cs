namespace Data.Models;

public class Prescription
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string Reason { get; set; }

    public int MedicineId { get; set; }

    public Medicine? Medicine { get; set; }
}