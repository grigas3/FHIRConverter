namespace FHIRConverter
{
    /// <summary>
    ///     FHIR Repository interface
    ///     Any FHIR store should implement this method in order the parsed data
    ///     to be stored properly
    /// </summary>
    public interface IFHIRRepository
    {
        /// <summary>
        ///     Store FHIR Resource
        /// </summary>
        /// <param name="resourceName">Resource Name</param>
        /// <param name="resource">Resource object</param>
        void StoreResource(string resourceName, object resource, string url);
    }
}