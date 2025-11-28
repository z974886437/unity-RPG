using System;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
   private Camera mainCamera;//主摄像头
   private float lastCameraPositionX;//最后相机位置 X
   private float cameraHalfWidth;//相机半宽
   
   [SerializeField] private ParallaxLayer[] backgroundLayers;//背景层

   private void Awake()
   {
      mainCamera = Camera.main;// 获取主摄像机（Camera.main 是 Unity 自动赋值的主摄像机）
      cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;// 计算摄像机的半宽度，orthographicSize 是摄像机的视口大小，aspect 是摄像机的长宽比
      CalculateImageLength();// 计算背景图像的长度（初始化时计算）
   }

   // 固定更新方法，每帧固定的时间间隔调用，适用于物理计算
   private void FixedUpdate()
   {
      float currentCameraPositionX = mainCamera.transform.position.x;// 获取当前摄像机的 X 轴位置
      float distanceToMove = currentCameraPositionX - lastCameraPositionX;// 计算相机位置变化的距离（相机的平移距离）
      lastCameraPositionX = currentCameraPositionX;// 更新最后的相机位置，确保下一帧计算正确的偏移量
      
      // 计算相机视野的左边界和右边界
      float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
      float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

      foreach (ParallaxLayer layer in backgroundLayers)// 遍历所有背景层，移动并进行背景循环
      {
         layer.Move(distanceToMove);// 移动当前背景层
         layer.LoopBackground(cameraLeftEdge,cameraRightEdge);// 使背景层循环，确保背景无缝滚动
      }
   }

   // 计算每一层背景的图像宽度
   private void CalculateImageLength()
   {
      foreach (ParallaxLayer layer in backgroundLayers)// 遍历所有背景层，计算每一层的图像宽度
         layer.CalculateImageWidth();
   }
}
