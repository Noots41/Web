using Model;


namespace Services
{
    public interface IDocumentRepository : IEntityRepository<Document>
    {
        Document Create(string name, Author author);
        bool CheckDoc(string fileName);
    }
}
