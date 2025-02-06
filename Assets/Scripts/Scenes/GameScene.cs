using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        
        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "Player");
        
        GameObject go = new GameObject { name = "@SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        pool.SetKeepMonsterCount(8);
    }

    public override void Clear()
    {
        
    }
}
