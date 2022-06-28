using Microsoft.EntityFrameworkCore;

namespace HWStorageAPI.Model
{
    public class DeviceContext : DbContext
    {
        public DeviceContext(DbContextOptions<DeviceContext> options)
            : base(options) { }

        public DbSet<Device> Devices => Set<Device>();
    }
}
