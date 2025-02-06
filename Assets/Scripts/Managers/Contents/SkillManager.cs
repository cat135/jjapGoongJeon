using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //현재 이 플레이어가 업그레이드 해놓은 스킬을 저장
    //임시로 플레이어한테 붙여놓을게
    private GameObject player;
    public ActiveSkill activeSkill;
    void Start()
    {
        player=GameObject.FindWithTag("Player");
        
    }
    
    public void MakeActiveSkill()
    {
        GameObject targetMonster = player.GetComponent<PlayerController>().LockTarget;
        if (targetMonster == null) return;
        
        SkillStatData skillData1 = new SkillStatData
        {
            Name = "Timed Straight Skill",
            Damage = 25f,
            DamageRatio = 1f,
            MissileSpeed = 10.0f,
            MissileSize = 1.0f
        };
        IMovementBehavior movement1 = new StraightMovement(player.transform.position, targetMonster.transform.position);
        IDamageApplicator damageApplicator1 = new TimedDamageApplicator(1.0f);
        IMotherBehavior effect1 = new MotherEffect();
        IOutOfMapBehavior outOfMap1 = new DestroyOutOfMap();
        ICollisionDetector collision1 = new AllObjectsCollisionDetector();
        ISkillTermination termination1 = new TimeBasedTermination(5.0f, this);

        ActiveSkill skill1 = Managers.Resource.Instantiate("ActiveSkills/BlueMissile").GetComponent<ActiveSkill>();
        skill1.Initialize(skillData1, movement1, damageApplicator1, effect1, outOfMap1, collision1, termination1,
            player.GetComponent<Stat>(), targetMonster);
        skill1.transform.position = player.transform.position;
    }
    
    
}
