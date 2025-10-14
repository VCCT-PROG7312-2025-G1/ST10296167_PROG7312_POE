$(document).ready(function () {

    // --- Input Validation ---
    var currentError = null;
    var fields = ["Title", "Location", "Category", "Date", "Time", "Description"];

    function showError(field) {
        // Remove all error highlights and messages
        $('.is-invalid, .is-valid').removeClass('is-invalid is-valid');
        $('.invalid-feedback').text('');

        var $f = $('[name="' + field + '"]');
        $f.addClass('is-invalid');
        // Custom error messages for date and time
        if (field === "Date") {
            $f.siblings('.invalid-feedback').text('Please select a date');
        } else if (field === "Time") {
            $f.siblings('.invalid-feedback').text('Please select a time');
        } else {
            $f.siblings('.invalid-feedback').text($f.data('val-required'));
        }
        currentError = field;
    }

    // Only clear error for the current field on input/change
    $('input:not([type="file"]), textarea, select').on('input change', function () {
        var $f = $(this), name = $f.attr('name'), val = $f.val();

        if (currentError === name && val?.trim()) {
            $f.removeClass('is-invalid').addClass('is-valid');
            $f.siblings('.invalid-feedback').text('');
            currentError = null;
        } else if (val?.trim() && !currentError) {
            $f.addClass('is-valid');
        }
    });

    $('form[action*="AddEvent"]').on('submit', function (e) {
        var firstEmpty = fields.find(f => {
            var $input = $('[name="' + f + '"]');
            var val = $input.val();
            // For select and text, check for empty string
            return !val || !val.trim();
        });
        if (firstEmpty) {
            e.preventDefault();
            showError(firstEmpty);
            $('[name="' + firstEmpty + '"]').focus();
        }
    });
});