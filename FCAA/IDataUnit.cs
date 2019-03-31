using FCAA.Data;
using FCAA.Data.Lattice;
using FCAA.FormalConceptAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = FCAA.Data.Attribute;
using Object = FCAA.Data.Object;

namespace FCAA
{

    public class FormalConceptApparatus
    {
        public IFormalConceptAlgorithm FormalConceptAlgorithm { get; set; }
    }

    public interface IDataImportUnit
    {
        FormalContext ImportContext();
        ICollection<Attribute> ImportContextAttributes();
        ICollection<Object> ImportContextObjects();
        ConceptLattice ImportConceptLattice();
    }

    public interface IDataExportUnit
    {
        void ExportContext(FormalContext context);
        void ExportObjects(ICollection<Object> objects);
        void ExportAttributes(ICollection<Attribute> attributes);
        void ExportConceptLattice(ConceptLattice lattice);
    }

    public interface IDataPreprocesUnit
    {
        FormalContext PreprocessContext();
        ICollection<Attribute> PreprocessAttributes();
        ICollection<Object> PreprocessObjects();
    }

    public interface IFormlaConceptsUnit
    {
        ICollection<FormalConcept> FormalConcepts();
    }

    public interface IConceptLatticeUnit
    {
        ConceptLattice FormalConceptLattice();
    }

    public class ImportDataUnit
    {
        public FormalContext ImportContextFromFile(string file)
        {
            throw new NotImplementedException();
        }
        public ICollection<Attribute> ImportAttributesFromFile(string file)
        {
            throw new NotImplementedException();
        }
        public ICollection<Object> ImportObjectsFromFile(string file)
        {
            throw new NotImplementedException();
        }

        public Task<FormalContext> ImportContextFromFileAsync(string file)
        {
            throw new NotImplementedException();
        }
        public Task<ICollection<Attribute>> ImportAttributesFromFileAsync(string file)
        {
            throw new NotImplementedException();
        }
        public Task<ICollection<Object>> ImportObjectsFromFileAsync(string file)
        {
            throw new NotImplementedException();
        }
    }

    public class ExportDataUnit
    {
        public string FormalConceptLatticeToCSV(ConceptLattice lattice)
        {
            throw new NotImplementedException();
        }

        public void FormalConceptLatticeToFile(ConceptLattice lattice, string file)
        {
            throw new NotImplementedException();
        }

        public void FormalConceptLatticeToNeo4jDataBase(ConceptLattice lattice)
        {
            throw new NotImplementedException();
        }
    }
}
