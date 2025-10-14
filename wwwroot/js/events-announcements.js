// Events & Announcements Page JavaScript

document.addEventListener("DOMContentLoaded", function () {
    initializeEventInteractions();
    initializeModalFunctionality();
    restoreScrollPosition();
    removeSearchParam();
});

// Save scroll position before the page unloads
window.addEventListener('beforeunload', function () {
    sessionStorage.setItem('scrollPos', window.scrollY);
});

// Restore scroll position after the page loads
function restoreScrollPosition() {
    const scrollPos = sessionStorage.getItem('scrollPos');
    if (scrollPos) {
        window.scrollTo(0, parseInt(scrollPos));
        sessionStorage.removeItem('scrollPos');
    }
}

// Remove 'search' param from URL after refresh (stops unnecessary search calls)
function removeSearchParam() {
    const url = new URL(window.location);
    if (url.searchParams.has('search')) {
        url.searchParams.delete('search');
        history.replaceState(null, '', url);
    }
}

// Initialize modal functionality
function initializeModalFunctionality() {
    // Get modal elements
    const eventDetailsModal = document.getElementById('eventDetailsModal');
    const announcementDetailsModal = document.getElementById('announcementDetailsModal');

    // Check if modals exist
    if (!eventDetailsModal || !announcementDetailsModal) {
        console.error('Modal elements not found in DOM');
        return;
    }

    // Initialize Bootstrap modals
    const eventModal = new bootstrap.Modal(eventDetailsModal);
    const announcementModal = new bootstrap.Modal(announcementDetailsModal);

    // Event view details links
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('event-view-link')) {
            e.preventDefault();
            const eventCard = e.target.closest('.event-card');
            if (eventCard) {
                openEventModal(eventCard, eventModal);
            }
        }
    });

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

function openEventModal(eventCard, modal) {
    try {
        const title = eventCard.querySelector('.event-title').textContent.trim();
        const category = eventCard.querySelector('.event-category').textContent.trim();
        const dateBadge = eventCard.querySelector('.event-date-badge').textContent.trim();
        const location = eventCard.querySelector('.event-meta-item').textContent.trim();
        const description = eventCard.dataset.fullDescription ||
            eventCard.querySelector('.event-description').textContent.trim();

        // Determine the type (recommended vs normal) for styling
        const isRecommended = eventCard.closest('.recommended-events') !== null;
        const modalHeader = document.getElementById('eventDetailsModal').querySelector('.modal-header');

        // Update modal header color
        modalHeader.className = 'modal-header';
        if (isRecommended) {
            modalHeader.classList.add('modal-header-success');
        } else {
            modalHeader.classList.add('modal-header-primary');
        }

        // Populate modal content
        document.getElementById('eventModalTitle').textContent = title;
        document.getElementById('eventModalCategory').textContent = category;
        document.getElementById('eventModalDate').textContent = dateBadge;
        document.getElementById('eventModalLocation').textContent = location;
        document.getElementById('eventModalDescription').textContent = description;
        document.getElementById('eventModalDescription').style.whiteSpace = 'pre-wrap';
        document.getElementById('eventModalDescription').style.wordWrap = 'break-word';

        modal.show();
    } catch (error) {
        console.error('Error opening event modal:', error);
    }
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

//Initialize event interactions and click handlers
function initializeEventInteractions() {
    const viewLinks = document.querySelectorAll(".event-view-link");

    viewLinks.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            // Modal handling is now done by initializeModalFunctionality()
        });
    });
}


// Utility function to format dates
function formatDate(dateString) {
    const options = {
        year: "numeric",
        month: "long",
        day: "numeric",
    };
    return new Date(dateString).toLocaleDateString("en-US", options);
}

// Utility function to format time
function formatTime(timeString) {
    const options = {
        hour: "2-digit",
        minute: "2-digit",
    };
    return new Date("2000-01-01 " + timeString).toLocaleTimeString(
        "en-US",
        options
    );
}

//Function to truncate text
function truncateText(text, maxLength) {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + "...";
}

//Search functionality for client-side filtering
function filterEvents(searchTerm) {
    const eventCards = document.querySelectorAll(".event-card");

    eventCards.forEach((card) => {
        const title = card.querySelector(".event-title").textContent.toLowerCase();
        const description = card
            .querySelector(".event-description")
            .textContent.toLowerCase();
        const category = card
            .querySelector(".event-category")
            .textContent.toLowerCase();

        const matchesSearch =
            title.includes(searchTerm.toLowerCase()) ||
            description.includes(searchTerm.toLowerCase()) ||
            category.includes(searchTerm.toLowerCase());

        if (matchesSearch) {
            card.style.display = "block";
        } else {
            card.style.display = "none";
        }
    });
}

// Smooth scroll to sections 
function scrollToSection(sectionId) {
    const section = document.getElementById(sectionId);
    if (section) {
        section.scrollIntoView({ behavior: "smooth" });
    }
}

//Reset all filters and submit the form
function resetFilters(event) {
    event.preventDefault();
    const form = document.getElementById('filterForm');
    if (form) {
        const categorySelect = form.querySelector('#category');
        const startDateInput = form.querySelector('#startDate');
        const endDateInput = form.querySelector('#endDate');

        if (categorySelect) categorySelect.value = '';
        if (startDateInput) startDateInput.value = '';
        if (endDateInput) endDateInput.value = '';

        form.submit();
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//