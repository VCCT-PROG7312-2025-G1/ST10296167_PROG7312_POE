using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using System.ComponentModel;

namespace ST10296167_PROG7312_POE.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly DataStore _dataStore;

        // Constrcutor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ReportService(DataStore dataStore)
        {
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public bool AddIssueAsync(Issue issue)
        {
            try
            {
                issue.ID = _dataStore.GenerateIssueID();
                Console.WriteLine(issue.ID);
                issue.Location = $"{issue.Address}, {issue.Suburb}";
                Console.WriteLine(issue.Location);

                _dataStore.ReportedIssues.Add(issue.ID, issue);
                Console.WriteLine("SAVED ISSUE");
                testStore();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void testStore()
        {
            foreach (var repIssue in _dataStore.ReportedIssues)
            {
                Console.WriteLine($"ID: {repIssue.Value.ID}, Location: {repIssue.Value.Location}, Description: {repIssue.Value.Description}");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
