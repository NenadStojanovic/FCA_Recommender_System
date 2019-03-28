using FCAA.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Object = FCAA.Data.Object;
using Attribute = FCAA.Data.Attribute;

namespace FCAA.DataImport
{
    public interface IContextFileImporter
    {
        string Description { get; }

        FormalContext ImportContext(string filePath);
        ICollection<Attribute> ImportContextAttributes(string filePath);
        ICollection<Object> ImportContextObjects(string filePath);

        Task<FormalContext> ImportContextAsync(string filePath);
    }
}
