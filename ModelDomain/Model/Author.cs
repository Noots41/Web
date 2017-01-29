using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Author
    {
        
        public virtual int Id { get; set; }

        [MaxLength(50)]
        public virtual string Login { get; set; }

        [MaxLength(50)]
        public virtual string Password { get; set; }

        [MaxLength(50)]
        public virtual string FirstName { get; set; }

        [MaxLength(50)]
        public virtual string LastName { get; set; }
    }
}
