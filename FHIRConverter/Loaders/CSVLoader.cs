using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FHIRConverter.Models;

namespace FHIRConverter.Loaders
{
    /// <summary>
    ///     CSV Loader
    /// </summary>
    public class CSVLoader
    {
        /// <summary>
        ///     Load A csv file
        /// </summary>
        /// <param name="file">Input file</param>
        /// <param name="hasHeader">File has a header</param>
        /// <param name="sep">Separator character</param>
        /// <returns></returns>
        public static CSVDataset LoadCSV(string file, bool hasHeader = true, char sep = ';')
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);


            var lines = File.ReadAllLines(file);
            if (lines.Length == 0)
                return null;
            var headers = new Dictionary<int, string>();
            int numOfHeaders;

            if (hasHeader)
            {
                var headerLine = lines[0].Split(sep).ToArray();
                numOfHeaders = headerLine.Length;
                for (var i = 0; i < headerLine.Length; i++) headers.Add(i, headerLine[i]);
            }
            else
            {
                var headerLine = lines[0].Split(sep).ToArray();
                numOfHeaders = headerLine.Length;
                for (var i = 0; i < headerLine.Length; i++) headers.Add(i, "COLUMN" + (i + 1));
            }


            var dataset = new CSVDataset();
            dataset.FileName = file;
            dataset.Date = DateTime.Now;
            dataset.Rows = new List<CSVRow>();
            var firstLine = true;
            var rowNumber = 1;
            foreach (var line in lines)
            {
                if (hasHeader && firstLine)
                {
                    firstLine = false;
                    continue;
                }

                var row = new CSVRow();
                row.RowNumber = rowNumber;
                var valueArrays = line.Split(sep).ToArray();

                if (valueArrays.Length != numOfHeaders)
                    continue;

                for (var i = 0; i < valueArrays.Length; i++)
                    row.Add(new CSVColumn {Value = valueArrays[i], ColumnIndex = i, ColumnName = headers[i]});

                dataset.Rows.Add(row);
                rowNumber++;
            }

            return dataset;
        }
    }
}