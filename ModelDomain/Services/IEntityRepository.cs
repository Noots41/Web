using System.Collections.Generic;

namespace Services
{
    /// <summary>
    /// Общее хранилище данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityRepository<T> where T : class
    {
        T Create();
        T Load(int Id);
        bool Delete(int Id);
        void Update(T newDoc);
        IEnumerable<T> GetAll();
    }
}
