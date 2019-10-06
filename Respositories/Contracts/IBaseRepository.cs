using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public interface IBaseRepository<Entity> where Entity : BaseEntity
{
    Task<ICollection<Entity>> GetAll(bool trackEntities = true);
    Task<bool> Exists(int id, bool trackEntities = true);
    Task<Entity> Get(int id, bool trackEntities = true);

    Task<Entity> Add(Entity entityToAdd);

    Task Update(Entity entityToUpdate, bool saveChanges = true);

    Task<Entity> Delete(int id, bool saveChanges = true);

    DbSet<Entity> Entities { get; }
    IQueryable<Entity> EntitiesAsNoTracking { get; }

    Task SaveChanges();
}