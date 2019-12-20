using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Paho.Models
{
    // Models returned by AccountController actions.
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Procedencia")]
        public string Hometown { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        //[Display(Name = "Contraseña actualxxx")]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Current password")]
        //public string OldPasswordENG { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [StringLength(100, ErrorMessage = "En la contraseña {0} debe haber al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        //[Display(Name = "Nueva contraseña")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirmar nueva contraseña")]
        [Display(Name = "Confirm new password")]
        //[Compare("NewPassword", ErrorMessage = "La nueva contraseña y la casilla de confirmación no coinciden.")]
        [Compare("NewPassword", ErrorMessage = "The new password and the confirmation box do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]       
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [StringLength(100, ErrorMessage = "En la contraseña {0} debe haber al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de contraseña")]
        [Compare("Password", ErrorMessage = "La nueva contraseña y la casilla de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Procedencia")]
        public string Hometown { get; set; }
        [Display(Name = "Primer nombre")]
        public string FirstName1 { get; set; }
        [Display(Name = "Segundo nombre")]
        public string FirstName2 { get; set; }
        [Display(Name = "Apellido")]
        public string LastName1 { get; set; }
        [Display(Name = "Segundo apellido")]
        public string LastName2 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Institución")]
        public long InstitutionID { get; set; }
        public int? type_region { get; set; }
        public string InstitutionType { get; set; }
        [Display(Name = "Institución Externa:")]            //#### CAFQ: 180911
        public bool ForeignLab { get; set; }                //#### CAFQ: 180911
        public IEnumerable<System.Web.Mvc.SelectListItem> Institutions { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> RolesList { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Este campo es requerido")]
        [StringLength(100, ErrorMessage = "En la contraseña {0} debe haber al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de contraseña")]
        [Compare("Password", ErrorMessage = "La nueva contraseña y la casilla de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; } 
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}