var _validFileExtensions = [".pdf", ".xlxs", ".xlx"];
var _allowedFormat = ["PDF","Excel"]
function ValidateFile(oForm) {
    var arrInputs = oForm.getElementsByTagName("input");
    for (var i = 0; i < arrInputs.length; i++) {
        var oInput = arrInputs[i];
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensions.length; j++) {
                    var sCurExtension = _validFileExtensions[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }

                if (!blnValid) {
                    swal("Sorry, " + sFileName + " is invalid, allowed files are: " + _allowedFormat.join(", "));
                    return false;
                }
            }
        }
    }

    return true;
}