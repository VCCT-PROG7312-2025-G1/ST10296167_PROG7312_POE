$(document).ready(function () {
  var currentError = null;
  var fields = ["Title", "Content"];

  function showError(field) {
    $(".is-invalid, .is-valid").removeClass("is-invalid is-valid");
    $(".invalid-feedback").text("");

    var $f = $('[name="' + field + '"]');
    $f.addClass("is-invalid");
    $f.siblings(".invalid-feedback").text($f.data("val-required"));
    currentError = field;
  }

  $("input, textarea").on("input change", function () {
    var $f = $(this),
      name = $f.attr("name"),
      val = $f.val();

    if (currentError === name && val?.trim()) {
      $f.removeClass("is-invalid").addClass("is-valid");
      $f.siblings(".invalid-feedback").text("");
      currentError = null;
    } else if (val?.trim() && !currentError) {
      $f.addClass("is-valid");
    }
  });

  $('form[action*="AddAnnouncement"]').on("submit", function (e) {
    var firstEmpty = fields.find(
      (f) =>
        !$('[name="' + f + '"]')
          .val()
          ?.trim()
    );
    if (firstEmpty) {
      e.preventDefault();
      showError(firstEmpty);
      $('[name="' + firstEmpty + '"]').focus();
    }
  });
});
