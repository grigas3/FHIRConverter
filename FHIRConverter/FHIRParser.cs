using FHIRConverter.Helpers;
using FHIRConverter.Models;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHIRConverter.Exceptions;
using Resource = Hl7.Fhir.Model.Resource;

namespace FHIRConverter
{
    public class FHIRParser
    {

        private readonly IFHIRRepository _repository;
        public FHIRParser()
        {


        }

        public FHIRParser(IFHIRRepository rep)
        {
            _repository = rep;
        }

        private const string GetDateStr = "GETDATE";

        private readonly Dictionary<string, Tuple<string,object,string>> _resourceDictionary = new Dictionary<string, Tuple<string, object,string>>();

        public static string GetResourceId(string baseUrl, string resourceType, string id)
        {
            return baseUrl + "/" + resourceType + "/" + id;
        }

        /// <summary>
        /// Get Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object GetResource(string resourceId)
        {
            if (_resourceDictionary.ContainsKey(resourceId))
                return _resourceDictionary[resourceId].Item2;

            return null;
        }

        private string GetBaseResourceId(string resourcePath,CSVRow row)
        {

            if (resourcePath.Contains($"{{RowNumber}}"))
                resourcePath = resourcePath.Replace($"{{RowNumber}}", row.RowNumber.ToString("D4"));
            foreach (var col in row)
            {
                if (resourcePath.Contains($"{{{col.ColumnName}}}"))
                {
                    resourcePath= resourcePath.Replace($"{{{col.ColumnName}}}", col.Value);
                }
            }

            if(resourcePath.Contains("{{"))
                throw new ResourceIdPathException();

            return resourcePath;

        }

        /// <summary>
        /// Parse Dataset
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="dataset"></param>
        public void Parse(FHIRMapping prop, CSVDataset dataset)
        {

            foreach (var row in dataset.Rows)
            {

                var rowId = GetBaseResourceId(prop.BaseResourceId, row);
                foreach (var mapping in prop.PropertyMapping)
                {
                    Parse(rowId,prop.BaseReferenceUrl, mapping, row);
                }
            }

            
        }

        private string GetUrl(string baseUrl,string resourceType,Resource r)
        {


            return baseUrl + "/" + resourceType.ToLower() + "/" + r.Id;


        }


        /// <summary>
        /// Parse Row
        /// </summary>
        /// <param name="baseResourceId"></param>
        /// <param name="prop"></param>
        /// <param name="columns"></param>
        public void Parse(string baseResourceId,string baseReferenceUrl,FHIRPropertyMapping prop, CSVRow columns)
        {

            //1. Find Column in the dataset
            CSVColumn column = null;
            if (prop.ColumnIndex.HasValue)
                column = columns.Where(e => e.ColumnIndex.HasValue)
                    .FirstOrDefault(e => e.ColumnIndex.Value == prop.ColumnIndex.Value);
            if (column == null && !string.IsNullOrEmpty(prop.ColumnName))
            {
                column = columns.FirstOrDefault(e => e.ColumnName == prop.ColumnName);
            }

            if (column == null)
                throw new ArgumentNullException();

            object root = null;


            if (string.IsNullOrEmpty(prop.ResourceBaseType))
            {
                throw new ResourceBaseTypeNotDefined();

            }

            var resourceId =GetBaseResourceId( baseResourceId + prop.ResourceId, columns);
            if (_resourceDictionary.ContainsKey(resourceId))
            {
                root = _resourceDictionary[resourceId].Item2;
                
                FHIRTranslator.SetProperty(root, prop.ResourcePath, column.Value, prop.ResourceTypes,prop.ValueTemplate);
            }
            else
            {
                root = FHIRTranslator.SetProperty(prop.ResourcePath, prop.ResourceTypes, column.Value, prop.ValueTemplate);

                if (root!=null&&root is Resource &&string.IsNullOrEmpty((root as Resource).Id))
                    (root as Resource).Id = resourceId;
                _resourceDictionary.Add(resourceId, Tuple.Create<string,object,string>(prop.ResourceBaseType,root,GetUrl(baseReferenceUrl,prop.ResourceBaseType,root as Resource)));
            }


            if (root != null && prop.FixedProperties != null)
                FHIRParser.CreateFixProperties(root, prop.FixedProperties,columns);
        }

        private static object ConvertValue(string value, List<CSVColumn> columns)
        {
            
                if ((value as string) == GetDateStr)
                    return new DateTimeOffset();

                return value;
        }

        public static void CreateFixProperties(object root, IEnumerable<FHIRProperty> fixProps, List<CSVColumn> columns=null)
        {
            if (root == null)
                throw new ArgumentNullException();

            if (fixProps == null)
                throw new ArgumentNullException();
            //Add to object (FHIR Resource) all fixed properties


            foreach (var c in fixProps)
            {
                FHIRTranslator.SetProperty(root, c.ResourcePath, ConvertValue(c.Value, columns), c.ResourceType);
            }
        }

        /// <summary>
        /// After Parse save data
        /// </summary>
        public void Flush()
        {
            if (_repository == null)
                return;
            var values=_resourceDictionary.Values;

            foreach (var v in values)
            {
                
                _repository.StoreResource(v.Item1,v.Item2,v.Item3);

            }



        }




    }
}