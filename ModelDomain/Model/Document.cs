using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Document
    {
        public virtual int Id { get; set; }

        [MaxLength(50)]
        public virtual string Name { get; set; }
       
        public virtual DateTime Date { get; set; }

        public virtual int AuthorId { get; set; }

        public virtual Author Author { get; set; }


    }
}
