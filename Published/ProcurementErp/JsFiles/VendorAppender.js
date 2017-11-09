$(function () {
    $('#VENDORS').change(function () {

        var vendorName;
        var vendorEmail;
        var className = $(this).attr('id');
        var val = $(this).val()
        var branch = $('#vendorslist option').filter(function () {
            return this.value == val;
        }).data('vendor');
        var msg = branch ? branch : 'No Match';
        var email = val.split("-----");
        
        if (msg == "No Match") {
            //alert(true);
            if (emailValidator().test(email[0])) {
                vendorEmail = email[0];
                vendorName = "NIL";
            } else {
                vendorEmail = "NIL";
                vendorName = "NIL";
            }
        } else {
            alert(emailValidator().test($.trim(email[1])));
            if (emailValidator().test($.trim(email[1]))) {
                vendorName = email[0];
                vendorEmail = email[1];
            } else {
                vendorName = email[0];
                vendorEmail = "NIL";
            }

        }
        var _rowLength = $("#SelectedVendors tr").length;
        alert(vendorEmail);
        alert(vendorName);
        if (vendorEmail === "NIL" || vendorName === "NIL") {
            swal("Invalid Vendor Selected");
        } else {
            $("#SelectedVendors").append("<tr class = 'selectedData_" + _rowLength + "'><td>" + vendorName
            + "<input type=\"hidden\" name=\"item[" + _rowLength + "].VENDOR_ID\" value=\"" + msg + "\" class=\"form-control\" />"
            + "<input type=\"hidden\" name=\"item[" + _rowLength + "].VENDOR_NAME\" value=\"" + vendorName + "\" class=\"form-control\" />"
            + "<input type=\"hidden\" name=\"item[" + _rowLength + "].AUTO_EMAIL\" value=\"" + vendorEmail + "\" class=\"form-control\" />"
            + "</td>"
        + "<td>" + vendorEmail + "</td>"
         + "<td><a href='#' onclick = \"removeRow('selectedData_" + _rowLength + "')\"  class='btn btn-sm btn-danger remove'>"
        + "<i class='fa fa-trash'></i></a></td></tr>");
        }


        //alert(msg);
        //alert(val);
        //alert(email[1]);
    });
}
       )

var emailValidator = function () {
    var pattern = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    return pattern;


}
var setCount = function (_name, _val) {
    localStorage.setItem(_name, _val);
};

var getCount = function (_name) {
    var $count = localStorage.getItem(name)
    if ($count == null) {
        return 0;
    } else {
        return $count;
    }
};

function removeRow(id) {
    //alert(id);
    $('.' + id).remove();
};

function checkVendorBeforeSubmit(vendorList) {
    var countVendors = $("#" + vendorList + " tr").length;
    if (countVendors > 0) {
        return true;
    } else {
        swal("Select a Vendor to Proceed");
        return false;
    }
}
