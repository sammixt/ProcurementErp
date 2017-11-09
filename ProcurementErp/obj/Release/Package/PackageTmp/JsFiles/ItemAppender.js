$(function () {

    $('.add').click(function () {
        var _rowLength = $("#ItemsInput tr").length;
        var $sn = _rowLength + 1;
        var _count = $sn - 1;
        $("#ItemsInput").append("<tr id=\"NewItem_" + $sn + "\">"
        + "<td>" + $sn + "</td>"
        + "<td><input type=\"text\" name=\"item[" + _count + "].DESCRIPTION\" class=\"form-control\" required /></td>"
        + "<td><input type=\"text\" name=\"item[" + _count + "].UNIT_OF_MEAS\" class=\"form-control\" /></td>"
        + "<td><input type=\"number\" name=\"item[" + _count + "].QUANTITY\" class=\"form-control\" required /></td>"
        + "<td><a href=\"#\" onclick=\"removeRow('NewItem_" + $sn + "')\" class=\"btn btn-sm btn-danger\"><i class=\"fa fa-trash\"></i></a></td>"
        + "</tr>");
    });
}
  )

var emailValidator = function () {
    var pattern = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
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
    $('#' + id).remove();
};


function checkItemBeforeSubmit(vendorList) {
    var countVendors = $("#" + vendorList + " tr").length;
    if (countVendors > 0) {
        return true;
    } else {
        swal("Add an Item to Proceed");
        return false;
    }
}