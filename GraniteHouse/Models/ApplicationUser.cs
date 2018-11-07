using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Lockout Reason")]
        public string LockoutReason { get; set; }

        [Display(Name = "UnLock Reason")]
        public string UnLockReason { get; set; }

        [NotMapped]
        public bool IsLockedOut { get; set; }

        [NotMapped]
        public string RolesNames { get; set; }

    }
}
