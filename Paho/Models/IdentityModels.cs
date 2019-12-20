using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Paho.Models
{

    //public class ApplicationRole : IdentityRole
    //{
    //    //public string Name { get; set; }

    //    public ApplicationRole() : base() { }
    //    public ApplicationRole(string name)
    //        : this()
    //    {
    //        this.Name = name;
    //    }

    //    //public ApplicationRole(string name, string description)
    //    //    : this(name)
    //    //{
    //    //    this.Description = description;
    //    //}


    //}
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string Hometown { get; set; }
        public long? InstitutionID { get; set; }
        public int? type_region { get; set; }
        public string FirstName1 { get; set; }
        public string FirstName2 { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        [Display(Name="Nombre")]
        public string FullName
        {
            get {
                return string.Concat(FirstName1, string.IsNullOrEmpty(FirstName2) ? "" : " ", FirstName2, " ", LastName1, string.IsNullOrEmpty(LastName2) ? "" : " ", LastName2);
            }
        }

        public string UserRolesListJoin;            //#### CAFQ: 180530
        public bool ForeignLab { get; set; }        //#### CAFQ: 180911

        [Display(Name = "Institución")]
        public virtual Institution Institution { get; set; }
        //public virtual Country Country { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }  
}