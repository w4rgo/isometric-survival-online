using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimators : MonoBehaviour
{
    [SerializeField] private List<Animator> _animators
        ;


    public void SetFloatToAnimators(String name ,float value)
    {
        foreach (var animator in _animators)
        {
            animator.SetFloat(name,value);
        }
    }
}