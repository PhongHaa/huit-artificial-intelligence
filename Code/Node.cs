using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public Node cameFrom; // Node mà từ đó node hiện tại đến được
    public List<Node> neighbors = new List<Node>(); // Danh sách các node lân cận

    public float hScore; // Giá trị heuristic: ước tính chi phí từ node hiện tại đến mục tiêu
    public float gScore; // Chi phí từ điểm bắt đầu đến node hiện tại
    public float fScore; // Tổng chi phí: fScore = gScore + hScore

    public bool visited; // Trạng thái đã được thăm hay chưa
    public Vector3Int gridPosition; // Vị trí của node trong lưới
    public Vector3 worldPosition; // Vị trí của node trong thế giới 3D

    private TextMeshPro textMesh; // Hiển thị giá trị fScore trên node
    public TMP_FontAsset fontAsset; // Font chữ cho textMesh

    void Start()
    {
        // Tạo một đối tượng văn bản hiển thị fScore trên node
        GameObject textObject = new GameObject("FScoreValueText");
        textObject.transform.SetParent(this.transform);
        textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.font = fontAsset;
        textMesh.fontSize = 1.1f;
        textMesh.enableAutoSizing = false;
        textMesh.rectTransform.sizeDelta = new Vector2(0.2f, 0.2f);
        textObject.transform.localPosition = Vector3.zero;
    }

    // Khởi tạo node với vị trí trong lưới
    public void Initialize(Vector3Int position)
    {
        gridPosition = position;
        hScore = 0;  // Giá trị heuristic ban đầu bằng 0
        gScore = 0;  // Chi phí từ điểm bắt đầu ban đầu bằng 0
        fScore = 0;  // Tổng chi phí ban đầu bằng 0
        cameFrom = null; // Không có node xuất phát ban đầu
        visited = false; // Node chưa được thăm
    }

    // Tính toán lại fScore khi gScore hoặc hScore thay đổi
    public void CalculateFCost()
    {
        fScore = gScore + hScore;
    }

    // Thêm node lân cận vào danh sách
    public void AddNeighbor(Node neighbor)
    {
        if (!neighbors.Contains(neighbor)) neighbors.Add(neighbor);
    }

    // Đánh dấu node đã được thăm
    public void VisitNode()
    {
        visited = true;
    }

    // Reset node về trạng thái ban đầu
    public void ResetNode()
    {
        cameFrom = null;
        visited = false;
        gScore = 0;
        fScore = 0;
    }

    // So sánh fScore giữa các node
    public int CompareTo(Node other)
    {
        if (other == null) return 1;
        return fScore.CompareTo(other.fScore);
    }

    // Hiển thị giá trị fScore lên node
    public void ShowValue()
    {
        textMesh.text = Mathf.CeilToInt(fScore).ToString();
    }

    // Ẩn giá trị fScore trên node
    public void HideValue()
    {
        textMesh.text = "";
    }
}
