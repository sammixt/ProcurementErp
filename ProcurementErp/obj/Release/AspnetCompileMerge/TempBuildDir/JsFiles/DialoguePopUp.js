$(function () {
    $('.view').on('click', function (e) {
        e.preventDefault();
        var $href = $(this).attr('href');
        $.ajax({
            type: "GET",
            url: $href,
            success: function (data) {
                $('#pendingRequestContainer').html(data);
                $('#Pending').css('display', 'none');
            }
        })
    });
})

var remove = function () {
    $('#PopUpView').css('display', 'none');
    $('#Pending').css('display', 'block');
}
