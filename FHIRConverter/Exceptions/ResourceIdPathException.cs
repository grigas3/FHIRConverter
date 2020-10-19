using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverter.Exceptions
{
    public class ResourceIdPathException:Exception
    {
        public string InvalidProp { get; set; }


        public ResourceIdPathException()
        {

        }

        public ResourceIdPathException(string prop)
        {

            this.InvalidProp = prop;
        }

    }
}
