window.onload = getRequestCount();
window.onload = getPendingRequestCount();
window.onload = PendingSourceReqCount();
function getRequestCount() {
    var $reqCountRoute = ajaxPath + "/Home/GetRequestHistoryCount"
    $.ajax({
        type: "GET",
        url: $reqCountRoute,
        success: function (data) {
            if (data.length > 0) {
                $('#ReqHistCount').html("<small class='label pull-right bg-blue'>" + data + "</small> ");
            }
        }
    })
}

function getPendingRequestCount() {
    var $reqCountRoute = ajaxPath + "/Home/GetPendingRequestCount"
    $.ajax({
        type: "GET",
        url: $reqCountRoute,
        success: function (data) {
            if (data.length > 0) {
                $('#PendingReqCount').html("<small class='label pull-right bg-blue'>" + data + "</small> ");
            }
        }
    })
}

function PendingSourceReqCount() {
    var $reqCountRoute = ajaxPath + "/Admin/CountPendingSourceReq"
    $.ajax({
        type: "GET",
        url: $reqCountRoute,
        success: function (data) {
            //alert(data);
            if (data.length > 0) {
                $('#PendingSourceReqCount').html("<small class='label pull-right bg-blue'>" + data + "</small> ");
            }
        }
    })
}

