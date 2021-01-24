﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_Controller : MonoBehaviour, IEntityController
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private GameObject target;
    private bool _inRange;

    private Hammer_Model _model;
    private Hammer_View _view;

    private Rigidbody2D _rd2D;

    [SerializeField]
    private GameObject[] hitbox;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;


    // Parameters:
    [SerializeField]
    private bool _changeDirection;

    private bool _isAttacking = false;
    private bool _canUseAttack = true;
    private bool _canDash = true;
    private bool _isDash = false;

    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }
    private int AttackNumber;




    private void Awake()
    {
        _view = this.GetComponent<Hammer_View>();
        _model = this.GetComponent<Hammer_Model>();
        _rd2D = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
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
        if (_inRange) CheckAttack();
    }



    //Attack
    private void CheckAttack()
    {
        if (!_isAttacking && _canUseAttack)
        {
            AttackNumber = Random.Range(0, 3);
            AttackStart();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Player")
            && !collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
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

    private void AttackStart()
    {

        _isAttacking = true;
        _canUseAttack = false;

        Invoke(nameof(CanUseAttack), 3f);
        Invoke(nameof(AttackStop), 1.1f);  

        foreach (var h in hitbox)
        {
            h.GetComponent<BoxCollider2D>().enabled = true;

        }
        //TOChange
        _model.Speed *= 3;        
    }

    private void AttackStop()
    {
        _model.Speed /= 3;
        _isAttacking = false;
        foreach (var h in hitbox)
        {
            h.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    //Check and Change direction
    private void ChangeMoveDirection(bool behind = false)
    {
        if (IsDead || _isAttacking) return;

        if (behind)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        if (!_isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }
        else if (_isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }
    }

    //Coroutine for Movement
    IEnumerator ChangeDirectionCorutine()
    {
        _model.Speed = -_model.Speed;
        this.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        _changeDirection = true;
    }

    public void TakeDamage(int dmg)
    {
        //check distance
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
        {
            ChangeMoveDirection(true);
        }


        if (!_canDash)
        {
            _model.HP -= dmg;
            if (_model.HP <= 0)
            {
                _model.HP = 0;
                _model.Speed = 0;

                IsDead = true;
                Invoke(nameof(DestroyMe), 1.0f);
            }
            else
            {
                IsHurting = true;
                Invoke(nameof(StopHurting), 0.2f);
            }
        }
        else
        {
            Dash();
        }

    }

    private void Dash()
    {
        _canDash = false;
        _isDash = true;

        gameObject.layer = 30;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 0;
        _model.Speed *= 10;

        Invoke(nameof(StopDashing), 0.3f);
        //Delay Dash
        Invoke(nameof(CanDash), 6f);
    }
    private void StopDashing()
    {
        _isDash = false;
        gameObject.layer = 0;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 5;
        _model.Speed /= 10;
    }



    private void DestroyMe() { Destroy(gameObject); }
    private void StopHurting() { IsHurting = false; }
    private void CanUseAttack() { _canUseAttack = true; }
    private void CanDash() { _canDash = true; }


    private void Animate()
    {
        if (IsDead) _view.Die();
        else if (IsHurting) _view.TakeDamage();
        else if (_isDash) _view.Dash();
        else if (_isAttacking && !_canUseAttack)
        {
            if (AttackNumber == 0) _view.Attack();
            else if (AttackNumber == 1) _view.Attack2();
            else _view.AttackSpinner();
        }
        else _view.Walk();
    }



    public int ReturnCurrentHP() { return _model.HP; }
    public int ReturnMaxHP() { return _model.MaxHP; }
}
