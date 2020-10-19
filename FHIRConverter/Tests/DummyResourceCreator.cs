using System;
using Hl7.Fhir.Model;

namespace FHIRConverter.Tests
{
    public class DummyResourceCreator
    {
        public static Observation CreateDummyObservation()
        {
            var c = new CodeableConcept {Text = "Test"};


            return new Observation
            {
                Value = new SimpleQuantity {Value = 12, Code = "Test"},
                Code = new CodeableConcept {Text = "Test"},
                Issued = DateTimeOffset.FromUnixTimeSeconds(10)
            };
        }


        public static ResearchSubject CreateObject()
        {
            var subject = new ResearchSubject();
            subject.Id = "ST01";
            var consent = new Consent();


            subject.Period = new Period {Start = "2018/01/02"};
            return subject;
        }
    }
}