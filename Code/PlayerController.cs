using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Enemy's properties
    public float moveSpeed = 0.8f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // Attack
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    public GameObject healthText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal"); // Move left/right
        movementInput.y = Input.GetAxisRaw("Vertical");   // Move up/down

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        // Set IsMoving parameter
        anim.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Update movement direction for animation
            anim.SetFloat("MoveX", movementInput.x);
            anim.SetFloat("MoveY", movementInput.y);
        }

        // Check if it's time to attack
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Attack using Space key
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;

                // Set attack direction based on movement input
                if (movementInput.y > 0.5f) // Attack upward
                {
                    anim.SetFloat("MoveY", 1);
                    anim.SetFloat("MoveX", 0);
                }
                else if (movementInput.y < -0.5f) // Attack downward
                {
                    anim.SetFloat("MoveY", -1);
                    anim.SetFloat("MoveX", 0);
                }
                else if (Mathf.Abs(movementInput.x) > 0) // Attack horizontally
                {
                    anim.SetFloat("MoveX", movementInput.x);
                    anim.SetFloat("MoveY", 0);
                    spriteRenderer.flipX = movementInput.x < 0; // Flip for left attack
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Move character based on input
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.deltaTime);

        // Flip sprite based on movement direction
        if (movementInput.x < 0)
            spriteRenderer.flipX = true;  // Lật sang trái
        else if (movementInput.x > 0)
            spriteRenderer.flipX = false; // Lật sang phải
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        Vector2 attackDirection = Vector2.zero;
        if (anim.GetFloat("MoveY") > 0) attackDirection = Vector2.up;
        else if (anim.GetFloat("MoveY") < 0) attackDirection = Vector2.down;
        else if (anim.GetFloat("MoveX") > 0) attackDirection = Vector2.right;
        else if (anim.GetFloat("MoveX") < 0) attackDirection = Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, attackDirection, attackRange, enemyLayers);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                    ShowDamageNumber(hit.collider.transform.position, 1);
                }
            }
        }
    }

    void ShowDamageNumber(Vector3 position, int damageAmount)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        RectTransform textTransform = Instantiate(healthText, canvas.transform).GetComponent<RectTransform>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        textTransform.position = screenPosition;
        HealthText healthTextComponent = textTransform.GetComponent<HealthText>();
        healthTextComponent.textMesh.text = damageAmount.ToString();
    }
}
