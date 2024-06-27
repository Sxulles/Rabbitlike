using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Rabbitlike.Core;
using Rabbitlike.Service.Extensions;
using Rabbitlike.Utils;
using Rabbitlike.Utils.Extensions;

namespace Rabbitlike.Service.Repository
{
    public interface IBaseRepository<T> where T : class, IBaseEntity, new()
    {
        Task<string> DatabaseSaveAsync();
        string DatabaseSave();

        T? GetByGuid(Guid guid);
        Task<T?> GetByGuidAsync(Guid guid);
        T? GetByGuidWithAutoInclude(Guid guid, bool withDeleted = false);
        Task<T?> GetByGuidWithAutoIncludeAsync(Guid guid, bool withDeleted = false);

        IQueryable<T> Get(Expression<Func<T, bool>>? predicate = null, bool track = false, bool withDeleted = false, params string[] includeProperties);
        IQueryable<T> GetWithExpression(Expression<Func<T, bool>>? predicate = null, bool track = false, bool withDeleted = false, params Expression<Func<T, bool>>[] includeProperties);
        IQueryable<T> GetWithAutoInclude(Expression<Func<T, bool>>? predicate = null, bool withDeleted = false, bool track = false);

        Result<IEnumerable<T>> Upsert(bool withSave, params T[] entities);
        Result<IEnumerable<T>> Upsert(IEnumerable<T> entities, bool withSave = true);

        Result<T> DeleteByGuid(Guid guid, bool withSave = true, bool hardDelete = false);
        Result<IEnumerable<T>> Delete(bool hardDelete, bool withSave = true, params T[] entities);
        Result<IEnumerable<T>> Delete(IEnumerable<T> entities, bool hardDelete = false, bool withSave = true);
    }
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseEntity, new()
    {
        private DbContext _DbContext { get; }
        protected readonly DbSet<T> _DbSet;
        private readonly ClaimsPrincipal _ClaimsPrincipal;
        private readonly Guid _UserId;
        protected int _BatchSize = 250;

        public BaseRepository(DbContext dbContext, ClaimsPrincipal claimsPrincipal)
        {
            _DbContext = dbContext;
            _ClaimsPrincipal = claimsPrincipal;
            _DbSet = _DbContext.Set<T>();
            _UserId = _ClaimsPrincipal.GetUserId();
        }

        #region DB Save
        public async Task<string> DatabaseSaveAsync()
        {
            string _result = string.Empty;

            try
            {
                SetShadowProperties();
                await _DbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _result = e.Message;
                throw;
            }

            return _result;
        }

        public string DatabaseSave()
        {
            string _result = string.Empty;

            try
            {
                SetShadowProperties();
                _DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _result = e.Message;
                throw;
            }

            return _result;
        }
        #endregion

        #region Get By Id
        public T? GetByGuid(Guid guid)
        {
            return _DbSet.Find(guid);
        }

        public async Task<T?> GetByGuidAsync(Guid guid)
        {
            return await _DbSet.FindAsync(guid);
        }

        public T? GetByGuidWithAutoInclude(Guid guid, bool withDeleted = false)
        {
            var _dbSet = _DbSet;

            foreach (var property in typeof(T).GetProperties().Where(x => x.IsPropertyVirtual()))
            {
                _dbSet.IncludeAs(property.Name, withDeleted);
            }

            return _dbSet.Find(guid);
        }

        public async Task<T?> GetByGuidWithAutoIncludeAsync(Guid guid, bool withDeleted = false)
        {
            var _dbSet = _DbSet;

            foreach (var property in typeof(T).GetProperties().Where(x => x.IsPropertyVirtual()))
            {
                _dbSet.IncludeAs(property.Name, withDeleted);
            }

            return await _dbSet.FindAsync(guid);
        }
        #endregion

        #region Get
        public virtual IQueryable<T> Get(Expression<Func<T, bool>>? predicate = null, bool track = false, bool withDeleted = false, params string[] includeProperties)
        {
            var _query = _DbSet.AsQueryable();

            _query = _query.SetQueryProperties(predicate, track, withDeleted);

            if (includeProperties.Any())
            {
                foreach (var property in includeProperties)
                {
                    _query = _query.IncludeAs(property, withDeleted);
                }
            }

            return _query;
        }

        public virtual IQueryable<T> GetWithExpression(Expression<Func<T, bool>>? predicate = null, bool track = false, bool withDeleted = false, params Expression<Func<T, bool>>[] includeProperties)
        {
            var _query = _DbSet.AsQueryable();

            _query = _query.SetQueryProperties(predicate, track, withDeleted);

            if (includeProperties.Any())
            {
                foreach (var property in includeProperties)
                {
                    _query = _query.IncludeAs(property, withDeleted);
                }
            }

            return _query;
        }

        public virtual IQueryable<T> GetWithAutoInclude(Expression<Func<T, bool>>? predicate = null, bool withDeleted = false, bool track = false)
        {
            var _query = _DbSet.AsQueryable();

            _query = _query.SetQueryProperties(predicate, track, withDeleted);

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.IsPropertyVirtual())
                    _query = _query.IncludeAs(property.Name, withDeleted);
            }

