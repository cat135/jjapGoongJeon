using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected float _hp;
    [SerializeField]
    protected float _maxHp;
    [SerializeField]
    protected float _attack;
    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected float _shootInterval;
    [SerializeField]
    protected float _shootDelay;

    public int Level { get { return _level; } set { _level = value; } }
    public float Hp { get { return _hp; } set { _hp = value; } }
    public float MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public float Attack { get { return _attack; } set { _attack = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float ShootInterval { get { return _shootInterval; } set { _shootInterval = value; } }
    public float ShootDelay { get { return _shootDelay; } set { _shootDelay = value; } }


    private void Start()
    {
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _shootDelay = 0.5f;
        _shootDelay = 0.8f;
    }

    public virtual void OnAttacked(ActiveSkill activeSkill)
    {
		float damage = Mathf.Max(0, activeSkill.Damage);
		Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(activeSkill.shooter);
        }   
    }
    
    public virtual void OnAttacked(float Damage)
    {
        float damage = Mathf.Max(0, Damage);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(null);
        }   
    }

    protected virtual void OnDead(Stat attacker)
    {
		PlayerStat playerStat = attacker as PlayerStat;
		if (playerStat != null)
		{
            playerStat.Exp += 2;
		}

        Managers.Game.Despawn(gameObject);
    }
}
