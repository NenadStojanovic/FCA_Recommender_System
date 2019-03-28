using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = FCAA.Data.Object;
using Attribute = FCAA.Data.Attribute;

namespace FCAA.Data
{
    public class PreprocessingAttribute : Attribute
    {
        public PreprocessingAttribute(string name) : base(name)
        {
        }

        public override bool Equals(object obj)
        {
            var attribute = obj as Attribute;
            return attribute != null &&
                   Name == attribute.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
