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

    /// <summary>
    ///     CSV Row
    /// </summary>
    public class CSVRow : List<CSVColumn>
    {
        public int RowNumber { get; set; }
    }

    /// <summary>
    ///     Column of CSV File
    /// </summary>
    public class CSVColumn
    {
        /// <summary>
        ///     Column Name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     Column Index
        /// </summary>
        public int? ColumnIndex { get; set; }

        /// <summary>
        ///     Column Value
        /// </summary>
        public string Value { get; set; }
    }
}