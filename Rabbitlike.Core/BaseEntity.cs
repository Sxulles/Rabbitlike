using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Rabbitlike.Core
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime LastModificationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }
        public Guid LastModifierId { get; set; }
        public Guid CreatorId { get; set; }
        public byte[] RowVersion { get; set; }
    }
    public abstract class BaseEntityMap<T> : IEntityMap, IEntityTypeConfiguration<T> where T : class, IBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.RowVersion).IsRowVersion();
        }

        public virtual void ConfigureKeys(ModelBuilder builder)
        {

        }

        public virtual void Seeding(ModelBuilder builder)
        {

        }

        public void ApplyConfiguration(ModelBuilder builder)
        {
            ConfigureKeys(builder);
            builder.ApplyConfiguration(this);
        }
    }
}
