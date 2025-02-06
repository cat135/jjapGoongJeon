using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
	protected int _exp;
    

	public int Exp 
	{ 
		get { return _exp; } 
		set 
		{ 
			_exp = value;

			int level = 1;
			while (true)
			{
				Data.Stat stat;
				if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
					break;
				if (_exp < stat.totalExp)
					break;
				level++;
			}

			if (level != Level)
			{
				Debug.Log("Level Up!");
				Level = level;
				SetStat(Level);
			}
		}
	}


	private void Start()
	{
		_level = 1;
		_exp = 0;
		_moveSpeed = 5.0f;
		_shootDelay = 0.5f;
		_shootInterval = 0.8f;
		
		SetStat(_level);
	}

	public void SetStat(int level)
	{
		Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
		Data.Stat stat = dict[level];
		_attack = stat.attack;
	}

	protected override void OnDead(Stat attacker)
	{
		Debug.Log("Player Dead");
	}
}
