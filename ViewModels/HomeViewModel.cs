using LocalScout.Models;
using System.Collections.Generic;

namespace LocalScout.ViewModels
{
    public class HomeViewModel
    {
        public List<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
        public List<ServiceCardViewModel> FeaturedServices { get; set; } = new List<ServiceCardViewModel>();
    }
}