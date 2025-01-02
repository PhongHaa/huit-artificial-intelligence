using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    public int width, height;
    public Tilemap tilemap;

    // Ô tường xung quanh
    public TileBase leftWall, topWall, rightWall;
    public TileBase leftBottomWall, bottomWall, rightBottomWall;
    
    // Nền và tường trong mê cung
    public TileBase wallTile, floorTile;

    // Mảng lưu trữ mê cung
    public int[,] maze;
    public Vector3Int start, goal;

    // Cài đặt nhân vật, quái và rương
    public GameObject playerPrefab, enemyPrefab, chestPrefab;
    private GameObject playerInstance, enemyInstance, chestInstance;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public int minEnemies = 5, maxEnemies = 10;

    // Cài đặt camera theo dõi nhân vật
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);

    // Quản lí các node
    public NodeManager nodeManager;

    // Thuật toán tìm đường đi
    public Pathfinding pathFinding;
    public TileBase pathTile;

    void Start()
    {
        width = Random.Range(10, 31);
        height = Random.Range(10, 31);

        GenerateMaze();                          // Gọi hàm tạo mê cung
        InstantiateMaze();                       // Gọi hàm hiển thị mê cung
        start = FindRandomStartPosition();       // Điểm bắt đầu
        do
        {
            goal = FindRandomGoalPosition();     // Điểm kết thúc
        } while (goal.x == start.x);

        // Khởi tạo các node
        nodeManager.InitializeNodes();

        // Khởi tạo nhân vật, kẻ thù và rương
        if (start != null && goal != null)
        {
            SpawnPlayerAtStart();
            SpawnChest();
        }
        SpawnEnemies();

        // Sử dụng thuật toán tìm đường đi và hiển thị trên mê cung
        Pathfinding();
    }

    void Update()
    {
        if (playerInstance != null && mainCamera != null)
        {
            // Cập nhật vị trí của camera để theo dõi nhân vật
            mainCamera.transform.position = playerInstance.transform.position + cameraOffset;
        }
    }

    void GenerateMaze()
    {
        // Tạo mảng mê cung với biên ngoài
        maze = new int[width + 2, height + 2];
        // Đặt tất cả ban đầu là tường (1)
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                maze[x, y] = 1;
            }
        }
        // Bắt đầu tạo mê cung
        DFS(1, 1);
    }

    void DFS(int x, int y)
    {
        maze[x, y] = 0; // Đánh dấu vị trí hiện tại là đường đi (0)
        
        // Danh sách các hướng đi
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2), new Vector2Int(0, -2),  // Lên & Xuống
            new Vector2Int(2, 0), new Vector2Int(-2, 0)   // Phải & Trái
        };

        directions = Shuffle(directions); // Xáo trộn hướng đi
        
        // Duyệt qua từng hướng
        foreach (var dir in directions)
        {
            int newX = x + dir.x, newY = y + dir.y;
            
            // Kiểm tra nếu vị trí mới còn nằm trong giới hạn và vẫn là tường
            if (IsInBounds(newX, newY) && maze[newX, newY] == 1)
            {
                // Đặt đường đi giữa ô hiện tại và ô mới là đường đi (0)
                maze[x + dir.x / 2, y + dir.y / 2] = 0;
                DFS(newX, newY);
            }
        }
        
        // Kiểm tra nếu vị trí cạnh sát biên chưa được đánh dấu, thì đánh dấu nó
        if (x == width - 1 || y == height - 1)
        {
            if (IsInBounds(x + 1, y)) maze[x + 1, y] = 0;
            if (IsInBounds(x, y + 1)) maze[x, y + 1] = 0;
        }
    }
    
    // Đảm bảo nằm trong phạm vi mê cung, không tính biên ngoài
    public bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width + 1 && y > 0 && y < height + 1;
    }

    List<Vector2Int> Shuffle(List<Vector2Int> list)
    {
        // Xáo trộn danh sách
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    void InstantiateMaze()
    {
        // Hiển thị mê cung và thêm tường, nền theo từng loại
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // Đặt tường đặc biệt cho các góc và biên
                if (x == 0 && y == 0)
                {
                    tilemap.SetTile(tilePosition, leftBottomWall); // Góc dưới trái
                }
                else if (x == width + 1 && y == 0)
                {
                    tilemap.SetTile(tilePosition, rightBottomWall); // Góc dưới phải
                }
                else if (y == 0)
                {
                    tilemap.SetTile(tilePosition, bottomWall); // Tường dưới
                }
                else if (x == 0)
                {
                    tilemap.SetTile(tilePosition, leftWall); // Tường trái
                }
                else if (x == width + 1)
                {
                    tilemap.SetTile(tilePosition, rightWall); // Tường phải
                }
                else if (y == height + 1)
                {
                    tilemap.SetTile(tilePosition, topWall); // Tường trên
                }
                else
                {
                    // Đặt nền và tường bên trong
                    if (maze[x, y] == 1)
                        tilemap.SetTile(tilePosition, wallTile); // Đặt tường
                    else
                        tilemap.SetTile(tilePosition, floorTile); // Đặt nền
                }
            }
        }
    }

    public Vector3Int FindRandomStartPosition()
    {
        List<Vector3Int> floorPositions = new List<Vector3Int>();

        // Duyệt qua toàn bộ mê cung và lưu vị trí nền (floorTile)
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (maze[x, y] == 0) floorPositions.Add(new Vector3Int(x, y, 0));
            }
        }

        // Chọn vị trí ngẫu nhiên làm điểm bắt đầu
        if (floorPositions.Count > 0)
            return floorPositions[Random.Range(0, floorPositions.Count)];
        else
            return new Vector3Int(width - 2, height - 2, 0);
    }

    public Vector3Int FindRandomGoalPosition()
    {
        List<Vector3Int> floorPositions = new List<Vector3Int>();

        // Duyệt qua toàn bộ mê cung và lưu vị trí nền (floorTile)
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (maze[x, y] == 0) floorPositions.Add(new Vector3Int(x, y, 0));
            }
        }

        // Chọn vị trí ngẫu nhiên làm đích
        if (floorPositions.Count > 0)
            return floorPositions[Random.Range(0, floorPositions.Count)];
        else
            return new Vector3Int(width - 2, height - 2, 0);
    }

    public void SpawnPlayerAtStart()
    {
        Vector3Int pos = new Vector3Int(start.x, start.y, 0);
        playerInstance = Instantiate(playerPrefab, tilemap.CellToWorld(pos), Quaternion.identity);
    }

    public void SpawnPlayerAtPosition(Vector3Int position)
    {
        // Hủy nhân vật cũ
        if (playerInstance != null) Destroy(playerInstance);

        // Tạo nhân vật tại vị trí mới
        playerInstance = Instantiate(playerPrefab, tilemap.CellToWorld(position), Quaternion.identity);
    }

    public void SpawnEnemies()
    {
        int size = width * height;

        // Số lượng enemy phụ thuộc vào kích thước mê cung
        int enemyCount = Mathf.Clamp(Mathf.FloorToInt(size * 0.1f), minEnemies, maxEnemies);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3Int randomTilePos = GetRandomFloorTilePosition();
            Vector3 worldPosition = tilemap.CellToWorld(randomTilePos);
            enemyInstance = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
            spawnedEnemies.Add(enemyInstance);
        }
    }

    public void SpawnChest()
    {
        Vector3Int pos = new Vector3Int(goal.x, goal.y, 0);
        chestInstance = Instantiate(chestPrefab, tilemap.CellToWorld(pos), Quaternion.identity);
    }

    Vector3Int GetRandomFloorTilePosition()
    {
        Vector3Int position;
        int attempts = 0;
        do
        {
            // Lấy ngẫu nhiên một vị trí trong mê cung, không chọn vị trí sát biên
            position = new Vector3Int(Random.Range(2, width - 1), Random.Range(2, height - 1), 0);
            attempts++;
            if (attempts > 100) break;
        } while (maze[position.x, position.y] == 1); // Chỉ spawn trên nền (maze[x, y] == 0 là nền)

        return position;
    }

    void Pathfinding()
    {
        nodeManager.startNode = nodeManager.GetNode(start);
        nodeManager.goalNode = nodeManager.GetNode(goal);
        pathFinding.FindPath();
        HighlightPath(pathFinding.path);
    }

    void HighlightPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Vector3Int tilePosition = new Vector3Int(node.gridPosition.x, node.gridPosition.y, 0);
            tilemap.SetTile(tilePosition, pathTile);
        }
    }

    public void RestartLevel()
    {
        // Destroy the current maze
        if (playerInstance != null) Destroy(playerInstance);

        // Destroy chest
        if (chestInstance != null) Destroy(chestInstance);

        // Destroy enemies
        foreach (GameObject enemy in spawnedEnemies) Destroy(enemy);
        spawnedEnemies.Clear();

        // Optionally, you can clear the tilemap
        tilemap.ClearAllTiles();

        // Clear previous nodes
        nodeManager.ClearNodes();

        // Khởi động lại màn chơi mới
        GenerateMaze();
        InstantiateMaze();
        start = FindRandomStartPosition();
        do
        {
            goal = FindRandomGoalPosition();
        } while (goal.x == start.x);
        nodeManager.InitializeNodes();
        SpawnPlayerAtStart();
        SpawnChest();
        SpawnEnemies();
        Pathfinding();
    }
}
