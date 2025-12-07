using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool needToCalcualte = true;
    private float finalValue;

    // 获取最终属性值的方法
    public float GetValue()
    {
        if (needToCalcualte)// 如果需要重新计算最终值
        {
            finalValue = GetFinalValue();// 调用 GetFinalValue 计算最终属性值
            needToCalcualte = false;// 计算完毕后，设置 needToCalcualte 为 false，避免下次重复计算
        }
        
        return finalValue;// 返回计算后的最终属性值
    }

    // 添加一个新的属性修改器
    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source); // 创建一个新的 StatModifier 对象，保存修改值和来源
        modifiers.Add(modToAdd);// 将修改器添加到修改器列表中
        needToCalcualte = true;// 设置需要重新计算最终值
    }

    // 移除指定来源的所有属性修改器
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source); // 从修改器列表中移除所有来源为指定值的修改器
        needToCalcualte = true; // 设置需要重新计算最终值
    }

    // 计算最终的属性值（包括所有的修改器）
    private float GetFinalValue()
    {
        float finalValue = baseValue; // 初始化最终值为基础值

        foreach (var modifier in modifiers )// 遍历所有修改器，累加它们的值
        {
            finalValue = finalValue + modifier.value; // 将每个修改器的值加到最终值上
        }

        return finalValue;// 返回计算后的最终值
    }

    public void SetBaseValue(float value) => baseValue = value;
    
}

[Serializable]// 使得这个类可以被序列化，可以在 Unity 编辑器或存档中保存数据
public class StatModifier
{
    public float value;// 4// 存储修改值，例如增加的伤害或生命值的数值
    public string source; // buff// 存储修改的来源，如 buff、debuff 或其他来源
    
    // 构造函数，初始化 value 和 source
    public StatModifier(float value, string source)
    {
        this.value = value; // 设置修改的值
        this.source = source;// 设置来源，如 "buff" 或 "debuff"
    }
}
