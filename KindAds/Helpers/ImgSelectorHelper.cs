using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Helpers {
    public static class ImgSelectorHelper {
        public static MvcHtmlString DisplayImageSelector(string idInput,  string imgPath)
        {
            StringBuilder sb = new StringBuilder();
            string imageName = string.IsNullOrEmpty(imgPath) ? "Image is required" : imgPath.Split('/').ElementAt(imgPath.Split('/').Count()-1);

            sb.Append($@"
                      <label id='{idInput}lbl'>{imageName}</label>
                      <img id='{idInput}img' src='{imgPath}' />
                      ");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString IncludeJSDisplayImageSelector(string idInput, string defaultImgPath="")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($@"$('#{idInput}').change(function() {{
                var file = $('#{idInput}')[0].files[0];
                if (file) {{
                    $('#{idInput}lbl').text(file.name);
                    var input = this;
                    var url = $(this).val();
                    var ext = url.substring(url.lastIndexOf('.') + 1).toLowerCase();
                    if (input.files && input.files[0] && (ext == 'gif' || ext == 'png' || ext == 'jpeg' || ext == 'jpg')) {{
                        var reader = new FileReader();
                        reader.onload = function(e) {{
                            $('#{idInput}img').attr('src', e.target.result);
                        }}
                        reader.readAsDataURL(input.files[0]);
                    }}
                    else {{
                        $('#{idInput}img').attr('src', '{defaultImgPath}');
                    }}
                }}
            }});");
            return new MvcHtmlString(sb.ToString());
        }
    }
}
