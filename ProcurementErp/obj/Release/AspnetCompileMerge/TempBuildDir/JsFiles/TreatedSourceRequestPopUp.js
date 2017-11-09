$(function () {
    $('.view').on('click', function (e) {
        e.preventDefault();
        var $href = $(this).attr('href');
        $.ajax({
            type: "GET",
            url: $href,
            success: function (data) {
                $('#RequestHistoryContainer').html(data);
                $('#History').css('display', 'none');
            }
        })
    });
})

var remove = function () {
    $('#PopUpView').css('display', 'none');
    $('#History').css('display', 'block');
}