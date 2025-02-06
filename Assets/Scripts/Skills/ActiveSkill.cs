using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct SkillStatData
{
    public string Name;
    // public int Hp;
    // public int MaxHp;
    // public int Penetrate;
    public float DamageRatio;
    public float Damage;
    // public float ShootDelay;
    // public float ShootInteval;
    public float MissileSpeed;
    public float MissileSize;
}

public class ActiveSkill : MonoBehaviour
{
    protected SkillStatData skillData = new SkillStatData();
    public Stat shooter;
    public GameObject targetMonster;
    [SerializeField] private int _targetTag=7;
    
    #region 스탯

    public string Name
    {
        get { return skillData.Name; }
        set { skillData.Name = value; }
    }
    
    public float DamageRatio
    {
        get { return skillData.DamageRatio; }
        set { skillData.DamageRatio =value; }
    }
    
    public float Damage
    {
        get { return skillData.Damage; }
        set { skillData.Damage = value; }
    }
    
    public float MissileSpeed
    {
        get { return skillData.MissileSpeed; }
        set { skillData.MissileSpeed = value; }
    }

    public float MissileSize
    {
        get { return skillData.MissileSize; }
        set { skillData.MissileSize = Mathf.Clamp(value, 0.1f, 5f); }
    }

    #endregion

    private IMovementBehavior movementBehavior;
    private IMotherBehavior effectBehavior;
    private IOutOfMapBehavior outOfMapBehavior;
    private IDamageApplicator damageApplicator;
    private ICollisionDetector collisionDetector;
    private ISkillTermination skillTermination;

    public void Initialize(SkillStatData data, IMovementBehavior move, IDamageApplicator damageApplicator,
        IMotherBehavior effect,
        IOutOfMapBehavior outOfMap, ICollisionDetector collisionDetector, ISkillTermination skillTermination,
        Stat shooter, GameObject targetMonster)
    {
        this.skillData = data;
        this.movementBehavior = move;
        this.effectBehavior = effect;
        this.outOfMapBehavior = outOfMap;
        this.damageApplicator = damageApplicator;
        this.collisionDetector = collisionDetector;
        this.skillTermination = skillTermination;
        this.shooter = shooter;
        this.targetMonster = targetMonster;
    }

    void Update()
    {
        PerformMovement();
        CheckOutOfMap();
        // collisionDetector?.DetectCollision();
        // damageApplicator?.ApplyDamage();
        // if (skillTermination != null && skillTermination.ShouldTerminate())
        // {
        //     Terminate();
        // }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _targetTag)
        {
            other.GetComponent<Stat>().OnAttacked(this);
            Managers.Resource.Destroy(gameObject);
        }
    }

    void PerformMovement()
    {
        movementBehavior.Move(skillData, gameObject, targetMonster);
    }

    void ApplyEffect()
    {
        effectBehavior.ApplyEffect();
    }

    void CheckOutOfMap()
    {
        outOfMapBehavior.OutOfMap(gameObject);
    }

    // void DetectCollision(Collider2D other)
    // {
    //     collisionDetector.DetectCollision(other);
    // }

    private void Terminate()
    {
        Debug.Log("Skill terminated");
        Managers.Resource.Destroy(gameObject);
    }
}

#region 인터페이스

// 움직이는 방식 인터페이스
public interface IMovementBehavior
{
    void Move(SkillStatData skillData, GameObject gameObject, GameObject closestMonster);
}

// 데미지를 주는 방식 인터페이스
public interface IDamageApplicator
{
    void ApplyDamage(SkillStatData skillData);
}

// 확정 효과 인터페이스
public interface IMotherBehavior
{
    void ApplyEffect();
}

// 맵밖으로 나갈 때 처리 인터페이스
public interface IOutOfMapBehavior
{
    void OutOfMap(GameObject gameObject);
}

// 충돌 감지 인터페이스
public interface ICollisionDetector
{
    void DetectCollision();
}

// 스킬 종료 조건 인터페이스
public interface ISkillTermination
{
    bool ShouldTerminate();
}

#endregion

#region 움직임

public class StraightMovement : IMovementBehavior
{
    private Vector3 direction; // 타겟 방향 벡터 저장

