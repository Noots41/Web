using Model;

namespace Services
{
    public interface IAuthorRepository : IEntityRepository<Author>
    {
        Author Create(string firstName, string lastName, string login, string password);
        bool CheckUser(string login, string password);
    }
}
