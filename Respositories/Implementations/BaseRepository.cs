﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : BaseEntity
{
    private readonly ApiDbContext _apiDBContext;
    private readonly IMemoryCache _memoryCache;

    public BaseRepository(ApiDbContext apiDBContext, IMemoryCache memoryCache)
    {
        _apiDBContext = apiDBContext;
        _memoryCache = memoryCache;
    }
    
    public async Task<ICollection<Entity>> GetAll(bool trackEntities = true)
    {
        var _set = trackEntities ? Entities : EntitiesAsNoTracking;

        return await _set.ToListAsync();
    }

    public async Task<bool> Exists(Expression<Func<Entity, bool>> predicate, bool trackEntities = true)
    {
        var _set = trackEntities ? Entities : EntitiesAsNoTracking;

        return await _set.AnyAsync(predicate);
    }

    public async Task<Entity> Get(Expression<Func<Entity, bool>> predicate, bool trackEntities = true)
    {
        /* var inMemoryEntity = _memoryCache.Get<Entity>(id);

        if (inMemoryEntity != null)
            return inMemoryEntity; */
        
        var _set = trackEntities ? Entities : EntitiesAsNoTracking;

        var retrievedEntity = await _set.FirstOrDefaultAsync(predicate);

        /* var memoryCacheEntryOption = new MemoryCacheEntryOptions()
                                            .SetSlidingExpiration(TimeSpan.FromSeconds(10));
        _memoryCache.Set<Entity>(id, retrievedEntity, memoryCacheEntryOption); */

        return retrievedEntity;
    }

    public async Task<Entity> Add(Entity entityToAdd)
    {
        Entities.Add(entityToAdd);
        await _apiDBContext.SaveChangesAsync();

        return entityToAdd;
    }

    public async Task Update(Entity entityToUpdate, bool saveChanges = true)
    {
        _apiDBContext.Entry<Entity>(entityToUpdate).State = EntityState.Modified;
        
        if(saveChanges)
            await _apiDBContext.SaveChangesAsync();
    }

    public async Task<Entity> Delete(Expression<Func<Entity, bool>> predicate, bool saveChanges = true)
    {
        var entityToDelete = await Entities.FirstOrDefaultAsync(predicate);

        if (entityToDelete == null)
            return null;
        
        Entities.Remove(entityToDelete);

        if(saveChanges)
            await _apiDBContext.SaveChangesAsync();

        return entityToDelete;
    }

    public DbSet<Entity> Entities 
    { 
        get { return _apiDBContext.Set<Entity>(); } 
    }

    public IQueryable<Entity> EntitiesAsNoTracking 
    { 
        get { return Entities.AsNoTracking(); } 
    }

    public async Task SaveChanges()
    {
        await _apiDBContext.SaveChangesAsync();
    }
}