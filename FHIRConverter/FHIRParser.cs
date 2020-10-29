using System;
using System.Collections.Generic;
using System.Linq;
using FHIRConverter.Exceptions;
using FHIRConverter.Helpers;
using FHIRConverter.Models;
using Hl7.Fhir.Model;

namespace FHIRConverter
{
    public class FHIRParser
    {
        private const string GetDateStr = "GETDATE";

        private readonly IFHIRRepository _repository;

        private readonly Dictionary<string, Tuple<string, object, string>> _resourceDictionary =
            new Dictionary<string, Tuple<string, object, string>>();

        public FHIRParser()
        {
        }

        public FHIRParser(IFHIRRepository rep)
        {
            _repository = rep;
        }

        public static string GetResourceId(string baseUrl, string resourceType, string id)
        {
            return baseUrl + "/" + resourceType + "/" + id;
        }

        /// <summary>
        ///     Get Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object GetResource(string resourceId)
        {
            if (_resourceDictionary.ContainsKey(resourceId))
                return _resourceDictionary[resourceId].Item2;

            return null;
        }


        /// <summary>
        /// Create a unique resource identifier
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetBaseResourceId(string resourcePath, CSVRow row)
        {
            if (resourcePath.Contains("{RowNumber}"))
                resourcePath = resourcePath.Replace("{RowNumber}", row.RowNumber.ToString("D4"));
            foreach (var col in row)
                if (resourcePath.Contains($"{{{col.ColumnName}}}"))
                    resourcePath = resourcePath.Replace($"{{{col.ColumnName}}}", col.Value);

            if (resourcePath.Contains("{{"))
                throw new ResourceIdPathException();

            return resourcePath;
        }

        /// <summary>
        ///     Parse Dataset
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="dataset"></param>
        public void Parse(FHIRMapping prop, CSVDataset dataset)
        {
            foreach (var row in dataset.Rows)
            {
                var rowId = GetBaseResourceId(prop.BaseResourceId, row);
                foreach (var mapping in prop.PropertyMapping) Parse(rowId, prop.BaseReferenceUrl, mapping, row);
            }
        }

        private static string GetUrl(string baseUrl, string resourceType, Resource r)
        {
            return baseUrl + "/" + resourceType.ToLower() + "/" + r.Id;
        }


        /// <summary>
        ///     Parse Row
        /// </summary>
        /// <param name="baseResourceId"></param>
        /// <param name="baseReferenceUrl"></param>
        /// <param name="prop"></param>
        /// <param name="columns"></param>
        public void Parse(string baseResourceId, string baseReferenceUrl, FHIRPropertyMapping prop, CSVRow columns)
        {
            //1. Find Column in the dataset
            CSVColumn column = null;
            if (prop.ColumnIndex.HasValue)
                column = columns.Where(e => e.ColumnIndex.HasValue)
                    .FirstOrDefault(e => e.ColumnIndex.Value == prop.ColumnIndex.Value);
            if (column == null && !string.IsNullOrEmpty(prop.ColumnName))
                column = columns.FirstOrDefault(e => e.ColumnName == prop.ColumnName);

            if (column == null)
                throw new ArgumentNullException();

            object root = null;


            if (string.IsNullOrEmpty(prop.ResourceBaseType)) throw new ResourceBaseTypeNotDefined();


            //Get Base Resource Identifier based on base ResourceId and property Resource Id
            var resourceId = GetBaseResourceId(baseResourceId + prop.ResourceId, columns);

            //if the resource already exists then the property is assigned to the existing resource
            if (_resourceDictionary.ContainsKey(resourceId))
            {
                root = _resourceDictionary[resourceId].Item2;

                FHIRTranslator.SetProperty(root, prop.ResourcePath, column.Value, prop.ResourcePathTypes,
                    prop.ValueTemplate);
            }
            else
            {
                //Create a new resource
                root = FHIRTranslator.SetProperty(prop.ResourcePath, prop.ResourcePathTypes, column.Value,
                    prop.ValueTemplate);

                //Set Resource Id
                if (root != null && root is Resource && string.IsNullOrEmpty((root as Resource).Id))
                    (root as Resource).Id = resourceId;

                // Add to resource dictionary with the corresponding resource URL for reference
                _resourceDictionary.Add(resourceId,
                    Tuple.Create(prop.ResourceBaseType,
                        root,
                        GetUrl(baseReferenceUrl, 
                            prop.ResourceBaseType, 
                            root as Resource)));
            }

            //Create Fix properties on the Resource
            if (root != null && prop.FixedProperties != null)
                CreateFixProperties(root, prop.FixedProperties, columns);
        }

        private static object ConvertValue(string value, List<CSVColumn> columns)
        {
            if (value == GetDateStr)
                return new DateTimeOffset();

            return value;
        }

        public static void CreateFixProperties(object root, IEnumerable<FHIRProperty> fixProps,
            List<CSVColumn> columns = null)
        {
            if (root == null)
                throw new ArgumentNullException();

            if (fixProps == null)
                throw new ArgumentNullException();
            //Add to object (FHIR Resource) all fixed properties
            foreach (var c in fixProps)
                FHIRTranslator.SetProperty(root, c.ResourcePath, ConvertValue(c.Value, columns), c.ResourcePathTypes);
        }

        /// <summary>
        ///     After Parse flush and save data
        /// </summary>
        public void Flush()
        {
            if (_repository == null)
                return;
            var values = _resourceDictionary.Values;

            foreach (var v in values) _repository.StoreResource(v.Item1, v.Item2, v.Item3);
        }
    }
}