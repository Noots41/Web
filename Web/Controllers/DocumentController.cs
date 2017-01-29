using Helpers;
using Model;
using NHibernate;
using NHibernate.Criterion;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DocumentController : Controller
    {
        private IDocumentRepository repository { get; set; }

        public DocumentController()
        {
            repository = new NHDocumentRepository();
        }


        // GET: Document
        public ActionResult Index()
        {
            var docs = repository.GetAll().OrderByDescending(it => it.Id);//.Where(o => o.ExetTimeMs > 1000);
            return View(docs);
        }

        public FilePathResult GetFile(int id)
        {
            string fileName = "";
            string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
            using (var session = NHibernateHelper.OpenSession())
            {
                var criteria = session.CreateCriteria(typeof(Document));
                criteria.Add(Restrictions.Eq("Id", id));
                criteria.SetMaxResults(1);
                var doc = criteria.List<Document>().FirstOrDefault();
                fileName = doc.Name;
            }
            return File(path + fileName, "text/plain", fileName);
        }

        public ActionResult Create()
        {
            foreach (string upload in Request.Files)
            {
                if (!(Request.Files[upload] != null && Request.Files[upload].ContentLength > 0)) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                string filename = Path.GetFileName(Request.Files[upload].FileName);
                Request.Files[upload].SaveAs(Path.Combine(path, filename));

                string mimeType = Request.Files[upload].ContentType;
                Stream fileStream = Request.Files[upload].InputStream;
                string fileName = Path.GetFileName(Request.Files[upload].FileName);
                int fileLength = Request.Files[upload].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);

                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        try
                        {
                            var newDoc = new Document { Name = fileName, Author = session.Get<Author>(1), Date = DateTime.Now };
                            session.Save(newDoc);
                            transaction.Commit();
                        }
                        catch (HibernateException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                fileStream.Close();
            }
            return View();
        }


    }
}