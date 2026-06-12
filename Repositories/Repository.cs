using LogicaServidor.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogicaServidor.Repositories;

public class Repository<T> where T : class
{
    public Repository(SensoresTrinityContext context)
    {
        Context = context;
    }
    public SensoresTrinityContext Context { get; }
    public virtual T? Get(object id)
    {
        return Context.Find<T>(id);
    }
    public virtual IEnumerable<T> GetAll()
    {
        return Context.Set<T>().AsQueryable();
    }
    public virtual void Insert(T entity)
    {
        Context.Add(entity);
        Context.SaveChanges();
    }
    public virtual void Insert(IEnumerable<T> entity)
    {
        Context.AddRange(entity);
        Context.SaveChanges();
    }
    public virtual void Update(T entity)
    {
        Context.Update(entity);
        Context.SaveChanges();
    }
    public virtual void Update(IEnumerable<T> entity)
    {
        Context.UpdateRange(entity);
        Context.SaveChanges();
    }
    public virtual void Delete(int id)
    {
        var entity = Context.Find<T>(id);
        if (entity != null)
        {
            Context.Remove(entity);
            Context.SaveChanges();
        }
    }
    public virtual void Delete(IEnumerable<int> ids)
    {
        var entities = Context.Set<T>().Where(e => ids.Contains(EF.Property<int>(e, "Id"))).ToList();
        Context.RemoveRange(entities);
        Context.SaveChanges();
    }
}