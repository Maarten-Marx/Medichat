namespace Data.Models;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }

    public ICollection<Appointment>? Appointments { get; set; }
}