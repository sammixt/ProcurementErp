$(function () {
    $("#SubmitSummary").on('click', function (e) {
        e.preventDefault;
        $href = ajaxPath + "/Request/ProcessSummaryPage";
        $.ajax({
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            url: $href,
            success: function (data) {
                if (data == true) {
                    window.location.href = "../Request/Completed";
                }
            }
        })
    }); 
})

function ContactVendorForUpdates(e) {
    e.preventDefault;
    $('#alertVendorOnUpdate').text('Processing....')
    $href = ajaxPath + "/Request/ProcessSummaryPage?UpdateVendor=Update";
    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        url: $href,
        success: function (data) {
            if (data == true) {
                window.location.href = ajaxPath + "/Request/Completed";
            }
        }
    })
}
