using FNAFOnline.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapNodeVision : MonoBehaviour
{
    public MapNodeFrame[] frames;

    public void SetPresent(MonsterType entity, bool present)
    {
        MapNodeFrame frame = GetFrame(entity);

        frame.Set(present);
    }

    public MapNodeFrame GetFrame(MonsterType forEntity)
    {
        return frames.FirstOrDefault((e) => e.forEntity == forEntity);
    }
}

[System.Serializable]
public class MapNodeFrame
{
    public MonsterType forEntity;
    public GameObject[] possibleVisions;
    public UnityEvent onMoveHere;
    public UnityEvent onLeave;

    public void Set(bool enable)
    {
        if (possibleVisions.Length > 0)
        {
            foreach (GameObject sr in possibleVisions)
                sr.SetActive(false);

            if (enable)
            {
                if (possibleVisions.Length == 1)
                {
                    possibleVisions[0].SetActive(true);
                }
                else
                {
                    possibleVisions[Random.Range(0, possibleVisions.Length)].SetActive(true);
                }
            }
        }

        if (enable)
            onMoveHere?.Invoke();
        else
            onLeave?.Invoke();
    }
}
