// Events & Announcements Page JavaScript

document.addEventListener("DOMContentLoaded", function () {
    initializeEventInteractions();
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


//Initialize event interactions and click handlers
function initializeEventInteractions() {
    const viewLinks = document.querySelectorAll(".event-view-link");

    viewLinks.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const eventTitle =
                this.closest(".event-card").querySelector(".event-title").textContent;
            // TODO: Implement event details modal or navigation
            console.log("View details for event:", eventTitle);
            alert("Event details functionality to be implemented");
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