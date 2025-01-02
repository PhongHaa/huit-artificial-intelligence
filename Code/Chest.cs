using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite openChestSprite;
    private SpriteRenderer spriteRenderer;
    private bool isOpen = false; // Kiểm tra trạng thái rương
    private bool playerInRange = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true; // Nhân vật ở trong phạm vị mở rương
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false; // Nhân vật không còn ở trong phạm vi
        }
    }

    void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.X))
        {
            OpenChest(); // Khi người chơi ấn nút X, rương được mở
        }
    }

    void OpenChest()
    {
        isOpen = true; // Đánh dấu rương đã mở
        spriteRenderer.sprite = openChestSprite;
        Debug.Log("Chest Opened! Level Completed!");
        StartCoroutine(CompleteLevel());
    }

    IEnumerator CompleteLevel()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<MazeGenerator>().RestartLevel();
    }
}
