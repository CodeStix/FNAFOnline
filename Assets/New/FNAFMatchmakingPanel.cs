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

    private float rt;
    private List<FNAFMatchmakingItem> items = new List<FNAFMatchmakingItem>();

    void Start()
    {
        if (refreshTime < 1.5f)
            refreshTime = 1.5f;

        rt = 0.5f;
    }

    void Update()
    {
        rt -= Time.deltaTime;

        if (rt < 0f)
        {
            rt = refreshTime;

            RefreshList();
        }
    }

    public void RefreshList()
    {
        onListRefreshing?.Invoke();
        refreshingObject?.SetActive(true);

        ClearList();

        FNAFClient.Instance.OnMatchmakingResponse += FNAFClient_OnMatchmakingResponse;
        FNAFClient.Instance.RequestMatchmaking();
    }

    private void FNAFClient_OnMatchmakingResponse(object sender, FNAFMatchmakingResponse e)
    {
        FNAFClient.Instance.OnMatchmakingResponse -= FNAFClient_OnMatchmakingResponse;

        foreach (FNAFRoom r in e.rooms)
        {
            AddToList(r);
        }

        onListRefreshed?.Invoke();
        refreshingObject?.SetActive(false);
    }

    public void AddToList(FNAFRoom r)
    {
        var v = Instantiate(roomListItemPrefab, transform);
        v.room = r;
        v.Show();

        items.Add(v);
    }

    void ClearList()
    {
        items.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
