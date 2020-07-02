using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacerController2D : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    
    [SerializeField] private float _jumpHeight = 20f;

    private bool _isGrounded, _jumpPressed, _moveAllowed, _gameStarted, _facingRight;

    private float _input;

    private Rigidbody2D _playerRb;
    
    private InputMaster _inputMaster;

    private Animator _anim;

    public bool GameStarted => _gameStarted;

    private void Awake()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
        _playerRb = GetComponent<Rigidbody2D>();
        _gameStarted = false;
        _facingRight = true;
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _moveAllowed = true;
        _input = _inputMaster.Player.Move.ReadValue<float>();
        _isGrounded = false;
        
        
        

        Vector3 playerPos = transform.position;
        Vector3 rayPos = new Vector3(playerPos.x - 0.3f, playerPos.y - 0.5f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayPos, Vector2.down, 1.25f);
        Debug.DrawRay(rayPos, Vector2.down);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _isGrounded = true;
                break;
            }
        }

        if (!_isGrounded)
        {
            rayPos = new Vector3(playerPos.x + 0.3f, playerPos.y - 0.5f);
            hits = Physics2D.RaycastAll(rayPos, Vector2.down, 1.25f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    _isGrounded = true;
                    break;
                }
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

        Vector3 test = transform.position;
        test.y = test.y - 0.6f;
        
        hits = Physics2D.RaycastAll(test, raycast, 0.5f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _moveAllowed = false;
            }
        }

        if (_moveAllowed)
        {
            Transform pTransform = transform;
            Vector2 position = pTransform.position;
            transform.Translate(Vector2.right * (_input * (Time.deltaTime * _movementSpeed)));    
        }
        
        if (_inputMaster.Player.Jump.triggered)
        {
            _jumpPressed = true;
        }
        
        if (_input > 0)
        {
            _gameStarted = true;
            if (!_facingRight)
            {
                Flip();
            }
            if (_isGrounded)
                _anim.SetBool("isRunning", true);
            else 
                _anim.SetBool("isRunning", false);
        } else if (_input < 0)
        {
            _gameStarted = true;
            if (_facingRight)
            {
                Flip();
            }
            if (_isGrounded)
                _anim.SetBool("isRunning", true);
            else
                _anim.SetBool("isRunning", false);
        } else
            _anim.SetBool("isRunning", false);
        
        if (!_isGrounded)
            _anim.SetBool("isNotOnGround", true);
        else 
            _anim.SetBool("isNotOnGround", false);
    }

    private void FixedUpdate()
    {
        if (_jumpPressed)
        {
            _jumpPressed = !_jumpPressed;
    
            if (_isGrounded)
            {
                _playerRb.AddForce(new Vector2(0, (_jumpHeight * Time.fixedDeltaTime)), ForceMode2D.Impulse);
                _isGrounded = false;
            }
        }
    }
    
    void Flip()
    {
        _facingRight = !_facingRight;
        Transform pTransform = transform;
        Vector3 scale = pTransform.localScale;
        scale.x *= -1;
        pTransform.localScale = scale;
    }
}
