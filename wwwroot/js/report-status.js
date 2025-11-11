// Used Claude to help write the JavaScript for Report Status page functionality

// Constants
const ITEMS_PER_PAGE = 10;
const MAX_VISIBLE_PAGES = 5;

// State
let currentPage = 1;
let updateStatusModal = null;
let currentIssueStatus = null;

// Initialize on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeAll);
} else {
    initializeAll();
}

function initializeAll() {
    initializePagination();
    restoreScrollPosition();
    setupPaginationEventListeners();
    initializeUpdateStatusModal();
    autoDismissAlerts();
}

// Auto dismiss success alerts after 3 seconds
function autoDismissAlerts() {
    const successAlerts = document.querySelectorAll('.alert-success');
    successAlerts.forEach(alert => {
        try {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            setTimeout(() => bsAlert.close(), 3000);
        } catch (e) {
            setTimeout(() => alert.style.display = 'none', 3000);
        }
    });
}

// Save scroll position before page unload 
window.addEventListener('beforeunload', () => {
    sessionStorage.setItem('scrollPos', window.scrollY.toString());
});

// Restore scroll position after page load
function restoreScrollPosition() {
    const scrollPos = sessionStorage.getItem('scrollPos');
    if (scrollPos) {
        window.scrollTo(0, parseInt(scrollPos, 10));
        sessionStorage.removeItem('scrollPos');
    }
}

// Pagination functions
function initializePagination() {
    const tableBody = document.getElementById('tableBody');
    if (!tableBody) return;

    currentPage = 1;
    updatePagination();
}

function getTotalPages() {
    const tableBody = document.getElementById('tableBody');
    if (!tableBody) return 0;
    const allRows = Array.from(tableBody.querySelectorAll('tr'));
    return Math.ceil(allRows.length / ITEMS_PER_PAGE);
}

function updatePagination() {
    const tableBody = document.getElementById('tableBody');
    if (!tableBody) return;

    const allRows = Array.from(tableBody.querySelectorAll('tr'));
    const totalPages = Math.ceil(allRows.length / ITEMS_PER_PAGE);

    // Reset to page 1 if current page is invalid
    if (totalPages > 0 && currentPage > totalPages) {
        currentPage = 1;
    }

    // Hide all rows, then show rows for current page
    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const endIndex = startIndex + ITEMS_PER_PAGE;
    
    allRows.forEach((row, index) => {
        row.style.display = (index >= startIndex && index < endIndex) ? '' : 'none';
    });

    renderPagination(totalPages);
}

function changePage(page) {
    const totalPages = getTotalPages();
    if (page >= 1 && page <= totalPages) {
        currentPage = page;
        updatePagination();
        
        // Scroll to top of table
        const tableContainer = document.querySelector('.table-container');
        if (tableContainer) {
            tableContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }
}

function setupPaginationEventListeners() {
    const paginationContainer = document.getElementById('paginationContainer');
    if (!paginationContainer) return;

    paginationContainer.addEventListener('click', (e) => {
        const pageLink = e.target.closest('.page-link');
        if (!pageLink) return;

        const pageItem = pageLink.closest('.page-item');
        if (!pageItem || pageItem.classList.contains('disabled')) return;

        e.preventDefault();

        // Handle page number from data attribute
        if (pageLink.dataset.page) {
            changePage(parseInt(pageLink.dataset.page, 10));
            return;
        }

        // Handle Previous/Next buttons
        const pageText = pageLink.textContent.trim();
        if (pageText === 'Previous' && currentPage > 1) {
            changePage(currentPage - 1);
        } else if (pageText === 'Next') {
            const totalPages = getTotalPages();
            if (currentPage < totalPages) {
                changePage(currentPage + 1);
            }
        } else if (!isNaN(pageText) && pageText !== '...') {
            changePage(parseInt(pageText, 10));
        }
    });
}

function createPaginationItem(text, isActive = false, isDisabled = false) {
    const li = document.createElement('li');
    li.className = `page-item${isActive ? ' active' : ''}${isDisabled ? ' disabled' : ''}`;
    
    const link = document.createElement(isDisabled ? 'span' : 'a');
    link.className = 'page-link';
    if (!isDisabled) {
        link.href = '#';
        if (!isNaN(text)) {
            link.dataset.page = text.toString();
        }
    }
    link.textContent = text;
    
    li.appendChild(link);
    return li;
}

function createEllipsisItem() {
    const li = document.createElement('li');
    li.className = 'page-item disabled';
    const span = document.createElement('span');
    span.className = 'page-link';
    span.textContent = '...';
    li.appendChild(span);
    return li;
}

function renderPagination(totalPages) {
    const paginationContainer = document.getElementById('pagination');
    if (!paginationContainer) return;

    paginationContainer.innerHTML = '';

    // Don't show pagination if only one page or no pages
    if (totalPages <= 1) return;

    // Previous button
    paginationContainer.appendChild(createPaginationItem('Previous', false, currentPage === 1));

    // Calculate page range
    let startPage = Math.max(1, currentPage - Math.floor(MAX_VISIBLE_PAGES / 2));
    let endPage = Math.min(totalPages, startPage + MAX_VISIBLE_PAGES - 1);

    if (endPage - startPage < MAX_VISIBLE_PAGES - 1) {
        startPage = Math.max(1, endPage - MAX_VISIBLE_PAGES + 1);
    }

    // First page and ellipsis
    if (startPage > 1) {
        paginationContainer.appendChild(createPaginationItem(1));
        if (startPage > 2) {
            paginationContainer.appendChild(createEllipsisItem());
        }
    }

    // Page number buttons
    for (let i = startPage; i <= endPage; i++) {
        paginationContainer.appendChild(createPaginationItem(i, i === currentPage));
    }

    // Last page and ellipsis
    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            paginationContainer.appendChild(createEllipsisItem());
        }
        paginationContainer.appendChild(createPaginationItem(totalPages));
    }

    // Next button
    paginationContainer.appendChild(createPaginationItem('Next', false, currentPage === totalPages));
}

