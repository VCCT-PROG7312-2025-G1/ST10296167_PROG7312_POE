// Used Claude to help write JavaScript for frontend, real-time validation and form submission checks

$(document).ready(function () {

    // --- Input Validation ---
    var currentError = null;
    var fields = ['Address', 'Suburb', 'Category', 'Description'];

    function showError(field) {
        $('.is-invalid, .is-valid').removeClass('is-invalid is-valid');
        $('.invalid-feedback').text('');

        var $f = $('[name="' + field + '"]');
        $f.addClass('is-invalid');
        $f.siblings('.invalid-feedback').text($f.data('val-required'));
        currentError = field;
    }

    $('input:not(#Files), textarea, select').on('input change', function () {
        var $f = $(this), name = $f.attr('name'), val = $f.val();

        if (currentError === name && val?.trim()) {
            $f.removeClass('is-invalid').addClass('is-valid');
            $f.siblings('.invalid-feedback').text('');
            currentError = null;
        } else if (val?.trim() && !currentError) {
            $f.addClass('is-valid');
        }
    });

    $('form[action*="SubmitIssueReport"]').on('submit', function (e) {
        var firstEmpty = fields.find(f => !$('[name="' + f + '"]').val()?.trim());
        if (firstEmpty) {
            e.preventDefault();
            showError(firstEmpty);
            $('[name="' + firstEmpty + '"]').focus();
        }
    });

    // --- File Input Validation ---
    $('#Files').on('change', function () {
        const allowedTypes = [
            'image/jpeg', 'image/png', 'text/plain', 'application/pdf',
            'application/msword',
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
        ];
        const maxSize = 5 * 1024 * 1024;
        const $validationSpan = $('.invalid-feedback[data-valmsg-for="Files"]');

        $(this).removeClass('is-invalid is-valid');
        $validationSpan.text("");

        for (let file of this.files) {
            if (!allowedTypes.includes(file.type)) {
                $validationSpan.text(`File type not allowed: ${file.name}`);
                $(this).addClass('is-invalid');
                this.value = "";
                return;
            }
            if (file.size > maxSize) {
                $validationSpan.text(`File too large (max 5MB): ${file.name}`);
                $(this).addClass('is-invalid');
                this.value = "";
                return;
            }
        }
    });

    // --- Rating Modal & Stars ---
    const ratingTexts = {
        1: "Poor - We'll do better",
        2: "Fair - Room for improvement",
        3: "Good - Thanks for the feedback",
        4: "Very Good - We're glad to help!",
        5: "Excellent - You made our day!"
    };

    $('input[name="rating"]').on('change', function () {
        const rating = $(this).val();
        $('#ratingText').text(ratingTexts[rating]).removeClass('text-danger').addClass('text-primary');
    });

    $('form[action*="SubmitRating"]').on('submit', function (e) {
        if (!$('input[name="rating"]:checked').val()) {
            e.preventDefault();
            $('#ratingText').text('Please select a rating').removeClass('text-primary').addClass('text-danger');
        }
    });
});