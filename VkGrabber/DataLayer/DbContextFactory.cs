namespace VkGrabber.DataLayer;

public class DbContextFactory
{
    private readonly string m_pathToDb;

    public DbContextFactory(string _pathToDb)
    {
        m_pathToDb = _pathToDb;
    }

    public GrabberDbContext CreateContext()
    {
        return new GrabberDbContext(m_pathToDb);
    }
}