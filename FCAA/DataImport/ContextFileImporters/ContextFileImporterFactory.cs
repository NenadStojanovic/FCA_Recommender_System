using FCAA.Data;
using FCAA.DataImport.ContextFileImporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAA.DataImport.ContextFileImporters
{
    public class ContextFileImporterFactory
    {
        public static IContextFileImporter ProduceImporter(ContextFileImporters importer)
        {
            switch (importer)
            {
                case ContextFileImporters.DefaultImporter:
                    return new DefaultContextFileImporter();
                case ContextFileImporters.Json_id_tags_Importer:
                    return new IdsTagsContextFileReader();
                case ContextFileImporters.Json_name_tags_with_preprocessing_Importer:
                    return new FCAFileReaderWithPreprocessing();
                default:
                    throw new Exception("File importer not supported.");
            }
        }
    }

    public enum ContextFileImporters
    {
        DefaultImporter,
        Json_id_tags_Importer,
        Json_name_tags_with_preprocessing_Importer
    }
}
