# ZA Municipal Services Application
****

### *GitHub Repository:*
[https://github.com/VCCT-PROG7312-2025-G1/ST10296167_PROG7312_POE.git](https://github.com/VCCT-PROG7312-2025-G1/ST10296167_PROG7312_POE.git)

### *YouTube Demo Video:*
*Link*
****
## Setting Up and Running the Application
### Setting up the Development Environment

1. Install Visual Studio 2022 or later with the following workloads:
   - .NET 8.0 SDK or later
   - ASP.NET Core

2. Clone the project code from GitHub or extract the project `.zip` file to a folder of your choosing

3. Navigate to the folder and open the `.sln` file using Visual Studio

### Running the Application

Once you have opened the project in Visual Studio:
- Build the solution: **Build -> Build Solution**
- Run the application
****
## Using the Application
When running the application, users will be presented with a menu to choose from the following options:
1. **Report Issues**
2. **Local Events and Announcements**
3. **Service Request Status**

> [!NOTE]
> Menu items **2** and **3** are not yet implemented and will be available in future updates.

### Reporting Issues
When selecting *Report Issues* from the menu, users will be met with a further sub-menu with the following options:
1. **Add Issue**
2. **Submitted Issues**

#### Adding an Issue
Selecting *Add Issue* will prompt the user to fill in an issue report form requiring the following details:
- Address
- Suburb
- Category
- Description
- Relevant Files (Optional)

Users can submit the form once all required fields are filled in by clicking the **Submit** button. A confirmation message will be displayed upon successful submission.

In order to improve user enagement, users will then be prompted to rate their experience submitting the report through an in-app survey that utilizes a user-friendly **star rating** system in combination with an optional feedback option.

#### Viewing Submitted Issues
Selecting *Submitted Issues* will display a list of all issues that have been reported by the user. Each issue entry will show the following details:
- Issue ID
- Category
- Location (Address and Suburb)
- Description
- Issue Status
- Uploaded files

If any files have been uploaded with the issue, users will be able to download and view it.
***