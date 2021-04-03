namespace PartnerCenter.Console.Models
{
    public class CollectionResult<T>
    {
        public T[]? Value { get; set; }
        public long TotalCount { get; set; }
    }
}