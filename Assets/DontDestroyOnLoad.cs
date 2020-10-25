using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class DontDestroyOnLoad : MonoBehaviour
{
	void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
