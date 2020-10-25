using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfNightDetermine : MonoBehaviour
{
    public GameObject winObject;
    public GameObject loseObject;
    public bool securityWins = false;

    void Start()
    {
        if (NightSetup.isGuard == securityWins)
        {
            winObject.SetActive(true);
            loseObject.SetActive(false);
        }
        else
        {
            winObject.SetActive(false);
            loseObject.SetActive(true);
        }
    }

}
