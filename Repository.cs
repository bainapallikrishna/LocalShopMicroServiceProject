 
public class Repository<T> where T : class : IRepository<T>  
{  
    protected readonly AppDbContext _context;  

    public virtual Task<IEnumerable<T>> GetAllAsync();  
    public virtual Task<T?> GetByIdAsync(int id);  
    public virtual Task AddAsync(T entity);  
    public virtual Task UpdateAsync(T entity);  
    public virtual Task DeleteAsync(int id);  
}  
