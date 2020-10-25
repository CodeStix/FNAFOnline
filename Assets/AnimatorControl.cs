using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
[RequireComponent(typeof(Animator))]
public class AnimatorControl : MonoBehaviour
{
    private Animator a;

    void Start ()
    {
        a = GetComponent<Animator>();
	}
	
	public void BoolTrue(string param)
    {
        a.SetBool(param, true);
    }

    public void BoolFalse(string param)
    {
        a.SetBool(param, false);
    }
}
