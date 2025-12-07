using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;
    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;
    
    [Header("Random rotation")]
    [SerializeField] private float minRotation = 0;
    [SerializeField] private float maxRotation = 360;
    
    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -0.3f;
    [SerializeField] private float xMaxOffset = 0.3f;
    [Space]
    [SerializeField] private float yMinOffset = -0.3f;
    [SerializeField] private float yMaxOffset = 0.3f;

    private void Start()
    {
        ApplyRandomOffset(); // 应用随机偏移
        ApplyRandomRotation(); // 应用随机旋转
        
        if(autoDestroy)// 如果启用自动销毁，延迟销毁该物体
            Destroy(gameObject,destroyDelay);
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
