using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverter.Models
{
    /// <summary>
    /// Property Mapping
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class FHIRPropertyMapping
    {
        /// <summary>
        /// Resource Identifier
        /// </summary>
        public string ResourceId { get; set; }
        /// <summary>
        /// Column Name (when the input file has a header row)
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Column Index (No Headers)
        /// </summary>
        public int? ColumnIndex { get; set; }
        /// <summary>
        /// Column Type to convert input string value
        /// Supported Datetime, double, integer
        /// Default string
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// Resource Types
        /// </summary>
        public string ResourceTypes { get; set; }

        /// <summary>
        /// Resource Path
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Fixed properties related to the specific entry
        /// </summary>
        public List<FHIRProperty> FixedProperties { get; set; }

        /// <summary>
        /// Resource Base Type
        /// </summary>
        public string ResourceBaseType { get; set; }

        /// <summary>
        /// Value Template
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}
