using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    // Lưu trữ các node theo vị trí trong lưới
    private Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();
    public Node startNode; // Node bắt đầu
    public Node goalNode; // Node mục tiêu
    public MazeGenerator mazeGenerator;
    public GameObject nodePrefab;
    public float nodeSpacing = 1f;

    // Khởi tạo và tạo các node dựa trên dữ liệu từ MazeGenerator
    public void InitializeNodes()
    {
        ClearNodes(); // Xóa các node trước khi tạo mới

        // Lấy dữ liệu mê cung từ MazeGenerator
        int[,] maze = mazeGenerator.maze;
        int width = mazeGenerator.width;
        int height = mazeGenerator.height;

        Vector3Int startPos = mazeGenerator.start; // Vị trí bắt đầu
        Vector3Int goalPos = mazeGenerator.goal; // Vị trí mục tiêu

        // Lặp qua lưới mê cung và tạo node cho các ô có thể đi qua
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                // Chỉ tạo node trên các ô có thể đi qua (maze[x, y] == 0)
                if (maze[x, y] == 0)
                {
                    Vector3Int gridPos = new Vector3Int(x, y, 0); // Vị trí trong lưới
                    Vector3 worldPos = mazeGenerator.tilemap.CellToWorld(gridPos); // Vị trí trong thế giới 3D

                    // Tạo node và gán vị trí
                    GameObject newNodeObject = Instantiate(nodePrefab, worldPos, Quaternion.identity);
                    Node newNode = newNodeObject.GetComponent<Node>();

                    // Khởi tạo node với vị trí trong lưới
                    newNode.Initialize(gridPos);

                    // Tính toán giá trị heuristic và fScore
                    newNode.hScore = CalculateHeuristic(gridPos); // Tính toán hScore dựa trên khoảng cách đến mục tiêu
                    newNode.CalculateFCost(); // Tính toán fScore

                    // Lưu node vào dictionary
                    nodes.Add(gridPos, newNode);

                    // Gán node bắt đầu và mục tiêu
                    if (gridPos == startPos) startNode = newNode;
                    else if (gridPos == goalPos) goalNode = newNode;
                }
            }
        }

        // Đặt các node lân cận cho từng node
        SetNeighbors();
    }

    // Tính toán heuristic (sử dụng khoảng cách Manhattan)
    private float CalculateHeuristic(Vector3Int gridPos)
    {
        Vector3Int goalPos = mazeGenerator.goal;
        return Mathf.Abs(gridPos.x - goalPos.x) + Mathf.Abs(gridPos.y - goalPos.y);
    }

    // Đặt các node lân cận cho mỗi node
    private void SetNeighbors()
    {
        foreach (var nodeEntry in nodes)
        {
            Node node = nodeEntry.Value;
            Vector3Int gridPos = node.gridPosition;

            // Kiểm tra các vị trí lân cận (trên, dưới, trái, phải)
            Vector3Int[] neighborPositions = {
                new Vector3Int(gridPos.x + 1, gridPos.y, 0), // Phải
                new Vector3Int(gridPos.x - 1, gridPos.y, 0), // Trái
                new Vector3Int(gridPos.x, gridPos.y + 1, 0), // Trên
                new Vector3Int(gridPos.x, gridPos.y - 1, 0)  // Dưới
            };

            foreach (Vector3Int neighborPos in neighborPositions)
            {
                // Nếu vị trí lân cận tồn tại trong danh sách node, thêm vào danh sách neighbors
                if (nodes.ContainsKey(neighborPos))
                {
                    node.AddNeighbor(nodes[neighborPos]);
                }
            }
        }
    }

    // Lấy node dựa trên vị trí trong lưới
    public Node GetNode(Vector3Int gridPos)
    {
        if (nodes.ContainsKey(gridPos))
        {
            return nodes[gridPos];
        }
        return null;
    }

    // Xóa tất cả các node và reset lại dictionary
    public void ClearNodes()
    {
        foreach (Node node in nodes.Values)
        {
            if (node != null) Destroy(node.gameObject); // Hủy game object của node
        }
        nodes.Clear(); // Xóa dữ liệu trong dictionary
    }

    // Reset tất cả các node trong lưới
    public void ResetAllNodes()
    {
        foreach (var node in nodes.Values)
        {
            node.ResetNode(); // Reset từng node
        }
    }

    // Hiển thị giá trị fScore trên tất cả các node
    public void ShowFScoreOnNodes()
    {
        foreach (var nodeEntry in nodes)
        {
            Node node = nodeEntry.Value;
            node.ShowValue();
        }
    }

    // Ẩn giá trị fScore trên tất cả các node
    public void HideFScoreOnNodes()
    {
        foreach (var nodeEntry in nodes)
        {
            Node node = nodeEntry.Value;
            node.HideValue();
        }
    }
}
