using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHIRConverter;
using FHIRConverter.Loaders;
using FHIRConverter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace FHIRConverterTests
{

    [TestClass]
    public class FHIRDatasetTests
    {



        private FHIRMapping CreateTestMapping()
        {
            var mappings = new List<FHIRPropertyMapping>()
            {
              

                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 0,
                    ColumnName = "PersonID",
                    ResourceId = "0000001001",
                    ResourcePath = "ResearchSubject/Id",
                    ResourceBaseType = "ResearchSubject",
                    ResourcePathTypes = "ResearchSubject/String",


                },
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 0,
                    ColumnName = "PersonID",
                    ResourceId = "0000001001",
                    ResourcePath = "ResearchSubject/Study/Reference",
                    ResourceBaseType = "ResearchSubject",
                    ResourcePathTypes = "ResearchSubject/ResourceReference/String",
                    ValueTemplate="http://www.silicofcm.com/fhir/researchStudy/study1",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "ResearchSubject/Study/Type",
                            ResourcePathTypes = "ResearchSubject/ResourceReference/String",
                            Value = "ResearchStudy"
                        }

                    }
                },
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 2,
                    ColumnName = "Age",
                    ResourceId = "{RowNumber}002002",
                    ResourcePath = "Observation/Value/Value",
                    ResourceBaseType = "Observation",
                    ResourcePathTypes = "Observation/SimpleQuantity/Double",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Code/Text",
                            ResourcePathTypes = "Observation/CodeableConcept/String",
                            Value = "Age"
                        }
                       
                    }
                },

                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 0,
                    ColumnName = "PersonID",
                    ResourceId = "{RowNumber}002002",
                    ResourcePath = "Observation/Subject/Reference",
                    ResourceBaseType = "Observation",
                    ResourcePathTypes = "Observation/ResourceReference/String",
                    ValueTemplate="http://www.silicofcm.com/fhir/researchSubject/{value}",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Subject/Type",
                            ResourcePathTypes = "Observation/ResourceReference/String",
                            Value = "ResearchSubject"
                        }

                    }
                },


                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 4,
                    ResourceId = "{RowNumber}002003",
                    ColumnName = "BP_Diastolic",
                    ResourceBaseType = "Observation",
                    ResourcePath = "Observation/Value/Value",
                    ResourcePathTypes = "Observation/SimpleQuantity/Double",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Code/Text",
                            ResourcePathTypes = "Observation/CodeableConcept/String",
                            Value = "Diastolic Blood Pressure"
                        }
                      
                    }
                },
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 0,
                    ColumnName = "PersonID",
                    ResourceId = "{RowNumber}002003",
                    ResourcePath = "Observation/Subject/Reference",
                    ResourceBaseType = "Observation",
                    ResourcePathTypes = "Observation/ResourceReference/String",
                    ValueTemplate="http://www.silicofcm.com/fhir/researchSubject/{value}",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Subject/Type",
                            ResourcePathTypes = "Observation/ResourceReference/String",
                            Value = "ResearchSubject"
                        }

                    }
                },
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 1,
                    ResourceId = "{RowNumber}002003",
                    ColumnName = "Encounter_Date",
                    ResourceBaseType = "Observation",
                    ResourcePath = "Observation/Issued",
                    ResourcePathTypes = "Observation/DateTimeOffset",
                   
                },
               
                new FHIRPropertyMapping()
                {
                ColumnType = "string",
                ColumnIndex = 1,
                ResourceId = "{RowNumber}002002",
                ColumnName = "Encounter_Date",
                ResourceBaseType = "Observation",
                ResourcePath = "Observation/Issued",
                ResourcePathTypes = "Observation/DateTimeOffset",

            }

            };

            var mapping = new FHIRMapping();
            mapping.BaseReferenceUrl = "http://www.silicofcm.com/fhir";
            mapping.BaseResourceId = "001{PersonID}";
            mapping.PropertyMapping = mappings;
            return mapping;

        }

        private class DummFHIRStore : IFHIRRepository
        {
            private Bundle b = new Bundle()
            {
                Type = Bundle.BundleType.Transaction

            };

            private string GetUrl(Resource r)
            {

                return r.GetType().Name;

            }

            public void StoreResource(string resourceName, object resource,string url)
            {
                if (resource is Resource)
                {
                    var s=(resource as Resource).ToJson();
                    b.AddResourceEntry((resource as Resource), url).Request = new Bundle.RequestComponent() { Method = Bundle.HTTPVerb.POST,Url=GetUrl((resource as Resource)) }; ;
                }


            }

            public void SaveJson(string file)
            {
                File.WriteAllText(file,b.ToJson());
            }
        }

        [TestMethod]
        public void FHIRDataset_Test1()
        {
            var mapping=CreateTestMapping();
            DummFHIRStore store=new DummFHIRStore();
            var file = @".\TestData\data.csv";
            var parser=new FHIRParser(store);

            var dataset= CSVLoader.LoadCSV(file,true,',');
            parser.Parse(mapping,dataset);
            parser.Flush();

            var resource=parser.GetResource("00126030000001001");
            Assert.IsNotNull(resource);
            Assert.IsTrue(resource is ResearchSubject);

            store.SaveJson("test.json");


            mapping.ToJson("mapping.json");

        }


    }
}
