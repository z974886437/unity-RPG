using System;
using UnityEngine;

public class Enltity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }
}
