using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;

    public float attackRange;

    private float timeBtwAttacks;
    public float startTimeBtwAttacks;

    private float stopTime;
    public float startStopTime;

    public float normalSpeed;

    public LayerMask attackMask;
    public Vector2 boxSize;

    private Animator anim;

    private Material matBlink;
    private Material matDefault;

    public int attackDamage;

    private SpriteRenderer spriteRend;

    public Transform point;
    bool moveingRight;

    Transform playerPoint;
    public float stoppingDistance;

    bool angry = false;
    bool goBack = false;
    bool facingRight = true;

    public int health;

    public GameObject deathEffect;
    public Transform enemyPoint;

    public AudioSource audioSource;
    public AudioClip deathSound;

    Rigidbody2D rb;

    public Transform enemyPos;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        playerPoint = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRend = GetComponent<SpriteRenderer>();

        rb = anim.GetComponent<Rigidbody2D>();

        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material;
        matDefault = spriteRend.material;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(playerPoint.position, rb.position ) <= attackRange )
        {
            anim.SetTrigger("Attack");

            angry = false;
            goBack = false;
        }
     
        else if(Vector2.Distance(transform.position, playerPoint.position) < stoppingDistance)
        {
            angry = true;
            goBack = false;
            anim.ResetTrigger("Attack");
        }

        else if (Vector2.Distance(transform.position, playerPoint.position) > stoppingDistance)
        {
            goBack = true;
            angry = false;
            anim.ResetTrigger("Attack");
        }

        if (angry == true)
        {
            anim.SetBool("IsWalking", true);
            Angry();
        }

        if (goBack == true)
        {
            anim.SetBool("IsWalking", true);
            GoBack();
        }

        else
        {
            anim.SetBool("IsWalking", false);
        }

    }

    // Состояния врага
    void Angry()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, playerPoint.position, speed * Time.deltaTime);
        if (facingRight == true && playerPoint.position.x < transform.position.x)
        {
            Flip();
        }
        if (facingRight == false && playerPoint.position.x > transform.position.x)
        {
            Flip();
        }
    }

    void GoBack()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        if (facingRight == false && transform.position.x < point.position.x)
        {
            Flip();
        }
        if (facingRight == true && transform.position.x > point.position.x)
        {
            Flip();
        }
    }

    public void Attack()
    {
        Collider2D colInfo = Physics2D.OverlapBox(transform.position, boxSize,  attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerControler>().TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            audioSource.PlayOneShot(deathSound);
            Die();
        }
    }

    void Die()
    {
        
        Instantiate(deathEffect,enemyPos.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            spriteRend.material = matBlink;

            Invoke("ResetMaterial", .05f);
        }
    }

    void ResetMaterial()
    {
        spriteRend.material = matDefault;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}