    // 생성자에서 초기화할 때 방향 벡터를 설정
    public StraightMovement(Vector3 startPosition, Vector3 targetPosition)
    {
        // 목표 위치 - 현재 위치로 방향 벡터 계산 후 정규화(normalized)
        direction = (targetPosition - startPosition).normalized;
    }
    public void Move(SkillStatData skillData, GameObject gameObject, GameObject closestMonster)
    {
        //일자로 계속 따라가게
        gameObject.transform.Translate(direction * (skillData.MissileSpeed * Time.deltaTime));
    }
}

public class FollowMovement : IMovementBehavior
{
    public void Move(SkillStatData skillData, GameObject gameObject, GameObject closestMonster)
    {
        //일자로 가되 약간 유도
    }
}

public class StaticMovement : IMovementBehavior
{
    public void Move(SkillStatData skillData, GameObject gameObject, GameObject closestMonster)
    {
        //얘는 움직이지 않음.
    }
}

public class LaserMovement : IMovementBehavior
{
    public void Move(SkillStatData skillData, GameObject gameObject, GameObject closestMonster)
    {
        //해당방향으로 
    }
}

#endregion

#region Mother/Infant

public class MotherEffect : IMotherBehavior
{
    public void ApplyEffect()
    {
        Debug.Log("Applies Mother effect to enhance nearby allies.");
    }
}

public class InfantEffect : IMotherBehavior
{
    public void ApplyEffect()
    {
        Debug.Log("Applies Infant effect to receive benefits from a Mother.");
    }
}

#endregion

#region OutOfMap

public class DestroyOutOfMap : IOutOfMapBehavior
{
    public void OutOfMap(GameObject gameObject)
    {
        if (!MapData.Map.CheckIfOutOfMap(gameObject.transform.position))
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}

public class CutOffOutOfMap : IOutOfMapBehavior
{
    public void OutOfMap(GameObject gameObject)
    {
        Debug.Log("The object is cut off from active gameplay when it leaves the map.");
    }
}

public class IgnoreOutOfMap : IOutOfMapBehavior
{
    public void OutOfMap(GameObject gameObject)
    {
        Debug.Log("The object ignores map boundaries and continues unaffected.");
    }
}
#endregion

#region Damage Applicators

public class TimedDamageApplicator : IDamageApplicator
{
    private ActiveSkill context;
    private float interval;

    public TimedDamageApplicator(float interval)
    {
        this.interval = interval;
    }

    public void ApplyDamage(SkillStatData skillData)
    {
        context.StartCoroutine(ApplyDamageCoroutine());
    }

    private IEnumerator ApplyDamageCoroutine()
    {
        while (true)
        {
            Debug.Log("Damage applied!");
            yield return new WaitForSeconds(interval);
        }
    }
}

public class OnCollisionDamageApplicator : IDamageApplicator
{
    public void ApplyDamage(SkillStatData skillData)
    {
        Debug.Log("Damage applied on collision!");
    }
}

#endregion

#region Collision Detectors

public class SpecificObjectCollisionDetector : ICollisionDetector
{
    public void DetectCollision()
    {
        Debug.Log("Specific object collision detected!");
    }
}

public class AllObjectsCollisionDetector : ICollisionDetector
{
    public void DetectCollision()
    {
        Debug.Log("All objects collision detected!");
    }
}

#endregion

#region Skill Termination Conditions

public class TimeBasedTermination : ISkillTermination
{
    private float duration;
    private float startTime;
    private MonoBehaviour context;

    public TimeBasedTermination(float duration, MonoBehaviour context)
    {
        
    }

    private IEnumerator TerminateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Skill terminated due to time limit");
        
    }

    public bool ShouldTerminate()
    {
        return false; // 실제 종료는 Coroutine으로 처리되므로 항상 false 반환
    }
}

public class HitCountBasedTermination : ISkillTermination
{
    private int maxHits;
    private int currentHits;

    public HitCountBasedTermination(int maxHits)
    {
        this.maxHits = maxHits;
        this.currentHits = 0;
    }

    public bool ShouldTerminate()
    {
        return currentHits >= maxHits;
    }

    public void RegisterHit()
    {
        currentHits++;
    }
}

#endregion
