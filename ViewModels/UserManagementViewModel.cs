using NeighborhoodServiceFinder.Data;
using System.Collections.Generic;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class UserManagementViewModel
    {
        public List<ApplicationUser> RegularUsers { get; set; }
        public List<ApplicationUser> ServiceProviders { get; set; }

        public UserManagementViewModel()
        {
            RegularUsers = new List<ApplicationUser>();
            ServiceProviders = new List<ApplicationUser>();
        }
    }
}