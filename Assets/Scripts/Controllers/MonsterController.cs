using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;

    [SerializeField]
    private GameObject _player;
    
    [SerializeField]
    private Define.MoveType _moveType;

    private Vector2 targetPosition;     // 현재 목표 위치

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        _stat.ShootInterval = 5f; // 다음 Idle 상태를 위한 초기화
        _stat.ShootDelay = 1f;
        State = Define.State.Moving; // 초기 상태를 Moving으로 설정
        
        _player = GameObject.FindWithTag("Player");

        if (_moveType == Define.MoveType.FastStalk)
            _stat.MoveSpeed = 1.6f;
        else _stat.MoveSpeed = 1f;

        SetNewTargetPosition();
    }

    void Update()
    {
        switch (State)
        {
            case Define.State.Moving:
                switch (_moveType)
                {
                    case Define.MoveType.SlowStalk:
                        UpdateMovingState();
                        break;
                    case Define.MoveType.FastStalk:
                        UpdateMovingState();
                        break;
                    case Define.MoveType.Wonder:
                        TempWonderState();
                        break;
                }
                
                break;
            case Define.State.Idle:
                UpdateIdleState();
                break;
        }
    }
    
    void SetNewTargetPosition()
    {
        // 현재 위치를 기준으로 무작위 목표 위치 설정
        Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(0, 3f);
        targetPosition = (Vector2)transform.position + randomDirection;
    }

    private void TempWonderState()
    {
        _stat.ShootDelay -= Time.deltaTime;
        if (_stat.ShootDelay <= 0)
        {
            SetNewTargetPosition();
            State = Define.State.Idle;
            _stat.ShootDelay = 5f; // 다음 Moving을 위한 초기화
        }

        transform.position =
            Vector3.MoveTowards(transform.position, _player.transform.position, _stat.MoveSpeed * Time.deltaTime);
        
    }
    
    private void UpdateMovingState()
    {
        _stat.ShootDelay -= Time.deltaTime;
        if (_stat.ShootDelay <= 0)
        {
            State = Define.State.Idle;
            _stat.ShootDelay = 5f; // 다음 Moving을 위한 초기화
        }

        transform.position =
            Vector3.MoveTowards(transform.position, _player.transform.position, _stat.MoveSpeed * Time.deltaTime);
    }

    private void UpdateIdleState()
    {
        
        // Idle 상태 관련 로직 (예시로 일정 시간 후 Attack 상태로 전환)
        _stat.ShootInterval -= Time.deltaTime;
        if (_stat.ShootInterval <= 0)
        {
            State = Define.State.Moving;
            _stat.ShootInterval = 1f; // 다음 Idle 상태를 위한 초기화
            
            // Idle 상태에서 공격을 포함한 로직
            PerformAttack();
        }
    }
    
    public void PerformAttack()
    {
        SkillStatData skillData1 = new SkillStatData
        {
            Name = "Timed Straight Skill",
            Damage = 25f,
            DamageRatio = 1f,
            MissileSpeed = 5.0f,
            MissileSize = 1.0f
        };
        IMovementBehavior movement1 = new StraightMovement(gameObject.transform.position, _player.transform.position);
        IDamageApplicator damageApplicator1 = new TimedDamageApplicator(1.0f);
        IMotherBehavior effect1 = new MotherEffect();
        IOutOfMapBehavior outOfMap1 = new DestroyOutOfMap();
        ICollisionDetector collision1 = new AllObjectsCollisionDetector();
        ISkillTermination termination1 = new TimeBasedTermination(5.0f, this);

        ActiveSkill skill1 = Managers.Resource.Instantiate("ActiveSkills/OrangeMissile").GetComponent<ActiveSkill>();
        skill1.Initialize(skillData1, movement1, damageApplicator1, effect1, outOfMap1, collision1, termination1,
            _stat, _player);
        skill1.transform.position = gameObject.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            other.GetComponent<Stat>().OnAttacked(_stat.Attack);
        }
    }
}
