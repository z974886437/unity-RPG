using System;
using System.Collections;
using UnityEngine;

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    
    [Header("Buff details")]
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

        StartCoroutine(BuffCo(buffDuration));// 启动Buff的协程，持续一段时间后消失
    }

    // Buff效果的协程
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false; // 设置物体不可再次使用
        sr.color = Color.clear;// 设置物体的颜色为透明（模拟Buff激活效果）
        Debug.Log("Buff is applied for : " + duration + " seconds");
        
        yield return new WaitForSeconds(duration); // 等待指定的持续时间
        
        Debug.Log("Buff is removed!");
        
        Destroy(gameObject); // 销毁物体（可能是Buff效果结束后，物体会消失）
    }
}
