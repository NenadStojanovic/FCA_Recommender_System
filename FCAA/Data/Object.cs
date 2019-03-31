using System;

namespace FCAA.Data
{
    public class Object
    {
        /// <summary>
        /// Object id. Initially random.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Object name.
        /// </summary>
        public string Name { get; set; }

        public Object(string name)
        {
            Name = name;
            //Id = Guid.NewGuid().ToString();
        }
    }
}
