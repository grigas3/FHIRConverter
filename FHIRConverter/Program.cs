using System;
using FHIRConverter.Loaders;
using FHIRConverter.Models;
using FHIRConverter.Storage;

namespace FHIRConverter
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            if (args.Length != 3)
            {
                Console.WriteLine("============================");
                Console.WriteLine("FHIR Converter Tool usage");
                Console.WriteLine("FHIRConverter.exe [CSV Flat File] [Mapping json] [Output Json]");
                
            }

            var mapping=FHIRMapping.FromJson(args[1]);
            var store= new SimpleFHIRStore();
            var parser = new FHIRParser(store);
            var dataset = CSVLoader.LoadCSV(args[0], true, ',');
            parser.Parse(mapping, dataset);
            parser.Flush();
            store.SaveJson(args[1]);



        }
    }
}