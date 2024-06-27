using Rabbitlike.Core;
using Rabbitlike.Utils.Mapping;

namespace Rabbitlike.Service.Model
{
    public interface IServiceModel<T>
    {
        T ToEntity();
    }

    public class BaseServiceModel<T> : MapperBase, IServiceModel<T> where T : IBaseEntity, new()
    {
        public Guid Id { get; set; }

        public virtual T ToEntity()
        {
            return this.ToModel<T>();
        }
    }
}
