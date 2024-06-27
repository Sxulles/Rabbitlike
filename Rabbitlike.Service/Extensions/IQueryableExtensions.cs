using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Rabbitlike.Core;

namespace Rabbitlike.Service.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> SetQuerySize<T>(this IQueryable<T> query, int? skip, int? take) where T : IBaseEntity
        {
            if (skip is not null)
                query = query.Skip((int)skip);
            if (take is not null)
                query = query.Take((int)take);

            return query;
        }

        public static IQueryable<T> SetQueryProperties<T>(this IQueryable<T> query, Expression<Func<T, bool>>? predicate = null, bool track = false, bool withDeleted = false) where T : class, IBaseEntity
        {
            if (predicate is not null)
                query = query.Where(predicate);

            if (!track)
                query = query.AsNoTracking();

            if (!withDeleted)
                query = query.Where(x => !x.DeletionDate.HasValue);

            return query;
        }

        public static IQueryable<T> IncludeAs<T>(this IQueryable<T> query, string navigationPropertyName, bool withDeleted = false) where T : class, IBaseEntity
        {
            query = query.Include(navigationPropertyName).Where(x => withDeleted || !x.DeletionDate.HasValue);

            return query;
        }

        public static IQueryable<T> IncludeAs<T>(this IQueryable<T> query, Expression<Func<T, bool>> navigationExpression, bool withDeleted = false) where T : class, IBaseEntity
        {
            query = query.Include(navigationExpression).Where(x => withDeleted || !x.DeletionDate.HasValue);

            return query;
        }
    }
}
