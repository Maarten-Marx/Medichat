namespace API.Data;

public class DbInitializer
{
    public static void Initialize(MedichatContext context)
    {
        context.Database.EnsureCreated();
    }
}