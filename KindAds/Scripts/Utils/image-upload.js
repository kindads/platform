function validateImgFile(minw,minh,idComponent, idComponentLabel) {
  if ($(idComponent).val().length > 0) {
    var ext = $(idComponent).val().split('.').pop().toLowerCase();
    if ($.inArray(ext, ['png', 'jpg', 'jpeg']) == -1) {
      $(idComponentLabel).show().text('Invalid extension');
      alert("Invalid image extension");
      return false;
    }
    else {
      var _URL = window.URL || window.webkitURL;

      var file = $(idComponent)[0].files[0];

      var newImg = $(idComponent + "img");
      var nImg = newImg;
      
      nImg.removeAttr("width");
      nImg.removeAttr("height");
      var h = nImg.height();
      var w = nImg.width();

      if (h <= minh || w <= minw) {
        alert("Image is smaller than " + minw + " x " + minh);
        return false;
      }
   
      return true;
    }
  } else {
    var imgTag = $(idComponent + "img");
    
    if (imgTag) {

      if (imgTag.height() > 0 && imgTag.width() > 0 && imgTag.attr('src') != "" &&  imgTag.attr('src') != "<NULL>") {

        console.log("img exist");
        return true;
      }
      console.log("img dont exist");
    }
   
      $(idComponentLabel).show().text('Image required');
      alert("Image required");
      return false;
   
  }
}
