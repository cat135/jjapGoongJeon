using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
	PlayerStat _stat;
	private SkillManager _skillManager; //임시
	bool _stopSkill = false;
	
	public GameObject LockTarget => _lockTarget;

	[SerializeField] private float _speed = 2f;

	public override void Init()
	{
		Managers.Input.KeyAction -= OnKeyEvent;
		Managers.Input.KeyAction += OnKeyEvent;
		Managers.Input.NoKeyAction -= OnNoKeyEvent;
		Managers.Input.NoKeyAction += OnNoKeyEvent;
		
		WorldObjectType = Define.WorldObject.Player;
		_stat = gameObject.GetComponent<PlayerStat>();
		_skillManager = gameObject.GetComponent<SkillManager>();

		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
	}
	
	//일단 임시로
	void Update()
	{
		if (SpawningPool.Spawning.MonsterCount <= 0) return;
		_lockTarget = FindClosestMonster();
	}

	void OnKeyEvent()
	{
		Vector3 newPosition = transform.position;
		
		//transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(Vector3.forward), 0.2f);
		if (Input.GetKey(KeyCode.W))
		{
			newPosition += Vector3.up * (Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			newPosition += Vector3.down * (Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.A))
		{
			newPosition += Vector3.left * (Time.deltaTime * _speed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			newPosition += Vector3.right * (Time.deltaTime * _speed);
		}
		// Clamp the position within the boundary
		newPosition.x = Mathf.Clamp(newPosition.x, MapData.Map.minDeadX, MapData.Map.maxDeadX);
		newPosition.y = Mathf.Clamp(newPosition.y, MapData.Map.minDeadY, MapData.Map.maxDeadY);

		// Apply the clamped position
		transform.position = newPosition;
		
		State = Define.State.Moving;
		
		if (attackCoroutine != null) // 이동 상태로 변경 시 공격 중지
		{
			StopCoroutine(attackCoroutine);
			attackCoroutine = null;
		}
	}

	void OnNoKeyEvent()
	{
		State = Define.State.Idle;
		
		attackCoroutine ??= StartCoroutine(AttackRoutine());
	}
	
	IEnumerator AttackRoutine()
	{
		// 처음 Idle 상태로 들어왔을 때 딜레이
		yield return new WaitForSeconds(_stat.ShootDelay);

		while (State == Define.State.Idle)
		{
			if(SpawningPool.Spawning.MonsterCount>0) PerformAttack();
			yield return new WaitForSeconds(_stat.ShootInterval);
		}
	}

	void PerformAttack()
	{
		_skillManager.MakeActiveSkill();
	}
	
	GameObject FindClosestMonster()
	{
		GameObject closestMonster = null; // 가장 가까운 몬스터
		float minDistance = float.MaxValue; // 초기 최소 거리

		Vector3 characterPosition = transform.position; // 캐릭터 위치

		foreach (var monster in Managers.Game.ActiveMonsters)
		{
			if (monster == null) continue; // 몬스터가 파괴되었거나 null인 경우 무시

			float distance = (characterPosition - monster.transform.position).sqrMagnitude;;

			if (distance < minDistance)
			{
				minDistance = distance;
				closestMonster = monster;
			}
		}

		return closestMonster; // 가장 가까운 몬스터 반환
	}
}
