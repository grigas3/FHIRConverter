using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FHIRConverter.Models
{



    /// <summary>
    /// Main Model for Mapping of a CSV file in FHIR resources
    /// </summary>
    public class FHIRMapping
    {
        /// <summary>
        /// The input file has Header
        /// </summary>
        public bool Header { get; set; }


        /// <summary>
        /// Base Resource Id
        /// Interpolation of Row Properties
        /// </summary>
        public string BaseResourceId { get; set; }

        /// <summary>
        /// Input File to Output FHIR Resource Mapping
        /// </summary>
        public List<FHIRPropertyMapping> PropertyMapping { get; set; }

        /// <summary>
        /// Specification of Resource Hierarchy
        /// </summary>
        public List<FHIRHierarchy> Hierarchy { get; set; }

        /// <summary>
        /// Base Reference Url
        /// </summary>
        public string BaseReferenceUrl { get; set; }

        /// <summary>
        /// Reference Schema including properties like the Resource type and resource identifier
        /// </summary>
        public string ReferenceSchema { get; set; }

        public void ToJson(string file)
        {
            var s=JsonConvert.SerializeObject(this);
            File.WriteAllText(file,s);

        }
        public FHIRMapping FromJson(string file)
        {
            var jsonTxt=File.ReadAllText(file);
            var s = JsonConvert.DeserializeObject<FHIRMapping>(jsonTxt);
            return s;
        }
    }
}
