$(document).ready(function () {
    $('.toggleClaim').click(function () {
        var self = $(this);
        var value = self.prop('checked');
        $('.check').prop('checked', value);
        document.getElementById('btn').click()
    });
    $('.check').click(function () {
        // console.log($(this).prop('checked'));
        // $("#hiddenInput").val(pageClicked);
        document.getElementById('btn').click()
    });
});