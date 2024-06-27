using Microsoft.EntityFrameworkCore;

namespace Rabbitlike.Core
{
    public interface IEntityMap
    {
        void ApplyConfiguration(ModelBuilder builder);
        void Seeding(ModelBuilder builder);
    }
}
