using AutoMapper;

namespace Rabbitlike.Utils.Mapping
{
    public interface IMapperBase
    {
        public virtual void Mapping(IMapperConfigurationExpression cfg)
        {
        }
    }
    public class MapperBase : IMapperBase
    {
        public virtual void Mapping(IMapperConfigurationExpression cfg)
        {
        }
    }
}
