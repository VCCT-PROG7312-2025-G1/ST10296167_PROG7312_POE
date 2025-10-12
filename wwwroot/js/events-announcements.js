// Events & Announcements Page JavaScript

document.addEventListener("DOMContentLoaded", function () {
  initializeFilters();
  initializeEventInteractions();
});

// Save scroll position before the page unloads
window.addEventListener('beforeunload', function () {
    sessionStorage.setItem('scrollPos', window.scrollY);
});

// Restore scroll position after the page loads
window.addEventListener('DOMContentLoaded', function () {
    const scrollPos = sessionStorage.getItem('scrollPos');
    if (scrollPos) {
        window.scrollTo(0, parseInt(scrollPos));
        sessionStorage.removeItem('scrollPos'); // optional cleanup
    }
});

function initializeEventInteractions() {
  // Add click handlers for event view buttons
  const viewButtons = document.querySelectorAll(".event-view-btn");

  viewButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.preventDefault();
      const eventTitle =
        this.closest(".event-card").querySelector(".event-title").textContent;
      // TODO: Implement event details modal or navigation
      console.log("View details for event:", eventTitle);
      // For now, just show an alert
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

// Function to truncate text
function truncateText(text, maxLength) {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + "...";
}

// Search functionality (if needed for client-side filtering)
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

function resetFilters(event) {
    event.preventDefault(); // prevent default button behavior
    const form = document.getElementById('filterForm');
    form.querySelector('#category').value = '';
    form.querySelector('#startDate').value = '';
    form.querySelector('#endDate').value = '';
    form.submit();
}
