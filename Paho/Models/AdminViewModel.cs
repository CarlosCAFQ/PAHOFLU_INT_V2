using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Paho.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role ")]
        public string Name { get; set; }

        [Display(Name = "Primer nombre")]
        public string FirstName1 { get; set; }
        [Display(Name = "Segundo nombre")]
        public string FirstName2 { get; set; }
        [Display(Name = "Primer apellido")]
        public string LastName1 { get; set; }
        [Display(Name = "Segundo apellido")]
        public string LastName2 { get; set; }
        [Required]
        [Display(Name = "Institución")]
        public long InstitutionID { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Institutions { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        //**** CAFQ: 190802
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [StringLength(100, ErrorMessage = "En la contraseña {0} debe haber al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de contraseña")]
        [Compare("Password", ErrorMessage = "La nueva contraseña y la casilla de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Cambiar password:")]
        public bool ChangePassword { get; set; }
        //**** CAFQ: 190802 END

        [Display(Name = "Hometown")]
        [Required(AllowEmptyStrings=false,ErrorMessage="Este campo es requerido")]
        public string Hometown { get; set; }
        [Display(Name = "Primer nombre")]
        public string FirstName1 { get; set; }
        [Display(Name = "Segundo nombre")]
        public string FirstName2 { get; set; }
        [Display(Name = "Primer apellido")]
        public string LastName1 { get; set; }
        [Display(Name = "Segundo apellido")]
        public string LastName2 { get; set; }
       [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Institución")]
        public long InstitutionID { get; set; }
        [Display(Name = "Institución Externa:")]            //#### CAFQ: 180911
        public bool ForeignLab { get; set; }                //#### CAFQ: 180911
        public string InstitutionType { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Institutions { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> RolesList { get; set; }
    }
}