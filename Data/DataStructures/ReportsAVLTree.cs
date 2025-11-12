using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data.DataStructures
{
    public class Node
    {
        public Node? LeftNode { get; set; }
        public Node? RightNode { get; set; }
        public int Height { get; set; }
        public Issue Data { get; set; }

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//

        public Node(Issue report)
        {
            Data = report;
            LeftNode = null;
            RightNode = null;
            Height = 1;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }

    public class ReportsAVLTree
    {
        private Node? root;

        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Build AVL Tree from a collection of reports
        public void Build(IEnumerable<Issue> reports)
        {
            root = null;

            foreach (var report in reports)
            {
                Insert(report);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Get height of a node
        private int Height(Node? node)
        {
            if (node != null)
            {
                return node.Height;
            }
            else
            {
                return 0;
            }
        }

        // Get balance factor of a node
        private int GetBalance(Node? node)
        {
            if (node == null)
            {
                return 0;
            }
            else
            {
                int leftHeight = Height(node.LeftNode);
                int rightHeight = Height(node.RightNode);

                return leftHeight - rightHeight;
            }
        }

        // Right rotate subtree 
        private Node RotateRight(Node y)
        {
            Node x = y.LeftNode!;
            Node T2 = x.RightNode!;

            // Rotate
            x.RightNode = y;
            y.LeftNode = T2;

            // Update heights
            y.Height = Math.Max(Height(y.LeftNode), Height(y.RightNode)) + 1;
            x.Height = Math.Max(Height(x.LeftNode), Height(x.RightNode)) + 1;

            return x;
        }

        // Left rotate subtree
        private Node RotateLeft(Node x)
        {
            Node y = x.RightNode!;
            Node T2 = y.LeftNode!;

            // Rotate
            y.LeftNode = x;
            x.RightNode = T2;

            // Update heights
            x.Height = Math.Max(Height(x.LeftNode), Height(x.RightNode)) + 1;
            y.Height = Math.Max(Height(y.LeftNode), Height(y.RightNode)) + 1;

            return y;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Insert a report into AVL tree and rebalance
        public void Insert(Issue report)
        {
            root = InsertRec(root, report);
        }

        private Node InsertRec(Node? node, Issue report)
        {
            if (node == null)
            {
                node = new Node(report);
                return node;
            }

            if (report.ID < node.Data.ID)
            {
                node.LeftNode = InsertRec(node.LeftNode, report);
            }
            else if (report.ID > node.Data.ID)
            {
                node.RightNode = InsertRec(node.RightNode, report);
            }
            else
            {
                return node;
            }

            node.Height = 1 + Math.Max(Height(node.LeftNode), Height(node.RightNode));

            int balance = GetBalance(node);

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

            return node;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Search for a report in AVL tree by report ID
        public Issue? Search(int reportId)
        {
            return SearchRec(root, reportId);
        }

        private Issue? SearchRec(Node? node, int reportId)
        {
            if (node == null)
            {
                return null;
            }

            if (reportId == node.Data.ID)
            {
                return node.Data;
            }
            else if (reportId < node.Data.ID)
            {
                return SearchRec(node.LeftNode, reportId);
            }
            else
            {
                return SearchRec(node.RightNode, reportId);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // In-order traversal of AVL tree, returning a sorted list of reports by ID
        public List<Issue> InOrder()
        {
            var result = new List<Issue>();
            InOrderRec(root, result);

            return result;
        }

        private void InOrderRec(Node? node, List<Issue> reports)
        {
            if (node != null)
            {
                InOrderRec(node.LeftNode, reports);
                reports.Add(node.Data);
                InOrderRec(node.RightNode, reports);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Update a report node in AVL tree
        public void Update(Issue report)
        {
            root = DeleteRec(root, report.ID);
            Insert(report);
        }

        // Delete a report node from AVL tree by report ID and rebalance
        private Node? DeleteRec(Node? node, int id)
        {
            if (node == null)
            {
                return null;
            }

            if (id < node.Data.ID)
            {
                node.LeftNode = DeleteRec(node.LeftNode, id);
            }
            else if (id > node.Data.ID)
            {
                node.RightNode = DeleteRec(node.RightNode, id);
            }
            else
            {
                if (node.LeftNode == null)
                {
                    return node.RightNode;
                }
                else if (node.RightNode == null)
                {
                    return node.LeftNode;
                }

                // Get smallest in the right subtree
                node.Data = MinValue(node.RightNode);
                node.RightNode = DeleteRec(node.RightNode, node.Data.ID);
            }

            if (node == null)
            {
                return node;
            }

            // Update height and rebalance
            node.Height = 1 + Math.Max(Height(node.LeftNode), Height(node.RightNode));
            int balance = GetBalance(node);

            if (balance > 1 && GetBalance(node.LeftNode) >= 0)
            {
                return RotateRight(node);
            }

            if (balance > 1 && GetBalance(node.LeftNode) < 0)
            {
                node.LeftNode = RotateLeft(node.LeftNode!);
                return RotateRight(node);
            }

            if (balance < -1 && GetBalance(node.RightNode) <= 0)
            {
                return RotateLeft(node);
            }

            if (balance < -1 && GetBalance(node.RightNode) > 0)
            {
                node.RightNode = RotateRight(node.RightNode!);
                return RotateLeft(node);
            }

            return node;
        }

        // Get smallest node in subtree
        private Issue MinValue(Node node)
        {
            Issue minValue = node.Data;

            while (node.LeftNode != null)
            {
                minValue = node.LeftNode.Data;
                node = node.LeftNode;
            }

            return minValue;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Count number of nodes in AVL tree
        public int Count()
        {
            return CountNodesRec(root);
        }

        private int CountNodesRec(Node? node)
        {
            if (node == null)
            {
                return 0;
            }

            return 1 + CountNodesRec(node.LeftNode) + CountNodesRec(node.RightNode);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//