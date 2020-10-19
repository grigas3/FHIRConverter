using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FHIRConverterTests
{

    [TestClass]
    public class CastingTests
    {

        public void PeriodCast_Test()
        {


            Period p=new Period();

            p.Start = "01-01-01";



        }
        public void ObsReferCast_Test()
        {
            double c = 1;

            Observation obs=new Observation();

           obs.Subject=new ResourceReference();

           obs.Value =new Quantity();

           ResearchSubject s=new ResearchSubject();
           

           //   (obs.Value as SimpleQuantity).Value = c;

        }


    }
}
