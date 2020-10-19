using Microsoft.VisualStudio.TestTools.UnitTesting;
using FHIRConverter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using FHIRConverter.Models;
using FHIRConverterTests;

namespace FHIRConverter.Helpers.Tests
{
    [TestClass()]
    public class FHIRTranslatorTests
    {

        public static string GetTypeName<T>()
        {

            return typeof(T).FullName;

        }
        [TestMethod()]
        public void CreateInstance_CreateResearchSubject_Test()
        {
            var name=GetTypeName<ResearchSubject>();
            Assert.AreEqual(name, "Hl7.Fhir.Model.ResearchSubject");
            var o=FHIRTranslator.CreateInstance(name);

            Assert.IsNotNull(o);

        }

        [TestMethod()]
        public void SetProperty_ResearchSubject_ID_Test()
        {
            var o = FHIRTranslator.CreateInstance("Hl7.Fhir.Model.ResearchSubject");
            FHIRTranslator.SetProperty(o,"SUB001", "Id", "SUB001");
            
            Assert.IsTrue(o is ResearchSubject);

            Assert.AreEqual((o as ResearchSubject).Id, "SUB001");
                


        }

        [TestMethod()]
        public void SetProperty_ResearchSubject_Period_Start_Test()
        {

            string value = "2018/01/01";
            var o = FHIRTranslator.CreateInstance("Hl7.Fhir.Model.ResearchSubject");
            FHIRTranslator.SetProperty(o,"XX", "Period/Start",value );

            Assert.IsTrue(o is ResearchSubject);

            Assert.AreEqual((o as ResearchSubject).Period.Start, value);



        }

        [TestMethod()]
        public void SetProperty_Fixed_Value_Test()
        {



        }

        [TestMethod()]
        public void SetProperty_Observation_Value_Test()
        {

            int value = 15;
            var o = FHIRTranslator.CreateInstance("Hl7.Fhir.Model.Observation");
         
            FHIRPropertyMapping mapping = new FHIRPropertyMapping()
            {
                ResourcePath = "Value/Value",
                ResourceTypes = "SimpleQuantity/Decimal"
            };
            SetProperty(o, mapping, value);
            Assert.IsTrue(o is Observation);
            Assert.IsTrue((o as Observation).Value is SimpleQuantity);
            Assert.AreEqual(((o as Observation).Value as SimpleQuantity).Value, value);


        }


        [TestMethod()]
        public void SetProperty_Observation_Value_Test2()
        {

            int value = 15;
            var o = FHIRTranslator.CreateInstance("Hl7.Fhir.Model.Observation");

            FHIRPropertyMapping mapping = new FHIRPropertyMapping()
            {
                ResourcePath = "Observation/Value/Value",
                ResourceTypes = "Observation/SimpleQuantity/Decimal"
            };
            SetProperty(o, mapping, value);
            Assert.IsTrue(o is Observation);
            Assert.IsTrue((o as Observation).Value is SimpleQuantity);
            Assert.AreEqual(((o as Observation).Value as SimpleQuantity).Value, value);


        }

        [TestMethod()]
        public void SetProperty_Observation_Value_Test3()
        {

            int value = 15;
       
            FHIRPropertyMapping mapping = new FHIRPropertyMapping()
            {
                ResourcePath = "Observation/Value/Value",
                ResourceTypes = "Observation/SimpleQuantity/Decimal"
            };
            var o=SetProperty( mapping, value);
            Assert.IsTrue(o is Observation);
            Assert.IsTrue((o as Observation).Value is SimpleQuantity);
            Assert.AreEqual(((o as Observation).Value as SimpleQuantity).Value, value);


        }



        [TestMethod()]
        public void SetProperty_Period_Start_Test()
        {

            var  value = "2020-01-01";

            FHIRPropertyMapping mapping = new FHIRPropertyMapping()
            {
                ResourcePath = "ResearchSubject/Period/Start",
                ResourceTypes = "ResearchSubject/Period/String"
            };
            var o = SetProperty(mapping, value);
            Assert.IsTrue(o is ResearchSubject);
            Assert.IsFalse((o as ResearchSubject).Period is null);
            Assert.AreEqual(((o as ResearchSubject).Period).Start, value);


        }


        private void SetProperty(object root,FHIRPropertyMapping mapping,object value)
        {

            FHIRTranslator.SetProperty(root, mapping.ResourcePath, value,mapping.ResourceTypes);

        }


        private object SetProperty( FHIRPropertyMapping mapping, object value)
        {

            return FHIRTranslator.SetProperty(mapping.ResourcePath,  mapping.ResourceTypes,value);

        }

        [TestMethod]
        public void AssignValue_Double_Test()
        {
            TestObject obj = new TestObject();

            var p = obj.GetType().GetProperty("DoubleProp");

            FHIRTranslator.AssignValue(p, obj, 15, "Double");

            Assert.AreEqual(obj.DoubleProp, 15);
            
        }


        [TestMethod]
        public void AssignValue_DateTimeOffset_Test()
        {
            TestObject obj = new TestObject();

            var p = obj.GetType().GetProperty("DateTimeOffsetProp");

            FHIRTranslator.AssignValue(p, obj, (long)15, "DateTimeOffset");

            Assert.AreEqual(obj.DateTimeOffsetProp.Second, 15);

        }

    }
}