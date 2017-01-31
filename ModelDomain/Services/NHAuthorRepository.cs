using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Helpers;
using NHibernate.Criterion;
using NHibernate;

namespace Services
{
    public class NHAuthorRepository : IAuthorRepository
    {
        Author IEntityRepository<Author>.Create(string Name)
        {
            throw new NotImplementedException();
        }

        Author IAuthorRepository.Create(string firstName, string lastName, string login, string password)
        {
            Author newUser;
            using (var session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        //The pseudo-syntax for INSERT statements is: INSERT INTO EntityName properties_list select_statement.Some points to note:
                        //Only the INSERT INTO...SELECT...form is supported; not the INSERT INTO ... VALUES...form.
                        //select_statement can be any valid HQL select query
                        newUser = new Author { FirstName = firstName, LastName = lastName, Login = login, Password = password };
                        session.Save(newUser);
                        transaction.Commit();
                    }
                    catch (HibernateException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return newUser;
        }

        bool IEntityRepository<Author>.Delete(int Id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Author> IEntityRepository<Author>.GetAll()
        {
            var users = new List<Author>();
            using (var session = NHibernateHelper.OpenSession())
            {
                var criteria = session.CreateCriteria(typeof(Author));
                users = criteria.List<Author>().ToList();
            }
            return users;
        }

        Author IEntityRepository<Author>.Load(int Id)
        {
            throw new NotImplementedException();
        }

        void IEntityRepository<Author>.Update(Author newDoc)
        {
            throw new NotImplementedException();
        }

        public bool CheckUser(string login, string password)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var count = session.QueryOver<Author>().And(x => x.Login == login && x.Password == password).RowCount();
                return count == 1;
            }
        }

    }
}
