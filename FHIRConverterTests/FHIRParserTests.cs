using Microsoft.VisualStudio.TestTools.UnitTesting;
using FHIRConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHIRConverter.Models;
using FHIRConverter.Helpers;
using Hl7.Fhir.Model;

namespace FHIRConverter.Tests
{
    [TestClass()]
    public class FHIRParserTests
    {
        [TestMethod()]
        public void CreateFixPropertiesTest()
        {

            List<FHIRProperty> fixedProperties = new List<FHIRProperty>();
            var o = FHIRTranslator.CreateInstance("Hl7.Fhir.Model.Observation");
            fixedProperties.Add(new FHIRProperty()
            {

                ResourcePath = "Code/Text",
                Value="Test",
                ResourceType= "CodeableConcept/String"
            });
            FHIRParser.CreateFixProperties(o, fixedProperties);

            Assert.AreEqual((o as Observation).Code.Text, "Test");


        }

        [TestMethod()]
        public void CreateParserJson()
        {

            FHIRMapping parser =new FHIRMapping();
            parser.Hierarchy = new List<FHIRHierarchy>()
            {
                new FHIRHierarchy()
                {
                    ResourceName="Patient"

                },
                new FHIRHierarchy()
                {
                    ResourceName = "Observation",
                    ParentResourceName = "Patient"

                }
            };
            parser.PropertyMapping = new List<FHIRPropertyMapping>()
            {
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 1,
                    ColumnName = "Age",
                    ResourceId = "001001",
                    ResourcePath = "Observation/Value/Value",
                    ResourceBaseType = "Observation",
                    ResourceTypes = "Observation/SimpleQuantity/Decimal",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Code/Text",
                            ResourceType = "Observation/CodeableConcept/String",
                            Value = "Age"
                        },
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Issued",
                            ResourceType = "Observation/DateTimeOffset",
                            Value = "GETDATE"
                        }
                    }
                },
                new FHIRPropertyMapping()
                {
                    ColumnType = "string",
                    ColumnIndex = 2,
                    ResourceId = "001002",
                    ColumnName = "BMI",
                    ResourceBaseType = "Observation",
                    ResourcePath = "Observation/Value/Value",
                    ResourceTypes = "Observation/SimpleQuantity/Decimal",
                    FixedProperties = new List<FHIRProperty>()
                    {
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Code/Text",
                            ResourceType = "Observation/CodeableConcept/String",
                            Value = "BMI"
                        },
                        new FHIRProperty()
                        {
                            ResourcePath = "Observation/Issued",
                            ResourceType = "Observation/DateTimeOffset",
                            Value = "GETDATE"
                        }
                    }
                }
            };

            parser.ToJson("parser.json");



        }

        [TestMethod]
        public void Parse_Test1()
        {

            FHIRParser parser = new FHIRParser();
            var propMapping1 = new FHIRPropertyMapping()
            {

                ColumnIndex = 1,
                ResourceId = "001001",
                ResourceBaseType = "Observation",
                ResourcePath = "Observation/Value/Value",
                ResourceTypes = "Observation/SimpleQuantity/Decimal",
                FixedProperties = new List<FHIRProperty>()
                {
                    new FHIRProperty()
                    {
                          ResourcePath = "Observation/Code/Text",
                          ResourceType = "Observation/CodeableConcept/String",
                          Value="Code1"
                    },
                      new FHIRProperty()
                    {
                          ResourcePath = "Observation/Issued",
                          ResourceType = "Observation/DateTimeOffset",
                          Value="GETDATE"
                    }
                }
            };



            parser.Parse(null,"001",propMapping1, new CSVRow(){new CSVColumn(){

                ColumnIndex=1,Value="1"
                }});

            var res=parser.GetResource("001001001");

            Assert.IsNotNull(res);

            Assert.AreEqual(((res as Observation).Value as SimpleQuantity).Value, 1);
        }
    }
}