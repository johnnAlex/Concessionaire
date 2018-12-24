$(function () {
    $('.toggleClaim').change(function () {
        var self = $(this);
        var value = self.prop('checked');
        $('.check').prop('checked', value);
        document.getElementById('btn').click()
    });
});