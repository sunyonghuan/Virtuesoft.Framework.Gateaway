namespace Virtuesoft.Framework.Gateaway.Sample.Data;

public class DataContext:DbContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public DataContext(DbContextOptions options) : base(options)
    {
        if (Database.EnsureCreated())
        {
            Roles.Add(new Role()
            {
                Name = "VIP"
            });
            SaveChanges();
        }
    }

    public DbSet<MemberAccount> Accounts { get; set; }

    public DbSet<Role> Roles { get; set; }
}

