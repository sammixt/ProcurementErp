
    function Approve () {
        $('#SubmitBtn').val('Processing....')
        $.ajax({
            type: 'POST',
            url: $('#Approval')[0].action,
            data: $('#Approval').serialize(),
            success: function (data) {
                if (data) {
                    window.location.reload();
                }             
            }

        })
    }
