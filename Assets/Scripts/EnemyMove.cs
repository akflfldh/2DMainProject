using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    Rigidbody2D rb;

    Animator animator;

    SpriteRenderer spriteRenderer;
    public int nextMove = 1;

    CircleCollider2D collider;


    public float minMoveThinkTime = 2.0f;
    public float maxMoveThinkTime = 5.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        collider = GetComponent<CircleCollider2D>();
        Think();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }


    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(nextMove, rb.linearVelocityY);

        Vector2 front = new Vector2(rb.position.x + nextMove * 0.2f, rb.position.y);

        RaycastHit2D hit = Physics2D.Raycast(front, Vector2.down, 0.5f,LayerMask.GetMask("Platform"));
        if(hit.collider ==null)
        {
            Turn();
        }

    }

    void Think()
    {

        //Set Next Active 
        nextMove = Random.Range(-1, 2);


        animator.SetInteger("WalkSpeed", nextMove);

        FlipSpriteX();


        float thinkTime = Random.Range(minMoveThinkTime, maxMoveThinkTime);

        Invoke("Think", thinkTime);
    }

    //방향 전환 
    void Turn()
    {
        nextMove *= -1;
        FlipSpriteX();
        CancelInvoke("Think");
        Invoke("Think", 5);
    }

    void FlipSpriteX()
    {
        //방향 sprite 뒤집기
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }
    

    public void OnDamaged()
    {
        //Sprite Alpha;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        collider.enabled = false;

        //Die effect Jump 
        rb.AddForce(Vector2.up * 5.0f, ForceMode2D.Impulse);

        //Destroy
        Invoke("Destroy", 5);



    }
}
