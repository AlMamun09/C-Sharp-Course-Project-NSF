using System.Collections.Generic;

namespace LocalScout.ViewModels
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<ServiceCardViewModel> Results { get; set; } = new List<ServiceCardViewModel>();
    }
}