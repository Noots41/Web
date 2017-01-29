using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Helpers;

namespace Services
{
    public class NHDocumentRepository : IDocumentRepository
    {
        Document IEntityRepository<Document>.Create()
        {
            return new Document() { Id = 0 };
        }
        

        bool IEntityRepository<Document>.Delete(int Id)
        {
            throw new NotImplementedException();
        }

        Document IEntityRepository<Document>.Load(int Id)
        {
            throw new NotImplementedException();
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
                    if(doc.Name.Length > 29)
                    doc.Name = doc.Name.Remove(30) + "...";
                }
            }
            return docs;
        }
    }
}
