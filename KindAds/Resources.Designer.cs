//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KindAds {
    using System;
  using System.Linq;


  /// <summary>
  ///   A strongly-typed resource class, for looking up localized strings, etc.
  /// </summary>
  // This class was auto-generated by the StronglyTypedResourceBuilder
  // class via a tool like ResGen or Visual Studio.
  // To add or remove a member, edit your .ResX file then rerun ResGen
  // with the /str option, or rebuild your VS project.
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KindAds.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

    /// <summary>
    ///   Looks up a localized resource of type System.Byte[].
    /// </summary>
    public static string abi
    {
      get
      {
        object obj = ResourceManager.GetObject("abi", resourceCulture);
        byte[] bytes = (byte[])(obj);
        var str = new string(bytes.Select(Convert.ToChar).ToArray());
        return str;
      }
    }

    /// <summary>
    ///   Looks up a localized string similar to CONGRATULATIONS!.
    /// </summary>
    public static string ACCOUNT_VERIFIED_CONGRATULATIONS {
            get {
                return ResourceManager.GetString("ACCOUNT_VERIFIED_CONGRATULATIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account Verified.
        /// </summary>
        public static string ACCOUNT_VERIFIED_TITLE {
            get {
                return ResourceManager.GetString("ACCOUNT_VERIFIED_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your account has been validated.
        /// </summary>
        public static string ACCOUNT_VERIFIED_VALIDATE {
            get {
                return ResourceManager.GetString("ACCOUNT_VERIFIED_VALIDATE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Agree Terms of Service and Privacy Policy.
        /// </summary>
        public static string AGREE_TERMS_POLICY {
            get {
                return ResourceManager.GetString("AGREE_TERMS_POLICY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create.
        /// </summary>
        public static string CREATE {
            get {
                return ResourceManager.GetString("CREATE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EMAIL.
        /// </summary>
        public static string EMAIL {
            get {
                return ResourceManager.GetString("EMAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ENTER CODE.
        /// </summary>
        public static string ENTER_CODE {
            get {
                return ResourceManager.GetString("ENTER_CODE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please enter your email address..
        /// </summary>
        public static string ENTER_EMAIL {
            get {
                return ResourceManager.GetString("ENTER_EMAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FORGOT YOUR PASSWORD?.
        /// </summary>
        public static string FORGOT_PASSWORD {
            get {
                return ResourceManager.GetString("FORGOT_PASSWORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sign In.
        /// </summary>
        public static string GO_TO_LOGIN {
            get {
                return ResourceManager.GetString("GO_TO_LOGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You will receive a link to create a new password..
        /// </summary>
        public static string LINK_NEW_PASSWORD {
            get {
                return ResourceManager.GetString("LINK_NEW_PASSWORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login.
        /// </summary>
        public static string LOGIN {
            get {
                return ResourceManager.GetString("LOGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NAME.
        /// </summary>
        public static string NAME {
            get {
                return ResourceManager.GetString("NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NEW PASSWORD.
        /// </summary>
        public static string NEW_PASSWORD {
            get {
                return ResourceManager.GetString("NEW_PASSWORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PASSWORD.
        /// </summary>
        public static string PASSWORD {
            get {
                return ResourceManager.GetString("PASSWORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RESEND CODE.
        /// </summary>
        public static string RESEND_CODE {
            get {
                return ResourceManager.GetString("RESEND_CODE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset password.
        /// </summary>
        public static string RESET_PASSWORD {
            get {
                return ResourceManager.GetString("RESET_PASSWORD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save.
        /// </summary>
        public static string SAVE {
            get {
                return ResourceManager.GetString("SAVE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We just send a text message with a code to enter here..
        /// </summary>
        public static string SEND_TEXT_CODE {
            get {
                return ResourceManager.GetString("SEND_TEXT_CODE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SIGN UP.
        /// </summary>
        public static string SIGN_UP {
            get {
                return ResourceManager.GetString("SIGN_UP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -Use 6 - 12 characters.
        /// </summary>
        public static string VAL_CHARACTERS_MAIL {
            get {
                return ResourceManager.GetString("VAL_CHARACTERS_MAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -Use letters with spaces, numbers, or symbols(!@@·$% &amp;*).
        /// </summary>
        public static string VAL_SIMBOLS_MAIL {
            get {
                return ResourceManager.GetString("VAL_SIMBOLS_MAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verify.
        /// </summary>
        public static string VERIFY {
            get {
                return ResourceManager.GetString("VERIFY", resourceCulture);
            }
        }
    }
}
