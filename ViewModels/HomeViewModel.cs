using NeighborhoodServiceFinder.Models;
using System.Collections.Generic;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class HomeViewModel
    {
        public List<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
        public List<ServiceCardViewModel> FeaturedServices { get; set; } = new List<ServiceCardViewModel>();
    }
}