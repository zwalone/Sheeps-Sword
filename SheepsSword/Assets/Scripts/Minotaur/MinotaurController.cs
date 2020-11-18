﻿using System.Collections;
using UnityEngine;

public class MinotaurController : MonoBehaviour, IEntityController
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private GameObject target;
    private bool _inRange;

    private MinotaurModel _model;
    private MinotaurView _view;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;

    private Rigidbody2D _rd2D;

    [SerializeField]
    private GameObject hitbox;

    //Parameters:
    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;
    bool isHurting;
    bool isDead;

    private void Awake()
    {
        _view = this.GetComponent<MinotaurView>();
        _model = this.GetComponent<MinotaurModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _view.WalkRight();
        _changeDirection = true;
    }

    private void FixedUpdate()
    {
        //Move Enemy and check direction
        _rd2D.MovePosition(_rd2D.position + new Vector2(_model.Speed, 0) * Time.fixedDeltaTime);
        ChangeMoveDirection();
    }

    private void Update()
    {
        Animate();

        if (_inRange)
        {
            CheckAttack();
        }

    }
    
    //Attack
    private void CheckAttack()
    {
        if(!_isAttacking )
        {
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            _inRange = true;
            target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _inRange = false;
            target = null;
        }
    }

    //Check and Change direction
    private void ChangeMoveDirection()
    {

        if (!_isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }else if(_isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }
    }

    public void TakeDamage(int dmg)
    {
        _model.HP -= dmg;
        if (_model.HP <= 0)
        {
            _model.HP = 0;
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(TakeDamage());
        }
    }

    IEnumerator Die()
    {
        _model.Speed = 0;
        _view.DieRight();
        isDead = true;

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    //Coroutine for Movement
    IEnumerator ChangeDirectionCorutine()
    {
        _model.Speed = -_model.Speed;
        this.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        _changeDirection = true;
    }

    IEnumerator Attack()
    {
        hitbox.GetComponent<BoxCollider2D>().enabled = true;
        _isAttacking = true;
        _view.AttackRight();

        _model.Speed *= 6;

        yield return new WaitForSeconds(1.1f);

        _model.Speed /= 6;
        _view.WalkRight();
        _isAttacking = false;
        hitbox.GetComponent<BoxCollider2D>().enabled = true;
    }

    IEnumerator TakeDamage()
    {
        _view.GetDamage();
        isHurting = true;

        yield return new WaitForSeconds(0.3f);

        isHurting = false;
       _view.WalkRight();
    }

    private void Animate()
    {
        if (isHurting) _view.GetDamage();
        else if (isDead) _view.DieRight();
        else if (_isAttacking) _view.AttackRight();
        else _view.WalkRight();
    }
}
