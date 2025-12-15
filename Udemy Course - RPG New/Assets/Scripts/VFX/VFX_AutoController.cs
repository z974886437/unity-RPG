using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class VFX_AutoController : MonoBehaviour
{
    private SpriteRenderer sr;
    
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;
    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;
    
    [Header("Fade effect")]
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeSpeed =1;
    
    [Header("Random rotation")]
    [SerializeField] private float minRotation = 0;
    [SerializeField] private float maxRotation = 360;
    
    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -0.3f;
    [SerializeField] private float xMaxOffset = 0.3f;
    [Space]
    [SerializeField] private float yMinOffset = -0.3f;
    [SerializeField] private float yMaxOffset = 0.3f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(canFade)
            StartCoroutine(FadeCo());
        
        ApplyRandomOffset(); // 应用随机偏移
        ApplyRandomRotation(); // 应用随机旋转
        
        if(autoDestroy)// 如果启用自动销毁，延迟销毁该物体
            Destroy(gameObject,destroyDelay);
    }

    //淡出
    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;// 设置目标颜色为完全不透明的白色

        while (targetColor.a > 0)// 循环，直到目标颜色的 alpha 值为 0（完全透明）
        {
            targetColor.a = targetColor.a - (fadeSpeed * Time.deltaTime);// 逐渐减少目标颜色的 alpha 值，使图像逐渐变得透明
            sr.color = targetColor;// 更新 SpriteRenderer 的颜色，使其变为目标颜色
            yield return null; // 等待下一帧继续执行，直到 alpha 值逐渐变为 0
        }

        sr.color = targetColor;// 最后一次确保完全透明
    }

    // 应用随机偏移量
    private void ApplyRandomOffset()
    {
        if (randomOffset == false) // 如果没有启用随机偏移，直接返回
            return;

        // 随机生成 x 和 y 偏移量
        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);
        
        transform.position = transform.position + new Vector3(xOffset, yOffset);// 更新物体位置，加上偏移量
    }

    // 应用随机旋转
    private void ApplyRandomRotation()
    {
        if (randomRotation == false) // 如果没有启用随机旋转，直接返回
            return;
        
        float zRotation = Random.Range(minRotation, maxRotation); // 随机生成 z 轴的旋转角度
        transform.Rotate(0,0,zRotation);// 旋转物体
    }
}
