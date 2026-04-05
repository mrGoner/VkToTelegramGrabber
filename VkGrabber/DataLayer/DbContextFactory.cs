namespace VkGrabber.DataLayer;

public class DbContextFactory(string pathToDb)
{
    public GrabberDbContext CreateContext()
    {
        return new GrabberDbContext(pathToDb);
    }
}