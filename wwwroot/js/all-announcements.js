// Used Claude to help write JavaScript for the modal and read more frontend functionality

document.addEventListener("DOMContentLoaded", function () {
    initializeModalFunctionality();
});

// Initialize modal functionality
function initializeModalFunctionality() {
    // Get modal elements
    const announcementDetailsModal = document.getElementById('announcementDetailsModal');

    // Check if modal exists
    if (!announcementDetailsModal) {
        console.error('Announcement modal element not found in DOM');
        return;
    }

    // Initialize Bootstrap modal
    const announcementModal = new bootstrap.Modal(announcementDetailsModal);

    // Announcement read more links
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('announcement-read-more')) {
            e.preventDefault();
            const announcementCard = e.target.closest('.announcement-card');
            if (announcementCard) {
                openAnnouncementModal(announcementCard, announcementModal);
            }
        }
    });
}

function openAnnouncementModal(announcementCard, modal) {
    try {
        const title = announcementCard.querySelector('.announcement-title').textContent.trim();
        const category = announcementCard.querySelector('.announcement-category').textContent.trim();
        const date = announcementCard.querySelector('.announcement-date').textContent.trim();
        const content = announcementCard.dataset.fullContent ||
            announcementCard.querySelector('.announcement-content').textContent.trim();

        // Determine announcement type for styling based on category
        const modalHeader = document.getElementById('announcementDetailsModal').querySelector('.modal-header');
        modalHeader.className = 'modal-header';

        if (category === 'Information') {
            modalHeader.classList.add('modal-header-info');
        } else if (category === 'Alert') {
            modalHeader.classList.add('modal-header-alert');
        } else if (category === 'Update') {
            modalHeader.classList.add('modal-header-update');
        }

        // Populate modal content
        document.getElementById('announcementModalTitle').textContent = title;
        document.getElementById('announcementModalCategory').textContent = category;
        document.getElementById('announcementModalDate').textContent = date;
        document.getElementById('announcementModalContent').textContent = content;
        document.getElementById('announcementModalContent').style.whiteSpace = 'pre-wrap';
        document.getElementById('announcementModalContent').style.wordWrap = 'break-word';

        modal.show();
    } catch (error) {
        console.error('Error opening announcement modal:', error);
    }
}