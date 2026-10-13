using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviour
{

    public GameManager gameManager;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;


    AudioSource audioSource;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    Animator animator;

    public InputAction moveAction;
    public InputAction jumpAction;


    CapsuleCollider2D capsuleCollider;


    Vector2 moveInput;

    public float moveSpeed = 5.0f;
    public float maxMoveSpeed = 5.0f;

    public float JumpPower = 5.0f;
    bool bJumpInput = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        audioSource = GetComponent<AudioSource>();

        moveAction.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        jumpAction.Enable();
        jumpAction.performed += OnJump;
       
    }


    void PlayAudioClip(string action)
    {

        switch(action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;

        }
        audioSource.Play();

    }


    // Update is called once per frame
    void Update()
    {


        //Animation 
        if( Mathf.Abs( rb.linearVelocityX) <  0.3f)
        {
            animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWalking", true);
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;


        


        //Stop 
        if (moveInput.x == 0)
        {
            rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * 0.5f;
        }else  if(moveInput.x  == 1.0f)
        {

            spriteRenderer.flipX = false;

        }
        else if(moveInput.x == -1.0f)
        {
            spriteRenderer.flipX = true;
        }


    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //이미 접프한상태 
        if (animator.GetBool("IsJumping"))
            return;

        bJumpInput = context.ReadValueAsButton();

        if (bJumpInput)
        {
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
            PlayAudioClip("JUMP");
        }
    }

    private void FixedUpdate()
    {

        // move player by input
        //rb.linearVelocity = new Vector2(
        //moveInput.x * moveSpeed,
        //rb.linearVelocity.y);
        //Jump

    

        //Move
       rb.AddForce(Vector2.right * moveInput.x, ForceMode2D.Impulse);
        
        //limit the max speed of ther player 
        if (rb.linearVelocityX > maxMoveSpeed)
            rb.linearVelocityX = maxMoveSpeed;
        else if (rb.linearVelocityX < -maxMoveSpeed)
            rb.linearVelocityX = -maxMoveSpeed;


        //땅 착지할때  
        if (rb.linearVelocityY < 0.0f && bJumpInput)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1.0f, LayerMask.GetMask("Platform"));

            Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0));


            if (hit.collider != null)
            {
                if (hit.distance < 0.5f)
                {

                        animator.SetBool("IsJumping", false);
                        bJumpInput = false;
                }
            }
        }


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Enemy")
        {


            if (rb.linearVelocityY < 0 && transform.position.y > collision.transform.position.y)
            {
                //Attack 
                OnAttack(collision.transform);


            }
            else
            {


                Debug.Log("플레이어가 맞았다.");
                OnDamaged(collision.transform.position);
            }
        }

    }

    void OnAttack(Transform enemyTransform)
    {

        gameManager.stagePoint += 100;

        rb.AddForce(Vector2.up * 10.0f, ForceMode2D.Impulse);

        PlayAudioClip("ATTACK");
        EnemyMove enemyMove= enemyTransform.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector3 targetPos)
    {
        //health 
        gameManager.HealthDown();


        
        //Change layer;(무적 판정 레이어 )
        gameObject.layer = 9;


        //view alpha 
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);


        int dir = (transform.position.x - targetPos.x) > 0 ? 1 : -1;
        //reaction force;
        rb.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);


        //Animation
        animator.SetTrigger("doDamaged");

        PlayAudioClip("DAMAGED");

        Invoke("OffDamaged", 2.0f);
    }
    
    //무적해제
    void OffDamaged()
    {

        gameObject.layer = 8;//player

        spriteRenderer.color = new Color(1, 1, 1, 1);


    }

    public void OnDie()
    {

        //Sprite Alpha;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        capsuleCollider.enabled = false;

        //Die effect Jump 
        rb.AddForce(Vector2.up * 5.0f, ForceMode2D.Impulse);

        PlayAudioClip("DIE");
        //Destroy
        Invoke("Destroy", 5);

    }

        


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Item")
        {
            //item 획득 

            //Point 
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                gameManager.stagePoint += 50;

            } else if (isSilver)
            {
                gameManager.stagePoint += 100;
            } else if (isGold)
            {

                gameManager.stagePoint += 300;
             }

            PlayAudioClip("ITEM");
            collision.gameObject.SetActive(false);

        }else if(collision.gameObject.tag =="Finish")
        {
            PlayAudioClip("FINISH");
            gameManager.NextStage();

        }

    }

    public void VelocityZero()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
