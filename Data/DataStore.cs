using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data
{
    public class DataStore
    {
        private int nextId = 1;
        public Dictionary<int, Issue> ReportedIssues { get; set; } = new Dictionary<int, Issue>();

        public int GenerateIssueID()
        {
            return nextId++;
        }
    }
}
