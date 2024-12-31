namespace Data.Models;

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string Contact { get; set; }
    public string Reason { get; set; }
    public string Time { get; set; }

    public int DoctorId { get; set; }

    public Doctor? Doctor { get; set; }
}