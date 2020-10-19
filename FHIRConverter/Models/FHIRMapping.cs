using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FHIRConverter.Models
{
    /// <summary>
    ///     Main Model for Mapping of a CSV file in FHIR resources
    /// </summary>
    public class FHIRMapping
    {
      
        /// <summary>
        ///     Base Resource Id
        ///     Interpolation of Row Properties
        /// </summary>
        public string BaseResourceId { get; set; }

        /// <summary>
        ///     Input File to Output FHIR Resource Mapping
        /// </summary>
        public List<FHIRPropertyMapping> PropertyMapping { get; set; }

    
        /// <summary>
        ///     Base Reference Url
        /// </summary>
        public string BaseReferenceUrl { get; set; }

      /// <summary>
      /// To Json
      /// </summary>
      /// <param name="file"></param>
        public void ToJson(string file)
        {
            var s = JsonConvert.SerializeObject(this);
            File.WriteAllText(file, s);
        }
        /// <summary>
        /// From Json
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FHIRMapping FromJson(string file)
        {
            var jsonTxt = File.ReadAllText(file);
            var s = JsonConvert.DeserializeObject<FHIRMapping>(jsonTxt);
            return s;
        }
    }
}