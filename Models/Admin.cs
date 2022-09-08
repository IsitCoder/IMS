using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class Admin:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ?ProfilePicture { get; set; }
    }
}
