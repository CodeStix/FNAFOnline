using Stx.Net.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFMatchmakingPanel : MonoBehaviour
{
    [Range(1.5f, 25f)]
    public float refreshTime;
    [Header("UI Items")]
    public FNAFMatchmakingItem roomListItemPrefab;
    public GameObject refreshingObject;
    [Header("Events")]
    public UnityVoidEvent onListRefreshing;
    public UnityVoidEvent onListRefreshed;
    public UnityVoidEvent onJoinFailed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
