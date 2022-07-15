using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private LevelManager levelManager;
    private InputManager inputManager;
    private Animator animator;

    private Rigidbody rb;
    private float maxVelocity = 12.5f;
    private float sqrMaxVelocity;
    private Vector3 playerVelocity;

    [Header("Slide Settings")]
    private Vector3 hitPointNormal;
    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;
    [SerializeField] private float slopeLimit = 30;
    [SerializeField] private float moveSpeed = 20;
    private float speed = 20f;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    [HideInInspector] public bool death;
    [HideInInspector] public bool slide;
    private bool startAnim, canFly;
    private bool isSliding
    {
        get
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 0.5f))
            {
                hitPointNormal = slopeHit.normal;
                slide = true;
                return Vector3.Angle(hitPointNormal, Vector3.up) > slopeLimit;
            }
            else
            {
                slide = false;
                return false;
            }
        }
    }

    private void Awake()
    {
        SetMaxVelocity(maxVelocity);
        rb = GetComponent<Rigidbody>();
        gameManager = GameManager.Instance;
        levelManager = LevelManager.Instance;
        inputManager = InputManager.Instance;
        animator = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        if(Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            moveSpeed += speedIncreaseAmount;
            levelManager.amount = moveSpeed - speed;
        }

        moveDirection = transform.right * inputManager.direction.x;
        moveDirection.z += Time.deltaTime * 5;
    }


    private void FixedUpdate()
    {
        if (!levelManager.isGameOn || death)
        {
            rb.useGravity = false;
            return;
        }

        if (!startAnim)
        {
            startAnim = true;
            animator.SetBool("Start", true);
        }


        var v = rb.velocity;
        if (v.sqrMagnitude > sqrMaxVelocity)
        {
            rb.velocity = v.normalized * maxVelocity;
        }

        if (isSliding)
        {
            animator.SetBool("Start", false);
            animator.SetBool("Sliding", true);
            animator.SetBool("Jump", false);
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, hitPointNormal).normalized;
            rb.MovePosition(transform.position + new Vector3(0, 0, slopeMoveDirection.z) * Time.deltaTime * moveSpeed);
            playerVelocity = rb.velocity;
            playerVelocity.x += slopeMoveDirection.x * 10;
            playerVelocity.y += slopeMoveDirection.y * 15;
            rb.velocity = playerVelocity;

            if (rb.velocity.y > -0.2f)
            {
                Vector3 gravity = rb.velocity;
                gravity.y -= 9.81f;
                rb.velocity = gravity;
            }

            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1))
            {
                if (hit.normal.x > 0)
                {
                    transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 60f, 0);
                }
                else
                {
                    transform.GetChild(0).transform.rotation = Quaternion.Euler(0, -60f, 0);
                }
            }

            canFly = true;
        }
        else
        {
           if(canFly)
            {
                canFly = false;
                Vector3 clear = rb.velocity;
                clear.x = 0;
                rb.velocity = clear;
            }
            animator.SetBool("Sliding", false);
            animator.SetBool("Jump", true);
            Vector3 goingForward = moveDirection;
            goingForward.z *= 5f;
            rb.MovePosition(transform.position + goingForward * Time.deltaTime * moveSpeed);
        }
        rb.useGravity = !isSliding;
    }


    private void SetMaxVelocity(float maxVelocity)
    {
        this.maxVelocity = maxVelocity;
        sqrMaxVelocity = maxVelocity * maxVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Death")
        {
            transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetTrigger("Death");
            death = true;
            if(LevelManager.Instance.score > GameManager.Instance.record)
                gameManager.record = levelManager.score;

            gameManager.SaveData();
            StartCoroutine(dieRespawn());
        }
        else if (other.transform.tag == "Ring")
        {
            other.GetComponentInChildren<ParticleSystem>().Play();
            gameManager.currency += 20;
        }
        else if(other.transform.tag == "Coin")
        {
            other.GetComponent<Animator>().enabled = true;
            gameManager.currency += 1;
        }
    }

    IEnumerator dieRespawn()
    {
        yield return new WaitForSeconds(5f);
        levelManager.Respawn();
    }
}
