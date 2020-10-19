using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverter.Tests
{
    public  class DummyResourceCreator
    {

        public static Observation CreateDummyObservation()
        {
            var c = new CodeableConcept() { Text = "Test" };
            

            return new Observation()
            {
                Value = new SimpleQuantity() { Value=12,Code="Test"} ,
                Code=new CodeableConcept() {  Text="Test"},
                Issued= DateTimeOffset.FromUnixTimeSeconds(10)
               
            };


        }


            public static ResearchSubject CreateObject()
        {
            ResearchSubject subject = new ResearchSubject();
            subject.Id = "ST01";
            var consent = new Consent()
            {

            };
            
            
            subject.Period=new Period() { Start = "2018/01/02" };
            return subject;

        }

    }
}
