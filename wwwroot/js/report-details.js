// Used Claude to help write the JavaScript to display images in a modal

document.addEventListener("DOMContentLoaded", function () {

    const viewFileUrl = document.body.dataset.viewFileUrl;
    const downloadFileUrl = document.body.dataset.downloadFileUrl;

    window.viewImage = function (fileId) {
        const imageUrl = `${viewFileUrl}?fileId=${fileId}`;
        const downloadUrl = `${downloadFileUrl}?fileId=${fileId}`;

        document.getElementById('modalImage').src = imageUrl;
        document.getElementById('downloadImageBtn').href = downloadUrl;

        const modal = new bootstrap.Modal(document.getElementById('imageModal'));
        modal.show();
    };

    // Auto-dismiss alerts
    setTimeout(function () {
        const alertElement = document.querySelector('.alert');
        if (alertElement) {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alertElement);
            bsAlert.close();
        }
    }, 5000);

});
