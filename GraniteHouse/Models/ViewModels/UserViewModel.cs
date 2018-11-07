using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser SelectedUser { get; set; }

        public List<RolesListOfSelectedUser> RolesList { get; set; }

    }
}
