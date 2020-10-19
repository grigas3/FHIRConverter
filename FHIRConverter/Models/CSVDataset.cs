using System;
using System.Collections.Generic;

namespace FHIRConverter.Models
{
    /// <summary>
    ///     CSV Dataset
    /// </summary>
    public class CSVDataset
    {
        /// <summary>
        ///     File Name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Rows
        /// </summary>
        public List<CSVRow> Rows { get; set; }
    }
}