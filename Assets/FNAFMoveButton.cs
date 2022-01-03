using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class FNAFMoveButton : MonoBehaviour
{
    public string monsterName;
    public int locationIndex;
    public FNAFMoveButton[] enableWhenAt;
    public FNAFMoveButton[] disableWhenAt;
    public FNAFOffice1 controller;

    private bool isHere = false;
    private Button button;

    public bool IsHere => isHere;

    private void Start()
    {
        //isHere = startLocation;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (button.enabled && button.interactable) 
        {
            FNAFClient.Instance.FNAF1RequestMove(monsterName, locationIndex, 0);

            //FNAFMoveButton neighbor = null;
            //for(int i = 0; i < enableWhenAt.Length; i++)
            //{
            //    if (enableWhenAt[i].isHere)
            //    {
            //        neighbor = enableWhenAt[i];
            //        break;
            //    }
            //}

            //if (neighbor != null)
            //{
            //    neighbor.isHere = false;
            //    isHere = true;
            //}
        }
    }

    public void SetIsHere(bool isHere)
    {
        this.isHere = isHere;
    }

    private void Update()
    {
        bool mayMoveHere = false;
        if (!isHere)
        {
            for(int i = 0; i < enableWhenAt.Length; i++)
            {
                FNAFMoveButton n = enableWhenAt[i];
                if (n.isHere)
                {
                    mayMoveHere = true;
                    break;
                }
            }
        }

        bool canMoveHere = true;
        if (mayMoveHere)
        {
            for (int j = 0; j < disableWhenAt.Length; j++)
            {
                FNAFMoveButton dn = disableWhenAt[j];
                if (dn.isHere)
                {
                    canMoveHere = false;
                    break;
                }
            }
        }

        if (mayMoveHere)
        {
            button.image.enabled = true;
            if (canMoveHere && controller.MayMove)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
        else
        {
            button.image.enabled = false;
        }
    }



}
