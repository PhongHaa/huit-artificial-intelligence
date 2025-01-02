using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public NodeManager nodeManager; // Quản lý các node
    public MazeGenerator mazeGenerator; // Tham chiếu đến maze generator

    public List<Node> path = new List<Node>(); // Lưu trữ đường đi tìm được

    private Node startNode; // Node bắt đầu của hành trình
    private Node goalNode; // Node mục tiêu của hành trình

    // Tìm đường đi từ startNode đến goalNode
    public void FindPath()
    {
        SetStartAndGoalNodes(); // Lấy node bắt đầu và mục tiêu từ NodeManager
        if (startNode == null || goalNode == null) return; // Nếu không có node hợp lệ, thoát ra

        path.Clear(); // Xóa đường đi trước đó
        nodeManager.ResetAllNodes(); // Reset tất cả các node

        bool pathFound = AStarAlgorithm(); // Thực hiện thuật toán A*
        if (pathFound) HighlightPath(path); // Nếu tìm được đường đi, làm nổi bật đường đi
    }

    // Gán node bắt đầu và mục tiêu từ NodeManager
    private void SetStartAndGoalNodes()
    {
        startNode = nodeManager.startNode;
        goalNode = nodeManager.goalNode;
    }

    // Thuật toán A* để tìm đường đi ngắn nhất
    private bool AStarAlgorithm()
    {
        List<Node> openSet = new List<Node>(); // Danh sách các node cần kiểm tra
        HashSet<Node> closedSet = new HashSet<Node>(); // Danh sách các node đã kiểm tra

        startNode.gScore = 0; // Chi phí từ điểm bắt đầu đến startNode bằng 0
        startNode.CalculateFCost(); // Tính toán fScore của startNode
        openSet.Add(startNode); // Thêm startNode vào openSet

        while (openSet.Count > 0)
        {
            // Sắp xếp openSet theo giá trị fScore, nếu fScore bằng nhau, so sánh thêm hScore
            openSet.Sort((nodeA, nodeB) => {
                int fScoreComparison = nodeA.fScore.CompareTo(nodeB.fScore);
                if (fScoreComparison == 0)
                {
                    return nodeA.hScore.CompareTo(nodeB.hScore); // So sánh hScore nếu fScore bằng nhau
                }
                return fScoreComparison;
            });

            Node currentNode = openSet[0]; // Lấy node có fScore thấp nhất

            // Nếu đã tới goalNode, truy vết lại đường đi
            if (currentNode == goalNode)
            {
                RetracePath(startNode, goalNode); // Truy vết lại đường đi từ goalNode
                return true; // Đường đi được tìm thấy
            }

            openSet.Remove(currentNode); // Loại bỏ currentNode khỏi openSet
            closedSet.Add(currentNode); // Thêm currentNode vào closedSet

            // Duyệt qua các node lân cận của currentNode
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (closedSet.Contains(neighbor)) continue; // Bỏ qua nếu neighbor đã trong closedSet

                float tentativeGScore = currentNode.gScore + 1; // Chi phí tạm tính đến neighbor

                // Cập nhật neighbor nếu có chi phí thấp hơn hoặc chưa nằm trong openSet
                if (tentativeGScore < neighbor.gScore || !openSet.Contains(neighbor))
                {
                    neighbor.cameFrom = currentNode; // Gán đường đi đến neighbor
                    neighbor.gScore = tentativeGScore; // Cập nhật gScore cho neighbor
                    neighbor.CalculateFCost(); // Tính lại fScore cho neighbor

                    if (!openSet.Contains(neighbor)) // Nếu neighbor chưa trong openSet
                    {
                        openSet.Add(neighbor); // Thêm neighbor vào openSet
                    }
                }
            }
        }

        return false; // Không tìm thấy đường đi
    }

    // Truy vết lại đường đi từ goalNode đến startNode
    private void RetracePath(Node startNode, Node endNode)
    {
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode); // Thêm node vào danh sách đường đi
            currentNode = currentNode.cameFrom; // Di chuyển về node trước đó
        }
        path.Reverse(); // Đảo ngược đường đi để có thứ tự từ start đến goal
    }

    // Làm nổi bật đường đi đã tìm được
    private void HighlightPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            node.VisitNode(); // Đánh dấu các node trong đường đi đã được thăm
        }
    }
}
