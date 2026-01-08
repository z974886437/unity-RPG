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
    Shard_TeleportHpRewind,// When you swap places with shard, your HP % is same as it was when you created shard. 当你用碎片交换位置时，你的生命值百分比和你创建碎片时一样。
    
    /*-----Sword Throw-----*/
    SwordThrow,// You can throw sword to damage enemies from range 你可以投掷剑来远距离伤害敌人
    SwordThrow_Spin,// Your sword will spin at one point and damage enemies,Like a chainsaw 你的剑会在某个点旋转并伤害敌人，就像电锯一样
    SwordThrow_Pierce,//Pierce sword will pierce N targets 穿刺剑能刺穿N个目标
    SwordThrow_Bounce,// Bounce sword will bounce between enemies //弹跳剑会在敌人之间弹跳
    
    /*----- Time Echo ------*/
    TimeEcho, // Create a clone of a player. It can take damage from enemies. 创建一个玩家的克隆体。它能受到敌人的伤害。
    TimeEcho_SingleAttack,//Time Echo can perform a single attack. 时间回声可以施展一次攻击。
    TimeEcho_MultiAttack,// Time Echo can perform N attacks 时间回声可以发动N次攻击
    TimeEcho_ChanceToDuplicate, // Time Echo has a Chance to create another time echo when attacks 时间回声在攻击时有机会制造另一个时间回声
    
    TimeEcho_HealWisp,// When time echo dies it creates a wisp that flies towards the player to heal it. 当时间回声消失时，它会产生一道飞向玩家的光芒以治疗它。
                        //Heal is = to percantage of damage taken when died 治疗值=死亡时所受伤害的感知值
    TimeEcho_CleanseWisp,// Wisp will now remove negative effects from player Wisp现在会移除玩家的负面效果
    TimeEcho_CooldownWisp // Wisp will reduce cooldown of all skills by N second. 幽灵会让所有技能的冷却时间减少N秒。
}
