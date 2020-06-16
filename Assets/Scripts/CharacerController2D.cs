using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacerController2D : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed = 5f;
    
    [SerializeField] private float jumpSpeed = 5f;

    private Collision2D _groundDetect;
    
    private bool _isGrounded = false;

    private bool _facingRight = true;

    private Animator _anim;

    private bool _gameStarted;
    private Rigidbody2D _playerRb;

    private float _input;

    private bool _wantToJump = false;

    private InputMaster _inputMaster;
    private void OnDisable()
    {
        _inputMaster.Disable();
    }

    private void Awake()
    {
        EnableInput();
        _inputMaster.Player.Move.performed += ctx => Move(ctx.ReadValue<float>());
        _inputMaster.Player.Jump.performed += ctx => Jump(ctx.ReadValue<float>());
    }

    private void EnableInput()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        _playerRb = GetComponent<Rigidbody2D>();
        _gameStarted = false;
        _anim = GetComponent<Animator>();
    }

    void Move(float direction)
    {
        if (direction > 0.5f)
        {
            _input = 1;
        } else if (direction < -0.5f)
        {
            _input = -1;
        }
        else
        {
            _input = 0;
        }
    }

    void Jump(float jump)
    {
        if (jump > 0 && _isGrounded)
        {
            _wantToJump = true;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_input > 0)
        {
            _gameStarted = true;
            if (!_facingRight)
            {
                Flip();
            }
        } else if (_input < 0)
        {
            _gameStarted = true;
            if (_facingRight)
            {
                Flip();
            }
        }
    }
    
    private void FixedUpdate()
    {
        
        _isGrounded = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 1.25f);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        
        Vector2 raycast = new Vector2();

        if (_facingRight)
        {
            raycast = Vector2.right;
        }
        else
        {
            raycast = Vector2.left;
        }

        Vector2 traPos = transform.position;
        Vector2 rayStart = new Vector2(traPos.x, traPos.y - 0.6f);

        hits = Physics2D.RaycastAll(rayStart, raycast, 1f);
        Debug.DrawRay(rayStart, raycast, Color.cyan);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        
        HandleMovement();
    }

    private void HandleMovement()
    {
        float jump = 0f;
        if (_wantToJump && _isGrounded)
        {
            jump = 1.0f * (jumpSpeed * Time.deltaTime);
            _wantToJump = false;
        }

        float h = _input * (movementSpeed * Time.deltaTime);
        transform.Translate(new Vector3(h, 0));
        
        if (_isGrounded)
        {
            if (jump > 0f)
            {
                _playerRb.AddForce(new Vector2(0, (jumpSpeed * Time.fixedDeltaTime)), ForceMode2D.Impulse);
                _anim.SetBool("isNotOnGround", true);
                _anim.SetBool("isRunning", false);
            } else if (h != 0f)
            {
                _anim.SetBool("isNotOnGround", false);
                _anim.SetBool("isRunning", true);
            }
            else
            {
                _anim.SetBool("isNotOnGround", false);
                _anim.SetBool("isRunning", false);
            }
        }
    }
    
    void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool GameStarted => _gameStarted;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }
}
