
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAI : MonoBehaviour
{
    public MoveOpportunity[] moveOpportunities;
    public MoveTimer moveTimer;
    public int moveTries = 1;
    public int movesPerOpportunity = 1;
    public int maxMovesPerOpportunity = 1;

    void Start()
    {
        moveTimer.OnCanMove += MoveTimer_OnCanMove;
    }

    private void MoveTimer_OnCanMove()
    {
        MoveRandomly();

        moveTimer.Refresh();

        Debug.Log("AI moved someone.");
    }

    private void MoveRandomly()
    {
        int moves = 0;

        for (int w = 0; w < movesPerOpportunity; w++)
        {
            foreach (MoveOpportunity m in moveOpportunities)
            {
                for (int i = 0; i < moveTries; i++)
                {
                    if (m.chance >= Random.Range(0f, 1f))
                    {
                        m.scheme.MoveRandomly(m.preferRooms);

                        if (++moves >= maxMovesPerOpportunity)
                            return;
                        break;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class MoveOpportunity
{
    public RoomScheme scheme;
    [Range(0f, 1f)]
    public float chance = 0.5f;
    public string[] preferRooms;
}
