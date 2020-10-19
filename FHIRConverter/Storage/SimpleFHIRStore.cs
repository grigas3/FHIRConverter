using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FHIRConverter.Storage
{
   /// <summary>
   /// A simple FHIR bundle store to file implementation
   /// </summary>
    internal class SimpleFHIRStore : IFHIRRepository
    {
        Bundle b = new Bundle();

        public void StoreResource(string resourceName, object resource, string url)
        {
            if (resource is Resource)
            {
                var s = (resource as Resource).ToJson();
                b.AddResourceEntry((resource as Resource), url);
            }
        }

        public void SaveJson(string file)
        {
            File.WriteAllText(file, b.ToJson());
        }
    }
}
