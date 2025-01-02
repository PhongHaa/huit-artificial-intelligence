using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 0.8f;          // Tốc độ di chuyển của enemy
    public float detectionRange = 1.5f;     // Khoảng cách phát hiện player
    public int health = 3;                  // Máu của enemy
    private bool isDead = false;            // Kiểm tra trạng thái chết

    private GameObject player;              // Đối tượng player
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 randomDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(MoveRandomly());  // Enemy di chuyển ngẫu nhiên
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
    }


    // Di chuyển ngẫu nhiên
    IEnumerator MoveRandomly()
    {
        while (!isDead)
        {
            randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            // Lật hình ảnh khi di chuyển sang trái hoặc phải
            if (randomDirection.x < 0) spriteRenderer.flipX = true;
            else if (randomDirection.x > 0) spriteRenderer.flipX = false;
            yield return new WaitForSeconds(Random.Range(1f, 3f));  // Đổi hướng sau mỗi 1-3 giây
        }
    }

    void FixedUpdate()
    {
        if (!isDead) rb.MovePosition(rb.position + randomDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // Nhận sát thương
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    // Enemy bị hạ gục
    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        rb.simulated = false;
        Destroy(gameObject, 1f);
    }
}