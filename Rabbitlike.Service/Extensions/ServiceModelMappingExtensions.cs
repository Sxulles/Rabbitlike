using Rabbitlike.Core;
using Rabbitlike.Service.Model;
using Rabbitlike.Utils.Mapping;

namespace Rabbitlike.Service.Extensions
{
    public static class ServiceModelMappingExtensions
    {
        public static T? ToServiceModel<T>(this IBaseEntity? entity) where T : IServiceModel<IBaseEntity>
        {
            return entity is not null ? entity.ToModel<T>() : default;
        }

        public static List<T> ToServiceModelList<T>(this IQueryable<IBaseEntity>? entities) where T : IServiceModel<IBaseEntity>
        {
            return [..entities?.Select(ToServiceModel<T>).AsEnumerable()];
        }

        public static List<T> ToServiceModelList<T>(this IEnumerable<IBaseEntity>? entities) where T : IServiceModel<IBaseEntity>
        {
            return [..entities?.Select(ToServiceModel<T>).AsEnumerable()];
        }

        public static List<T> ToEntityList<T>(this IEnumerable<IServiceModel<T>> serviceModels) where T : IBaseEntity
        {
            return [..serviceModels.Select(x => x.ToEntity())];
        }
    }
}
