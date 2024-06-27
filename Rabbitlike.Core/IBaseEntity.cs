namespace Rabbitlike.Core
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime LastModificationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }
        public Guid LastModifierId { get; set; }
        public Guid CreatorId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
