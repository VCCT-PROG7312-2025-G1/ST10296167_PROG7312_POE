using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data.DataStructures
{
    public class ReportsGraph
    {
        private Dictionary<int, HashSet<int>> adjacencyList;
        private Dictionary<int, Issue> reportsList;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//

        public ReportsGraph()
        {
            adjacencyList = new Dictionary<int, HashSet<int>>();
            reportsList = new Dictionary<int, Issue>();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Build graph from a collection of reports
        public void Build(IEnumerable<Issue> reports)
        {
            adjacencyList.Clear();
            reportsList.Clear();

            foreach (var report in reports)
            {
                AddReport(report);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Add a new report and create edges based on category and creation date
        public void AddReport(Issue report)
        {
            if (!adjacencyList.ContainsKey(report.ID))
            {
                adjacencyList[report.ID] = new HashSet<int>();
            }

            // Store report
            reportsList[report.ID] = report;

            // Compare with existing reports
            foreach (var other in reportsList.Values)
            {
                if (other.ID == report.ID) continue;

                bool sameCategory = report.Category == other.Category;
                bool closeTime = Math.Abs((report.CreatedAt - other.CreatedAt).TotalDays) <= 3;

                // Create edge if same category and created within 3 days
                if (sameCategory && closeTime)
                {
                    AddEdge(report.ID, other.ID);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Connect two reports in graph
        private void AddEdge(int id1, int id2)
        {
            if (!adjacencyList.ContainsKey(id1))
            {
                adjacencyList[id1] = new HashSet<int>();
            }

            if (!adjacencyList.ContainsKey(id2))
            {
                adjacencyList[id2] = new HashSet<int>();
            }

            adjacencyList[id1].Add(id2);
            adjacencyList[id2].Add(id1);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Update an existing reports stored data
        public void UpdateReport(Issue updatedReport)
        {
            if (reportsList.ContainsKey(updatedReport.ID))
            {
                reportsList[updatedReport.ID] = updatedReport;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Get all reports related to a given report using Breadth-first seach (BFS)
        public List<Issue> GetRelatedReports(int reportId)
        {
            var results = new List<Issue>();

            if (!adjacencyList.ContainsKey(reportId))
            {
                return results;
            }

            var visited = new HashSet<int>();
            var queue = new Queue<int>();

            queue.Enqueue(reportId);
            visited.Add(reportId);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                foreach (var neighbor in adjacencyList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);

                        // Add connected report to results
                        if (reportsList.ContainsKey(neighbor))
                        {
                            results.Add(reportsList[neighbor]);
                        }
                    }
                }
            }

            return results;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // NOT IN ACTIVE USE: kept for legacy reasons
        // Get all groups of connected reports using DFS
        public List<List<Issue>> GetAllConnectedComponents()
        {
            var components = new List<List<Issue>>();
            var visited = new HashSet<int>();

            foreach (var reportId in adjacencyList.Keys)
            {
                if (!visited.Contains(reportId))
                {
                    var cluster = new List<Issue>();
                    DFS(reportId, visited, cluster);
                    components.Add(cluster);
                }
            }

            return components;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Depth-first search to find connected reports
        private void DFS(int current, HashSet<int> visited, List<Issue> cluster)
        {
            visited.Add(current);

            if (reportsList.ContainsKey(current))
            {
                cluster.Add(reportsList[current]);
            }

            foreach (var neighbor in adjacencyList[current])
            {
                if (!visited.Contains(neighbor))
                {
                    DFS(neighbor, visited, cluster);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Get the count of related reports for a specific report
        public int GetRelatedCount(int reportId)
        {
            if (!adjacencyList.ContainsKey(reportId))
                return 0;

            return adjacencyList[reportId].Count;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//