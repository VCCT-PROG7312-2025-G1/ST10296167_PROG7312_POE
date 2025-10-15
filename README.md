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

> [!NOTE]
> Menu item **3** is not yet implemented and will be available in future updates.

### Reporting Issues

When selecting _Report Issues_ from the menu, users will be met with a further sub-menu with the following options:

1. **Add Issue**
2. **Submitted Issues**

#### Adding an Issue

Selecting _Add Issue_ will prompt the user to fill in an issue report form requiring the following details:

- Address
- Suburb
- Category
- Description
- Relevant Files (Optional)

Users can submit the form once all required fields are filled in by clicking the **Submit** button. A confirmation message will be displayed upon successful submission.

In order to improve user enagement, the user engagment strategy of in-app feedback will be used to allow users to rate their experience submitting the report through an in-app survey that utilizes a user-friendly **star rating** system in combination with an optional feedback option.

#### Viewing Submitted Issues

Selecting _Submitted Issues_ will display a list of all issues that have been reported by the user. Each issue entry will show the following details:

- Issue ID
- Category
- Location (Address and Suburb)
- Description
- Issue Status
- Uploaded files

If any files have been uploaded with the issue, users will be able to download and view it.

---

### Local Events and Announcements

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

## AI Declaration

### Demo Data Seeding

**Tool:** CoPilot (GPT-5 mini)

**Purpose:**

CoPilot was used to create methods to seed my database with demo data for testing based on my applications models.

### CSS & JavaScript

**Tool:** Claude Sonnet 4.5

**Purpose:**

I workshopped with Claude to get it to help generate the css used for my Events and Announcements View.
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
