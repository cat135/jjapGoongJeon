using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    private static SpawningPool s_instance;

    public static SpawningPool Spawning
    {
        get
        {
            if (s_instance != null) return s_instance;

            var go = GameObject.Find("@SpawningPool");
            if (go == null)
            {
                go = new GameObject { name = "@SpawningPool" };
                go.transform.position = Vector3.zero;
                go.AddComponent<SpawningPool>();
            }

            s_instance = go.GetComponent<SpawningPool>();

            return s_instance;
        }
    }
    
    [SerializeField]
    int _monsterCount = 0;
    int _reserveCount = 0;

    public int MonsterCount => _monsterCount;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    Vector3 _spawnPos;
    [SerializeField]
    float _spawnRadius = 8.0f;
    [SerializeField]
    float _spawnTime = 5.0f;
    
    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        int temp = Random.Range(0, 3);
        GameObject obj = null;
        switch (temp)
        {
            case 0:
                obj = Managers.Game.Spawn(Define.WorldObject.Monster, "BlueKnight");
                break;
            case 1:
                obj = Managers.Game.Spawn(Define.WorldObject.Monster, "YellowKnight");
                break;
            case 2:
                obj = Managers.Game.Spawn(Define.WorldObject.Monster, "RedKnight");
                break;
        }
         
        
        Vector3 randPos = _spawnPos + Random.insideUnitSphere * Random.Range(0, _spawnRadius);

        if (obj != null) obj.transform.position = randPos;
        _reserveCount--;
    }
}
