using FNAFOnline.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapNodes : MonoBehaviour
{
    public StartingNode[] startingNodes;
    public MapNode[] nodes;

    private Dictionary<MonsterType, MapNode> locations;

    void Start()
    {
        locations = new Dictionary<MonsterType, MapNode>();

        foreach (StartingNode sn in startingNodes)
        {
            MapNode start = GetNode(sn.GetANode());

            Debug.Log("is null? " + (start == null));

            start.ForceMoveHere(sn.forEntity);
            locations.Add(sn.forEntity, start);
        }
    }

    public bool CheckCanMove(MonsterType entity, string to)
    {
        return GetNode(to)?.CheckCanModeHere(locations[entity], entity) ?? false;
    }

    public void ForceMove(MonsterType entity, string to)
    {
        locations[entity].ForceMoveAway(entity);

        MapNode newLocation = GetNode(to);
        newLocation.ForceMoveHere(entity);

        locations[entity] = newLocation; 
    }

    public void SendBack(MonsterType entity)
    {
        ForceMove(entity, GetStartingNode(entity).GetANode());
    }

    public MapNode GetNode(string name)
    {
        return nodes.FirstOrDefault((e) => e.nodeName == name);
    }

    public StartingNode GetStartingNode(MonsterType forEntity)
    {
        return startingNodes.FirstOrDefault((e) => e.forEntity == forEntity);
    }
}

[System.Serializable]
public class MapNode
{
    public string nodeName;
    public List<string> connectedNodes;
    [Range(1, 6)]
    public int maxEntitiesAtOne = 1;
    public MonsterType[] allowedEntities;
    [Space]
    public MapNodeVision optionalVision;

    [HideInInspector]
    internal List<MonsterType> here = new List<MonsterType>();

    public bool CheckCanModeHere(MapNode from, MonsterType entity)
    {
        return from.connectedNodes.Contains(nodeName) && allowedEntities.Contains(entity) && here.Count < maxEntitiesAtOne && !here.Contains(entity);
    }

    public void ForceMoveHere(MonsterType entity)
    {
        here.Add(entity);

        optionalVision?.GetFrame(entity).Set(true);
    }

    public void ForceMoveAway(MonsterType entity)
    {
        here.Remove(entity);

        optionalVision?.GetFrame(entity).Set(false);
    }
}

[System.Serializable]
public class StartingNode
{
    public MonsterType forEntity;
    public string mainStartingNode;
    public string[] alternativeStartingNodes;
    public UnityEvent onMove;

    private bool isFirstTime = true;

    public string GetANode()
    {
        if (isFirstTime)
        {
            isFirstTime = false;

            return mainStartingNode;
        }
        else
        {
            return alternativeStartingNodes[Random.Range(0, alternativeStartingNodes.Length)];
        }
    }
}