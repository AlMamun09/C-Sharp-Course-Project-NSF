using System.Collections.Generic;

namespace LocalScout.Models
{
    public class PaginatedServiceResult
    {
        public List<ProviderService> Services { get; set; } = new List<ProviderService>();
        public string? LastDocumentId { get; set; }
        public bool HasMorePages { get; set; }
    }
}