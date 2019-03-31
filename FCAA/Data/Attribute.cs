using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FCAA.Data
{
    public class Attribute
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Attribute(string name)
        {
            Name = name;
            //Id = Guid.NewGuid().ToString(); // Slows down attribute load
        }
    }
}
