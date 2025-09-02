using LocalScout.Data;
using LocalScout.Models;
using System.Collections.Generic;

namespace LocalScout.ViewModels
{
    public class ServiceDetailsViewModel
    {
        // The main service the user is viewing
        public ProviderService MainService { get; set; } = new ProviderService();

        // The provider who offers this service
        public ApplicationUser Provider { get; set; } = new ApplicationUser();

        // The list of other services from the same provider
        public List<ProviderService> OtherServices { get; set; } = new List<ProviderService>();
    }
}