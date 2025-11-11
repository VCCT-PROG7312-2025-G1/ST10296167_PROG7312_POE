# ZA Municipal Services Application

---

### _GitHub Repository:_

[https://github.com/VCCT-PROG7312-2025-G1/ST10296167_PROG7312_POE.git](https://github.com/VCCT-PROG7312-2025-G1/ST10296167_PROG7312_POE.git)

### _YouTube Demo Videos:_

#### Part 2 Video
[https://youtu.be/tAApxcHLAY8](https://youtu.be/tAApxcHLAY8)

#### Part 1 Video
[https://youtu.be/iX2-10jpqV0?si=o3jmx8cb9076vzQd](https://youtu.be/iX2-10jpqV0?si=o3jmx8cb9076vzQd)

---

## Setting Up the Application

### Setting up the Development Environment

1. Install Visual Studio 2022 or later with the following workloads:

   - .NET 8.0 SDK or later
   - ASP.NET Core

2. Clone the project code from GitHub or extract the project `.zip` file to a folder of your choosing

3. Navigate to the folder and open the `.sln` file using Visual Studio

## Building and Running the Application

Once you have opened the project in Visual Studio:

1. Navigate to the Solution Explorer (View -> Solution Explorer)
2. Right-click the solution file
3. Click **Restore NuGet Packages**
4. Verify the project includes the following NuGet packages:
   - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
   - Microsoft.AspNetCore.Identity.EntityFrameworkCore
   - Microsoft.AspNetCore.Identity.UI
   - Microsoft.EntityFrameworkCore.Sqlite
   - Microsoft.EntityFrameworkCore.Tools
   - Microsoft.VisualStudio.Web.CodeGeneration.Design
5. Build the solution: **Build -> Build Solution**
6. Run the application

---

## Data Structure Usage

### Singleton DataStore Design Pattern

All data structures in this application are centralized within a **singleton `DataStore` class** (found in `Data/DataStore.cs`). This design choice provides several key advantages:

- **Centralized State Management**: All in-memory data structures are managed in one location, making the application state predictable and easier to debug.
- **Cross-Service Accessibility**: Services across the application can access the same data structures without duplication, ensuring consistency.
- **Performance Optimization**: Frequently accessed data (like events, announcements, and search history) is cached in memory for fast retrieval.
- **Session Persistence**: The singleton pattern ensures data persists across requests during the application lifetime.

The `DataStore` is registered as a singleton in `Program.cs` using `builder.Services.AddSingleton<DataStore>()` and is injected into services that require access to these data structures.

### List

**Location**: `DataStore.CurrentRecommendations` (List<Event>)

**Purpose**: Caches the current set of recommended events for quick display.

**Usage**: Generated recommendations from the recommendation engine are stored in this list to avoid recalculating recommendations on every page load. The list is populated by the `RecommendationsGenerator` class after analyzing search history and scoring available events. Users see these recommendations displayed in a dedicated section on the Events and Announcements page. Implementation can be found in `Services/Event/EventService.cs` and `Services/Recommendation/RecommendationsGenerator.cs`.

### Linked List

**Location**: `DataStore.UserFeedback` (LinkedList<Feedback>)

**Purpose**: Maintains user feedback submissions in chronological order with efficient insertion.

**Usage**: After users submit an issue report, they are prompted to provide feedback via a star rating system. This feedback is appended to the end of the linked list using `AddLast()`. The linked list structure is ideal here because feedback is always added at the end and traversed sequentially when analyzing user satisfaction trends. Implementation can be found in `Services/Report/ReportService.cs`.

### Dictionary

**Location**: `DataStore.ReportedIssues` (Dictionary<int, Issue>)

**Purpose**: Stores reported issues with O(1) lookup by issue ID.

**Usage**: When users submit issue reports through the Report Issues feature, each issue is assigned a unique ID and stored in this dictionary. The dictionary allows for fast retrieval when displaying submitted issues or updating issue status. Key methods can be found in `Services/Report/ReportService.cs` where issues are added using `_dataStore.ReportedIssues.Add(issue.ID, issue)`.

### SortedDictionary

**Location**: `DataStore.EventsByDate` (SortedDictionary<DateTime, List<Event>>)

**Purpose**: Organizes events chronologically by date and time, automatically maintaining sorted order.

**Usage**: In the Events and Announcements feature, events are grouped by their date/time and automatically sorted. This allows efficient retrieval of events within date ranges and ensures the events are always displayed chronologically without manual sorting. When users search for events between specific dates, the sorted structure enables efficient range queries. See `Services/Event/EventService.cs` for implementation details.

### Queue

**Location**: `DataStore.SearchHistory` (Queue<SearchQuery>)

**Purpose**: Tracks the most recent user searches in FIFO order for generating personalized event recommendations.

**Usage**: Each time a user searches for events using category or date filters, the search query is enqueued. The queue is limited to the 3 most recent searches (dequeuing older searches when the limit is exceeded). This search history powers the recommendation engine, which analyzes patterns to suggest relevant upcoming events. The recommendation algorithm applies recency weighting, giving more importance to the most recent searches. Implementation spans `DataStore.LogSearch()` and `Services/Recommendation/RecommendationEngine.cs`.

### Stack

**Location**: `DataStore.RecentAnnouncements` (Stack<Announcement>)

**Purpose**: Maintains announcements in LIFO order to quickly access the most recent announcements.

**Usage**: Municipal announcements are pushed onto the stack as they are created, ensuring the most recent announcements are always at the top. When displaying announcements on the Events and Announcements page, the application pops or peeks the top items from the stack to show users the latest updates first. See `Services/Announcement/AnnouncementService.cs` where announcements are pushed using `_dataStore.RecentAnnouncements.Push(announcement)`.

### HashSet

**Location**: `DataStore.UniqueCategories` (HashSet<string>)

**Purpose**: Stores unique event categories without duplicates for populating filter dropdowns.

**Usage**: When users access the Events and Announcements page, the application extracts all unique event categories from available events and stores them in this hash set. The O(1) add and lookup operations ensure efficient duplicate checking, and the resulting category list populates the search filter dropdown. The hash set is cleared and repopulated each time categories are requested. See `Services/Event/EventService.cs` in the `GetAllCategoriesAsync()` method.

---

## Service Request Status Feature - Data Structure Usage 

### Overview

The **Service Request Status** feature utilizes three advanced data structures that work together to provide efficient organization, retrieval, and display of service requests. Each data structure serves a specific purpose in the data pipeline, from initial storage, to filtering, to final prioritized display.


### AVL Tree

**Location**: `DataStructures/ReportsAVLTree.cs` -> `DataStore.ReportsAVLTree`

**Purpose**: Primary storage structure for all service requests, indexed by unique Issue ID with guaranteed logarithmic time complexity for all operations.

**Role & Efficiency Contribution**:

The AVL Tree serves as the backbone of the Service Request Status feature, providing **O(log n)** search, insertion, and deletion operations even as the number of issues grows. Unlike a standard Binary Search Tree which can degrade to O(n) in worst-case scenarios (unbalanced tree), the AVL tree maintains balance through automatic rotations after each insertion or update.

**How It Works**:

When a new issue is reported, it is inserted into the AVL tree based on its ID. The tree automatically performs rotations (Left-Left, Right-Right, Left-Right, Right-Left) to maintain a height balance factor of ±1 at every node. This self-balancing property ensures fast operations regardless of insertion order.

**Examples**:

1. **Direct ID Search**: When a user enters "Issue #42" in the search filter, the AVL tree traverses from root to leaf in at most log₂(n) steps. With 1000 issues, this requires only ~10 comparisons versus 500 comparisons on average with a linear search.

2. **Sorted Display**: The `InOrder()` traversal method visits nodes in ascending ID order (left subtree → root → right subtree), automatically producing a sorted list of all issues in O(n) time without additional sorting overhead.

3. **Status Updates**: When an employee updates an Issue from "Submitted" to "InProgress", the AVL tree deletes the node, updates the issue object, and re-inserts it while maintaining balance through rotations.

**Code Implementation Highlights**:
```csharp
// Automatic balancing after insertion

// Left Left Case
if (balance > 1 && report.ID < node.LeftNode!.Data.ID)
{
    return RotateRight(node);
}

// Right Right Case
if (balance < -1 && report.ID > node.RightNode!.Data.ID)
{
    return RotateLeft(node);
}

// Left Right Case
if (balance > 1 && report.ID > node.LeftNode!.Data.ID)
{
    node.LeftNode = RotateLeft(node.LeftNode!);
    return RotateRight(node);
}

// Right Left Case
if (balance < -1 && report.ID < node.RightNode!.Data.ID)
{
    node.RightNode = RotateRight(node.RightNode!);
    return RotateLeft(node);
}
```
<br> 

**Efficiency Benefits**:
- Search by ID: **O(log n)**
- Insert new issue: **O(log n)**
- In-order traversal: **O(n)**
- Tree height with 1000 issues: **~10 levels** (vs. up to 1000 in unbalanced BST)

### Graph

**Location**: `DataStructures/ReportsGraph.cs` → `DataStore.ReportsGraph`

**Purpose**: Models relationships between service requests to identify patterns, detect potential duplicates, and enable **Related Issues** functionality.

**Role & Efficiency Contribution**:

The Graph data structure captures the reality that service requests are not isolated and issues reported in the same category within a short time window are likely related (e.g., multiple potholes on the same road). By representing issues as nodes and relationships as edges, the graph enables **O(V + E)** traversal to find all connected issues, where V is vertices (issues) and E is edges (relationships).

**How It Works**:

Each issue is a vertex in the graph. When a new issue is added, the system compares it against existing issues and creates bidirectional edges if two conditions are met:
1. **Same Category**: Both issues belong to the same category (e.g., "Road Damage")
2. **Temporal Proximity**: Issues were reported within 3 days of each other (proximity could be made closer in real-world scenarios))

The graph stores edges efficiently as `Dictionary<int, HashSet<int>>`, where each issue ID maps to a set of connected issue IDs.

**Examples**:

1. **Related Issues (BFS Traversal)**: When viewing Issue #23 (pothole on Main Street), the "Related Issues" section displays Issues #24 and #25 (also potholes on Main Street, reported 1-2 days apart). The BFS algorithm starts at Issue #23, explores immediate neighbors, then their neighbors, efficiently finding all connected issues in the cluster.

2. **Duplicate Detection**: Three users report the same broken streetlight (Issues #67, #68, #69) within 2 days. The graph connects all three, and the system can flag them as potential duplicates for employee review, preventing redundant work orders.

3. **Connected Components (DFS)**: The `GetAllConnectedComponents()` method uses Depth-First Search to identify clusters of related issues. For example, it might discover a cluster of 5 water-related issues in Durban North reported within a 3-day span, suggesting a systemic problem requiring coordinated resolution.

**Code Implementation Highlights**:
```csharp
// Edge creation logic
bool sameCategory = report.Category == other.Category;
bool closeTime = Math.Abs((report.CreatedAt - other.CreatedAt).TotalDays) <= 3;

if (sameCategory && closeTime)
    AddEdge(report.ID, other.ID);  // Bidirectional connection
```
<br>

**Efficiency Benefits**:
- Find related issues (BFS): **O(V + E)** where E is typically small
- Add new issue and create edges: **O(n)** where n is existing issues
- Check if edge exists: **O(1)** using HashSet
- Connected components (DFS): **O(V + E)**

### Min-Heap

**Location**: `DataStructures/ReportsMinHeap.cs` → `DataStore.ReportsMinHeap`

**Purpose**: Optimizes the display order of service requests by sorting them based on a combined priority score that adapts to user role (user vs. employee), ensuring the most relevant issues appear first for each audience.

**Role & Efficiency Contribution**:

The Min-Heap provides **O(n log n)** sorting for display purposes, significantly faster than comparison-based sorts for repeatedly extracting the highest-priority items. More importantly, it allows **O(log n)** insertion of new issues and **O(1)** access to the highest-priority issue, making it ideal for dynamic priority queues where different users require different orderings.

**How It Works**:

The heap is a complete binary tree stored as an array where each parent node has a priority value less than or equal to its children (min-heap property). The unique aspect of this implementation is **role-based priority calculation**:

**For Users**:
1. **Primary Factor**: Issue Status with priority order:
   - InProgress (0) - Issues actively being worked on
   - Submitted (1) - Issues awaiting attention
   - Resolved (2) - Completed issues
2. **Secondary Factor**: Creation Date (newer issues prioritized within the same status)

**For Employees**:
1. **Primary Factor**: Issue Status with **inverted priority** for actionable items:
   - Submitted (-1) - Remapped to highest priority
   - InProgress (0) - Currently being handled
   - Resolved (2) - Completed work
2. **Secondary Factor**: Creation Date (newer issues prioritized within the same status)

This role-based prioritization ensures users see service requests actively being worked on first, while employees see new submitted service request first (optimizing their workflow).

When extracting issues for display, the heap repeatedly removes the root (minimum priority), replaces it with the last element, and performs "heapify down" to restore the heap property in O(log n) time per extraction.

**Examples**:

1. **User - Priority**: After filtering issues by category "Road Damage", 50 issues remain. For a citizen viewing the request status page, the heap orders them to show:
   - All "InProgress" issues first (Status=0) - demonstrating active work
   - Within "InProgress", newest issues appear first (showing recent activity)
   - Then "Submitted" issues (Status=1) - pending requests
   - Finally "Resolved" issues (Status=2) - completed work

   This ordering builds public trust by highlighting that issues are actively being addressed.

2. **Employee - Priority**: The same 50 issues are reordered for employees using the inverted priority:
   - All "Submitted" issues first (remapped to -1) - requiring immediate attention
   - Within "Submitted", newest reports appear first 
   - Then "InProgress" issues (Status=0) - currently being handled
   - Finally "Resolved" issues (Status=2) - for reference

   This ensures employees immediately see unassigned work requiring action, optimizing task allocation.

3. **Dynamic Re-Prioritization**: Consider these three issues after a status update:
   - Issue #10: InProgress, reported 5 days ago
   - Issue #11: Submitted, reported 2 days ago  
   - Issue #12: InProgress, reported 1 day ago

   **User sees**: #12 (InProgress, newest) → #10 (InProgress, older) → #11 (Submitted)
   
   **Employee sees**: #11 (Submitted, needs action) → #12 (InProgress, newest) → #10 (InProgress, older)

   The same dataset produces two different priority orders optimized for each user type's needs.

**Code Implementation Highlights**:
```csharp
private int ComparePriority(Issue issue1, Issue issue2)
{
    int status1 = (int)issue1.Status;
    int status2 = (int)issue2.Status;

    // Invert priority for employees: Submitted becomes highest priority
    if (_isEmployee)
    {
        status1 = (issue1.Status == IssueStatus.Submitted) ? -1 : (int)issue1.Status;
        status2 = (issue2.Status == IssueStatus.Submitted) ? -1 : (int)issue2.Status;
    }

    // Primary: Status (lower value = higher priority)
    int statusComparison = status1.CompareTo(status2);
    if (statusComparison != 0)
        return statusComparison;
    
    // Secondary: Newer issues first within same status
    return issue2.CreatedAt.CompareTo(issue1.CreatedAt);
}
```
<br>

**Efficiency Benefits**:
- Build heap from n issues: **O(n log n)**
- Extract minimum (highest priority): **O(log n)**
- Peek at minimum: **O(1)**
- Extract all sorted: **O(n log n)** total
- Role-based priority recalculation: **O(1)** per comparison

---

## Database Usage

A local SQLite database is used to persist user data, reported issues, events, and announcements across sessions.

> **Note:** There is no need to manually create or setup the database. A local SQLite database file will be created and populated when first running the application.

This application includes one distinct user role: **Employee**. Employees are pre-registered in the database.

The following employee account is automatically created in the local database when first running the app:

**Default Employee**

- **Email:** employee@municipal.gov.za
- **Password:** Employee123!

> **Important:** You must log in as an employee to add events and announcements.

---

## Using the Application

When running the application, users will be presented with a menu to choose from the following options:

1. **Report Issues**
2. **Local Events and Announcements**
3. **Service Request Status**

### 1. Report Issues

#### Adding an Issue

Selecting _Report Issue_ will prompt the user to fill in an issue report form requiring the following details:

- Address
- Suburb
- Category
- Description
- Relevant Files (Optional)

Users can submit the form once all required fields are filled in by clicking the **Submit** button. A confirmation message will be displayed upon successful submission.

In order to improve user enagement, the user engagment strategy of in-app feedback will be used to allow users to rate their experience submitting the report through an in-app survey that utilizes a user-friendly **star rating** system in combination with an optional feedback option.

---

### 2. Local Events and Announcements

When selecting _Local Events and Announcements_ from the menu, users will be met with a single page displaying:
- The 10 most recent announcements 
- A list of all events with search filter options & recommendations

#### Recent Announcements

The most recent announcements will be displayed at the top of the page with horisontal scrolling to browse through them.

Each announcement will show the following details:
- Category
- Date
- Title
- Content

Users can click _Read More_ to view the full announcement content.

Additionally, users can click _View All_ to navigate to a dedicated announcements page displaying all announcements in reverse chronological order.

#### Events List

Below the announcements section, users will find a list of all upcoming events. Each event entry will display the following details:
- Title
- Category
- Date and Time
- Location
- Description

Users can click _Event Details_ to view the full event details.

#### Search Filters & Recommendations

Users can filter events using the following search criteria:
- Category 
- Start Date
- End Date

A user can search using any combination of these filters.

After performing a search, the application will log the search query and update the user's search history. Based on this search history, the application will then generate personalized event recommendations displayed in a dedicated section above the events list.

The recommendation algorithm analyzes a user's recent search patterns, creating individual scores for each event based on 

- Category Frequency
- Date Range Preference
- Recency Weighting
- Proximity to Current Date

#### Login (Employee Only)
Employees can log in to access employee-only features by clicking the **Login** button located at the top right corner of the application.

#### Adding Announcements (Employee Only)
In order to add announcements, an employee must first log in using their credentials.

Selecting the **Add Announcement** button will prompt the employee to fill in an announcement form requiring the following details:
- Title
- Category
- Content

Employees can submit the form once all required fields are filled in by clicking the **Submit** button. A confirmation message will be displayed upon successful submission.

#### Adding Events (Employee Only)
In order to add events, an employee must first log in using their credentials.

Selecting the **Add Event** button will prompt the employee to fill in an event form requiring the following details:
- Title
- Location
- Category
- Date and Time
- Description

Employees can submit the form once all required fields are filled in by clicking the **Submit** button. A confirmation message will be displayed upon successful submission.

#### Logging Out
Employees can log out of their account at any time by clicking the **Logout** button located at the top right corner of the application.

---

### 3. Service Request Status

#### Service Requests

Selecting _Service Request Status_ from the menu will direct all users to a service requests page which displays a list of all reported issues with the following details:

- Issue ID
- Category
- Location (Address and Suburb)
- Status
- Uploaded file count
- View Details button
- Related Issues count

This list can then be filtered using any the following criteria:

- Issue ID
- Category
- Status
- Start Date
- End Date

If users wish to view more details about a specific issue, they can click the **View Details** button associated with that issue. This will display the full details of the issue, including attached files (which the user can view or download) as well as any related issues that have been identified by the system.

#### Service Requests (Employee Only)

When an employee logs in and navigates to the Service Request Status page, they will have access to additional features that allow them to manage reported issues. These features include:

- **Statistics Overview:** At the top of the page, employees will see a statistics overview section displaying key metrics such as the total number of reported issues and the number of issues in each status category.
- **Update Issue Status:** Employees can update the status of any reported issue by clicking the **Update** button, selecting the desired updated status from a dropdown menu (only future states) and confirming their choice via the **Confirm** button. This updated status can then be viewed by all users.

---

## AI Declaration

### Demo Data Seeding

**Tool:** CoPilot (GPT-5 mini)

**Purpose:**

CoPilot was used to create methods to seed my database with demo data for testing based on my applications models.

### CSS & JavaScript

**Tool:** Claude Sonnet 4.5

**Purpose:**

I workshopped with Claude to get it to help generate the css used for my Events and Announcements and Report Status views.
Additioanlly Claude was consulted in order to help me write the necessary JavaScript to smoothen and enhance my applications UX.

### Recommendations 

**Tool:** Claude Sonnet 4.5

**Purpose:**

Claude was used to help me brainstorm and develop the point, proximity, and weight based scoring system used for my recommendation algorithm. 
Claude was then subsequently used to suggest and help develop improvements to my initial algorithm approach and it helped debug my code with the addition of detailed console logs.

### Events and Recommendations View

**Tool:** Claude Sonnet 4.5

**Purpose:**

Claude was used to help improve the HTML layout of my view in addition to helping me implement the in-page modal popup for event and announcements details.

### Advanced Data Structures 

**Tool:** Claude Sonnet 4.5

**Purpose:**

Claude was used to help brainstorm which advanced data structures would be best suited for my Service Request Status feature and was consulted during the development of these data structures to help me learn how they work, improve my implementations, and debug some issues I encountered.
