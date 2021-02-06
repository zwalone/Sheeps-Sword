﻿using UnityEngine;

public class Hammer_Model : MonoBehaviour
{
    [SerializeField]
    private int _maxHP;
    public int MaxHP
    {
        get { return _maxHP; }
        set { _maxHP = value; }
    }

    [SerializeField]
    private int _hp;
    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    [SerializeField]
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
}