            return _query;
        }
        #endregion

        #region Update
        public virtual Result<IEnumerable<T>> Upsert(bool withSave, params T[] entities)
        {
            return Upsert(entities, withSave);
        }

        public virtual Result<IEnumerable<T>> Upsert(IEnumerable<T> entities, bool withSave = true)
        {
            return UpsertEntities(entities, withSave);
        }

        private Result<IEnumerable<T>> UpsertEntities(IEnumerable<T> entities, bool withSave = true, int currentBatchFrom = 0)
        {
            if (!entities.Any() || entities.Any(x => x is null))
                throw new ArgumentNullException();

            try
            {
                var _currentBatch = entities.Skip(currentBatchFrom).Take(_BatchSize);
                var _nextBatchFrom = currentBatchFrom + _BatchSize;

                var _existingEntities = Get(x => _currentBatch.Select(y => y.Id).Contains(x.Id), false).ToList();
                var _nonExistingEntities = _currentBatch.Where(x => !_existingEntities.Select(y => y.Id).Contains(x.Id));

                if (_nonExistingEntities.Any())
                    _DbSet.AddRange(_nonExistingEntities);

                _DbSet.UpdateRange(_existingEntities);

                if(withSave) DatabaseSave();

                if (_nextBatchFrom < entities.Count())
                    return UpsertEntities(entities, withSave, _nextBatchFrom);

                return new Result<IEnumerable<T>>();
            }
            catch (Exception e)
            {
                return new Result<IEnumerable<T>>(e.Message);
            }
        }
        #endregion

        #region Delete
        public Result<T> DeleteByGuid(Guid guid, bool withSave = true, bool hardDelete = false)
        {
            var _result = Delete(hardDelete, withSave, GetByGuid(guid)!);
            return new Result<T> { Data = _result.Data.FirstOrDefault()!, Message = _result.Message, Succeeded = _result.Succeeded };
        }

        public Result<IEnumerable<T>> Delete(bool hardDelete, bool withSave = true, params T[] entities)
        {
            return Delete(entities as List<T>?? [], hardDelete, withSave);
        }

        public Result<IEnumerable<T>> Delete(IEnumerable<T> entities, bool hardDelete = false, bool withSave = true)
        {
            return DeleteEntities(entities, hardDelete, withSave);
        }

        private Result<IEnumerable<T>> DeleteEntities(IEnumerable<T> entities, bool logicalDelete = true, bool withSave = true, int currentBatchFrom = 0)
        {
            if (!entities.Any() || entities.Any(x => x is null))
                throw new ArgumentNullException();

            try
            {
                var _currentBatch = entities.Skip(currentBatchFrom).Take(_BatchSize);
                var _nextBatchFrom = currentBatchFrom + _BatchSize;

                if (logicalDelete)
                {
                    foreach (var entity in _currentBatch)
                    {
                        entity.DeletionDate = DateTime.Now;
                        entity.LastModifierId = _UserId;
                    }

                    _DbSet.UpdateRange(_currentBatch);
                }
                else _DbSet.RemoveRange(_currentBatch);

                if(withSave) DatabaseSave();

                if (_nextBatchFrom < entities.Count())
                    return DeleteEntities(entities, logicalDelete, withSave, _nextBatchFrom);

                return new Result<IEnumerable<T>>(entities);
            }
            catch (Exception e)
            {
                return new Result<IEnumerable<T>>(e.Message);
            }
        }
        #endregion

        private void SetShadowProperties()
        {
            var entries = _DbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State is EntityState.Added)
                {
                    entityEntry.Property(nameof(BaseEntity.CreationDate)).CurrentValue = DateTime.Now;
                    entityEntry.Property(nameof(BaseEntity.CreatorId)).CurrentValue = _UserId;
                }
                else if (entityEntry.State is EntityState.Modified)
                {
                    entityEntry.Property(nameof(BaseEntity.LastModificationDate)).CurrentValue = DateTime.Now;
                    entityEntry.Property(nameof(BaseEntity.LastModifierId)).CurrentValue = _UserId;
                }
            }
        }
    }
}
