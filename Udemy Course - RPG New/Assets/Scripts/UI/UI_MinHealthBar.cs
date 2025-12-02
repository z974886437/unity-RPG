using System;
using UnityEngine;

public class UI_MinHealthBar : MonoBehaviour
{
    
    private Entity entity ;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnFlipped += HandleFlip;// 订阅 OnFlipped 事件，当触发该事件时调用 HandleFlip 方法
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;// 取消订阅 OnFlipped 事件
    }

    // 事件触发时执行的处理方法
    private void HandleFlip() => transform.rotation = Quaternion.identity;// 将物体的旋转角度重置为初始状态（无旋转）
    
}
