namespace FHIRConverter.Models
{
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