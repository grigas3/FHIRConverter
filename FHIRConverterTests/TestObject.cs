using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverterTests
{
    class TestObject
    {
        public DateTime? NullableDateTimeProp { get; set; }
        public DateTime DateTimeProp { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetProp { get; set; }
        public DateTimeOffset DateTimeOffsetProp { get; set; }


        public double? NullableDoubleProp { get; set; }
        public double DoubleProp { get; set; }

        public decimal? NullableDecimalProp { get; set; }
        public decimal DecimalProp { get; set; }

        public string StringProp { get; set; }

    }
}
