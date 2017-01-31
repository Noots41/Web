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
    public class NHDocumentRepository : IDocumentRepository
    {

        Document IDocumentRepository.Create(string name, Author author)
        {
            var newDoc = new Document();
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        newDoc = new Document { Name = name, Author = author, Date = DateTime.Now };
                        session.Save(newDoc);
                        transaction.Commit();
                    }
                    catch (HibernateException)
                    {
                        throw;
                    }
                }
            }
            return newDoc;
        }

        bool IEntityRepository<Document>.Delete(int Id)
        {
            throw new NotImplementedException();
        }

        Document IEntityRepository<Document>.Load(int Id)
        {
            var doc = new Document();
            string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
            using (var session = NHibernateHelper.OpenSession())
            {
                var criteria = session.CreateCriteria(typeof(Document));
                criteria.Add(Restrictions.Eq("Id", Id));
                criteria.SetMaxResults(1);
                doc = criteria.List<Document>().FirstOrDefault();
            }
            return doc;
        }

        void IEntityRepository<Document>.Update(Document newDoc)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(newDoc);
                    }
                    catch (Exception e)
                    {
                        //вывод е в лог
                        transaction.Rollback();
                        throw;
                    }
                    transaction.Commit();
                }

            }
        }

        public IEnumerable<Document> GetAll()
        {
            var docs = new List<Document>();
            using (var session = NHibernateHelper.OpenSession())
            {
                var criteria = session.CreateCriteria(typeof(Document));
                docs = criteria.List<Document>().ToList();
                foreach (var doc in docs)
                {
                    if (doc.Name.Length > 30)
                        doc.Name = doc.Name.Remove(30) + "...";
                }
            }
            return docs;
        }

        Document IEntityRepository<Document>.Create(string Name)
        {
            throw new NotImplementedException();
        }

        public bool CheckDoc(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var count = session.QueryOver<Document>().And(x => x.Name == name).RowCount();
                return count == 1;
            }
        }
    }
}
