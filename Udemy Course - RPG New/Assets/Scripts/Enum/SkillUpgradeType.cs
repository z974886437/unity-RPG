using UnityEngine;

public enum SkillUpgradeType 
{
    None,
    
    
    /* -------Dash Tree------*/
    Dash,
    Dash_CloneOnStart,
    Dash_CloneOnStartAndArrival,
    Dash_ShardOnShart,
    Dash_ShardOnStartAndArrival,
    
    
    /*----- Shard Tree -----*/
    Shard ,
    Shard_MoveToEnemy,
    Shard_TripleCast,
    Shard_Teleport,
    Shard_TeleportAndHeal
}
