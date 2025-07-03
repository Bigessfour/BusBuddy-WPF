using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Bus_Buddy.Data.Interfaces;
using Bus_Buddy.Models.Base;

namespace Bus_Buddy.Data.Repositories;

/// <summary>
/// Generic repository implementation for common CRUD operations
/// Supports both async and sync operations for Syncfusion compatibility
/// Includes soft delete functionality and audit tracking
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BusBuddyDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly bool _supportsSoftDelete;

    public Repository(BusBuddyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
        _supportsSoftDelete = typeof(BaseEntity).IsAssignableFrom(typeof(T));
    }

    #region Async Query Operations

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.AnyAsync();
    }

    public virtual async Task<int> CountAsync()
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.CountAsync();
    }

    #endregion

    #region Synchronous Query Operations

    public virtual T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public virtual T? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public virtual IQueryable<T> GetAllQueryable()
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query;
    }

    public virtual IEnumerable<T> GetAll()
    {
        return GetAllQueryable().ToList();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query.ToList();
    }

    public virtual T? FirstOrDefault(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query.FirstOrDefault();
    }

    public virtual bool Any(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query.Any();
    }

    public virtual int Count()
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query.Count();
    }

    public virtual int Count(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = _dbSet.Where(expression);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query.Count();
    }

    #endregion

    #region Modification Operations

    public virtual async Task<T> AddAsync(T entity)
    {
        SetAuditFields(entity, isUpdate: false);
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            SetAuditFields(entity, isUpdate: false);
        }
        await _dbSet.AddRangeAsync(entityList);
        return entityList;
    }

    public virtual T Add(T entity)
    {
        SetAuditFields(entity, isUpdate: false);
        var result = _dbSet.Add(entity);
        return result.Entity;
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            SetAuditFields(entity, isUpdate: false);
        }
        _dbSet.AddRange(entityList);
    }

    public virtual void Update(T entity)
    {
        SetAuditFields(entity, isUpdate: true);
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            SetAuditFields(entity, isUpdate: true);
        }
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<bool> RemoveByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        Remove(entity);
        return true;
    }

    public virtual async Task<bool> RemoveByIdAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        Remove(entity);
        return true;
    }

    #endregion

    #region Soft Delete Operations

    public virtual async Task<bool> SoftDeleteAsync(int id)
    {
        if (!_supportsSoftDelete) return false;

        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        SoftDelete(entity);
        return true;
    }

    public virtual async Task<bool> SoftDeleteAsync(object id)
    {
        if (!_supportsSoftDelete) return false;

        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        SoftDelete(entity);
        return true;
    }

    public virtual void SoftDelete(T entity)
    {
        if (!_supportsSoftDelete || entity is not BaseEntity baseEntity) return;

        baseEntity.IsDeleted = true;
        SetAuditFields(entity, isUpdate: true);
        _dbSet.Update(entity);
    }

    public virtual async Task RestoreAsync(int id)
    {
        if (!_supportsSoftDelete) return;

        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return;

        Restore(entity);
    }

    public virtual async Task RestoreAsync(object id)
    {
        if (!_supportsSoftDelete) return;

        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return;

        Restore(entity);
    }

    public virtual void Restore(T entity)
    {
        if (!_supportsSoftDelete || entity is not BaseEntity baseEntity) return;

        baseEntity.IsDeleted = false;
        SetAuditFields(entity, isUpdate: true);
        _dbSet.Update(entity);
    }

    #endregion

    #region Pagination Support

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var totalCount = await query.CountAsync();

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    #endregion

    #region Advanced Querying

    public virtual IQueryable<T> Query()
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query;
    }

    public virtual IQueryable<T> QueryNoTracking()
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return query;
    }

    public virtual async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        IQueryable<T> query = _dbSet;

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.Select(selector).ToListAsync();
    }

    public virtual async Task<IEnumerable<TResult>> SelectAsync<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector)
    {
        IQueryable<T> query = _dbSet.Where(filter);

        if (_supportsSoftDelete)
        {
            query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
        }

        return await query.Select(selector).ToListAsync();
    }

    #endregion

    #region Helper Methods

    private void SetAuditFields(T entity, bool isUpdate)
    {
        if (entity is not BaseEntity baseEntity) return;

        var currentUser = GetCurrentUser();
        var currentTime = DateTime.UtcNow;

        if (!isUpdate)
        {
            baseEntity.CreatedDate = currentTime;
            baseEntity.CreatedBy = currentUser;
        }
        else
        {
            baseEntity.UpdatedDate = currentTime;
            baseEntity.UpdatedBy = currentUser;
        }

        baseEntity.OnSaving();
    }

    private string? GetCurrentUser()
    {
        // This would typically get the current user from the security context
        // For now, return a default value or get from a service
        return "System"; // TODO: Implement proper user context
    }

    #endregion
}
