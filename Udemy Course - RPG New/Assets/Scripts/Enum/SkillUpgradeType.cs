using UnityEngine;

public enum SkillUpgradeType 
{
    None,
    
    
    /* -------Dash Tree------*/
    Dash,// Dash to avoid damage 冲刺以避免受伤
    Dash_CloneOnStart,// Create a clone when dash starts 冲刺开始时创建克隆
    Dash_CloneOnStartAndArrival,// Create a clone when dash starts and ends 当冲刺开始和结束时创建一个克隆
    Dash_ShardOnShart,// Create a shard when dash starts 冲刺开始时创建碎片
    Dash_ShardOnStartAndArrival,// Create a shard when dash starts and ends 冲刺开始和结束时创建碎片
    
    
    /*----- Shard Tree -----*/
    Shard ,// The shard explodes when touched by an enemy or time goes up 碎片被敌人触碰或时间延长时会爆炸
    Shard_MoveToEnemy,// Shard will move towards nearest enemy 碎片会朝最近的敌人移动
    Shard_Multicast,// Shard ability can have up to N charges. You can cast them all in a raw 碎片能力最多可以有N次充能。你可以在RAW里全部施放
    Shard_Teleport,// You can swap places with the last shard you created 你可以和你最后创建的碎片交换位置
    Shard_TeleportHpRewind// When you swap places with shard, your HP % is same as it was when you created shard. 当你用碎片交换位置时，你的生命值百分比和你创建碎片时一样。
}
