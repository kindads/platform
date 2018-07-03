using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace captivate_express_webapp.Models.Access
{
  [Serializable]
  public class CreateAccountViewModel
  {
    [Required(ErrorMessage = "Name is required")]
    [MinLength(4, ErrorMessage = "Minimum length is {1} characters")]
    [MaxLength(24, ErrorMessage = "Maximum length is {1} characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [MaxLength(255, ErrorMessage = "Maximum length is {1} characters")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Minimum length is {1} characters")]
    [MaxLength(12, ErrorMessage = "Maximum length is {1} characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage ="You must accept terms and policy")]
    public bool AgreeTerms { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { set; get; }

    public SelectList ListRole { set; get; }
  }

  public class LoginViewModel
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [MaxLength(255, ErrorMessage = "Maximum length is {1} characters")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Minimum length is {1} characters")]
    [MaxLength(12, ErrorMessage = "Maximum length is {1} characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "TestLanguage", ResourceType = typeof(Resource.Language.Access.Login.Resources))]
    public string TestLanguage { get; set; }
  }

  public class NewPasswordViewModel
  {

    public string Email { get; set; }
    public string Code { get; set; }
    [Required(ErrorMessage = "*")]
    [MinLength(6, ErrorMessage = "Minimum length is {1} characters")]
    [MaxLength(12, ErrorMessage = "Maximum length is {1} characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

  }

  public class RecoverPasswordViewModel
  {
    [Required(ErrorMessage = "*")]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [MaxLength(255, ErrorMessage = "Maximum length is {1} characters")]
    public string Email { get; set; }
  }

  public class VerifyAccountViewModel
  {
    [Required(ErrorMessage = "*")]
    [MaxLength(255, ErrorMessage = "Maximum length is {1} characters")]
    public string Code { get; set; }
  }

}
