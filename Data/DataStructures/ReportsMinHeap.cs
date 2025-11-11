using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data.DataStructures
{
    public class ReportsMinHeap
    {
        private List<Issue> heap;

        public ReportsMinHeap()
        {
            heap = new List<Issue>();
        }

        public int Count => heap.Count;

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Build heap from a list of issues
        public void Build(List<Issue> issues)
        {
            heap.Clear();

            foreach (var issue in issues)
            {
                Insert(issue);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Add a new issue to the heap and maintain heap property
        public void Insert(Issue report)
        {
            heap.Add(report);
            BubbleUp(heap.Count - 1);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Remove and return the highest priority issue 
        public Issue? ExtractMin()
        {
            if (heap.Count == 0)
            {
                return null;
            }

            Issue min = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                PushDown(0);
            }

            return min;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Return highest priority issue without removing it
        public Issue? Peek()
        {
            return heap.Count > 0 ? heap[0] : null;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Get all issues in priority order without modifying heap
        public List<Issue> ExtractAllSorted()
        {
            var sorted = new List<Issue>();

            while (heap.Count > 0)
            {
                sorted.Add(ExtractMin()!);
            }

            return sorted;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Bubble up a node to maintain heap property
        private void BubbleUp(int index)
        {
            if (index == 0)
                return;

            int parentIndex = (index - 1) / 2;

            if (ComparePriority(heap[index], heap[parentIndex]) < 0)
            {
                Swap(index, parentIndex);
                BubbleUp(parentIndex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Heapify down a node to maintain heap property
        private void PushDown(int index)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild < heap.Count && ComparePriority(heap[leftChild], heap[smallest]) < 0)
            {
                smallest = leftChild;
            }

            if (rightChild < heap.Count && ComparePriority(heap[rightChild], heap[smallest]) < 0)
            {
                smallest = rightChild;
            }

            if (smallest != index)
            {
                Swap(index, smallest);
                PushDown(smallest);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Compare two issues to determine priority based on status and creation date
        private int ComparePriority(Issue issue1, Issue issue2)
        {
            // Primary - lower status value = higher priority
            int statusComparison = issue1.Status.CompareTo(issue2.Status);

            if (statusComparison != 0)
            {
                return statusComparison;
            }

            // Secondary - newer issues first = higher priority
            return issue2.CreatedAt.CompareTo(issue1.CreatedAt);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Swap two nodes in the heap
        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        public bool IsEmpty()
        {
            return heap.Count == 0;
        }

        public void Clear()
        {
            heap.Clear();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//