// Update Status Modal functions
function initializeUpdateStatusModal() {
    const updateStatusModalElement = document.getElementById('updateStatusModal');
    if (!updateStatusModalElement) {
        console.error('Update status modal element not found in DOM');
        return;
    }

    if (typeof bootstrap === 'undefined') {
        console.error('Bootstrap is not loaded');
        return;
    }

    updateStatusModal = new bootstrap.Modal(updateStatusModalElement);

    // Handle update status button clicks
    document.addEventListener('click', (e) => {
        const updateButton = e.target.closest('.update-status-btn');
        if (updateButton) {
            e.preventDefault();
            openUpdateStatusModal(updateButton);
        }
    });

    // Handle form submission
    const form = document.getElementById('updateStatusForm');
    if (form) {
        form.addEventListener('submit', handleStatusUpdateSubmit);
    }
}

function openUpdateStatusModal(updateButton) {
    try {
        const issueId = updateButton.getAttribute('data-issue-id');
        const issueCategory = updateButton.getAttribute('data-issue-category');
        const issueStatus = parseInt(updateButton.getAttribute('data-issue-status'), 10);

        currentIssueStatus = issueStatus;

        document.getElementById('modalIssueId').textContent = issueId;
        document.getElementById('modalIssueCategory').textContent = issueCategory;
        document.getElementById('modalIssueIdInput').value = issueId;

        populateStatusDropdown(currentIssueStatus);
        updateStatusModal.show();
    } catch (error) {
        console.error('Error opening update status modal:', error);
    }
}

function populateStatusDropdown(currentStatus) {
    const statusSelect = document.getElementById('statusSelect');
    if (!statusSelect) return;

    statusSelect.innerHTML = '';

    // Status workflow
    const workflowOrder = [
        { value: 1, displayName: 'Submitted' },
        { value: 0, displayName: 'In Progress' },
        { value: 2, displayName: 'Resolved' }
    ];

    const currentIndex = workflowOrder.findIndex(s => s.value === currentStatus);
    const availableStatuses = currentIndex >= 0 
        ? workflowOrder.slice(currentIndex) 
        : workflowOrder;

    availableStatuses.forEach(status => {
        const option = document.createElement('option');
        option.value = status.value;
        option.textContent = status.displayName;
        option.selected = status.value === currentStatus;
        statusSelect.appendChild(option);
    });
}

function handleStatusUpdateSubmit(e) {
    e.preventDefault();

    const statusSelect = document.getElementById('statusSelect');
    const selectedStatus = parseInt(statusSelect.value, 10);

    // Close modal if status hasn't changed
    if (selectedStatus === currentIssueStatus) {
        updateStatusModal.hide();
        return;
    }

    // Submit the form
    e.target.submit();
}
