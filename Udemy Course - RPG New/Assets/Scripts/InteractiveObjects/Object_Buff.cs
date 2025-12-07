using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType type;
    public float value;
    
}

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;
    
    [Header("Buff details")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4;
    [SerializeField] private bool canBeUsed = true;
    
    [Header("Floaty movement")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosistion;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosistion = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;// 计算基于时间和浮动速度的y轴偏移值
        transform.position = startPosistion + new Vector3(0, yOffset); // 沿Y轴做正弦浮动运动
    }

    // 触发器进入时调用的方法
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeUsed == false) // 如果当前物体不能被使用，直接返回
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));// 启动Buff的协程，持续一段时间后消失
    }

    // Buff效果的协程
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false; // 设置物体不可再次使用
        sr.color = Color.clear;// 设置物体的颜色为透明（模拟Buff激活效果）

        ApplyBuff(true);
        
        yield return new WaitForSeconds(duration); // 等待指定的持续时间

        ApplyBuff(false);
        
        Destroy(gameObject); // 销毁物体（可能是Buff效果结束后，物体会消失）
    }

    // 应用或移除增益效果（Buff）
    private void ApplyBuff(bool apply)
    {
        // 遍历所有的 Buff
        foreach (var buff in buffs)
        {
            if(apply)// 如果是应用 Buff
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value,buffName);// 获取对应的 Stat 并添加修改器（增加 Buff 的效果）
            else
                // 如果是移除 Buff
                // 获取对应的 Stat 并移除指定来源的修改器（移除 Buff 的效果）
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
        }
    }
}
