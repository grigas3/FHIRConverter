using System.Collections.Generic;

namespace FHIRConverter.Models
{
    /// <summary>
    ///     CSV Row
    /// </summary>
    public class CSVRow : List<CSVColumn>
    {
        public int RowNumber { get; set; }
    }
}