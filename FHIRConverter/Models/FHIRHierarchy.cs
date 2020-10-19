namespace FHIRConverter.Models
{
    /// <summary>
    ///     Model to define an FHIR Hieararchy for the mapping process
    /// </summary>
    public class FHIRHierarchy
    {
        /// <summary>
        ///     Resource Name used in mapping
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        ///     Parent Resource Name used in mapping
        ///     Important. For setting parent resource, only one resource per data row should be declared
        /// </summary>
        public string ParentResourceName { get; set; }

        /// <summary>
        ///     FHIR References
        /// </summary>
        public string FHIRReference { get; set; }
    }
}