function appendToStorage(name, data) {
    var old = localStorage.getItem(name);
    if (old === null)
    { old = " "; } else {
        old += " > ";
    }
    localStorage.setItem(name, old + data);
}

function getItemInStorage(name) {
    var item = localStorage.getItem(name);
    return item;
}

function clearItemInStorage(name) {
    localStorage.removeItem(name);
    //localStorage.clear();
}

function getSelectedValue($selectedId, $containerId) {
    var $displayTextId = $('#CategoryHolder');
    var $select = $('#' + $selectedId);
    var $selectedValue = $select.val();
    var $selectedText = $('#' + $selectedId + ' option:selected').text();
    var $href = ajaxPath+'/Home/GetCategoryById?ParentId=' + $selectedValue;
    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        url: $href,
        success: function (data) {
            var dataLength = data.length
            if (dataLength > 0) {
                $select.empty();
                $.each(data, function (key, value) {
                    $select.append("<option value='" + data[key].CATEGORY_NUM + "'>" + data[key].CATEGORY_NAME + "</option>");
                })
                $select.prepend("<option selected='selected'>Select sub-category</option>");
               
            } else {
                $select.css("display", "none");
            }
            appendToStorage('Category', $selectedText);
            appendToStorage('CategoryID', $selectedValue);
            $displayTextId.text(getItemInStorage('Category'));
        }
    })
}

function Clear() {
        var $select = $('#Category');
        var $displayTextId = $('#CategoryHolder');
        $select.css("display", "block");
        $displayTextId.text("");
        $select.empty();
        clearItemInStorage('Category');
        clearItemInStorage('CategoryID');
        var $href = ajaxPath + '/Home/GetCategoryById?ParentId=' + 0;
        $.ajax({
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            url: $href,
            success: function (data) {
                var dataLength = data.length
                if (dataLength > 0) {
                    $select.empty();
                    $select.prepend("<option> </option>");
                    $.each(data, function (key, value) {
                        $select.append("<option value='" + data[key].CATEGORY_NUM + "'>" + data[key].CATEGORY_NAME + "</option>");
                    })

                } else {
                    $select.css("display", "none");
                }
            }
        })
}

function SaveCategory() {
   // alert(getItemInStorage('Category'))
    //alert(getItemInStorage('CategoryID'))
    $(".CategorySelectedDiv").css("display", "block")
    $("#CategorySelected").val(getItemInStorage('Category'));
    $(".CategorySelected").val(getItemInStorage('Category')); 
    $("#CategorySelectedID").val(getItemInStorage('CategoryID'));
    clearItemInStorage('Category');
    clearItemInStorage('CategoryID');
}

function getVendorOnChange(id, className) {
    //alert(id); alert(className);
    var $ClassNameId = id;
    var val = $("." + className).val();
    var vendor = $('#vendors option').filter(function () {
        return this.value == val;
    }).data('vendor');
    var msg = vendor ? vendor : 'No Match';
    $("." + $ClassNameId).val(msg);
    //alert(msg);
}





