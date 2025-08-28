using System.Collections.Generic;

namespace LocalScout.ViewModels
{
    public class CategoryServicesViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<ServiceCardViewModel> Services { get; set; } = new List<ServiceCardViewModel>();
    }
}