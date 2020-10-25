using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigateMenu : MonoBehaviour
{
    public string menuInAnimation;
    public string menuOutAnimation;
    public Animation[] menuItems;
    public UnityEvent onFirst;
    public UnityEvent onNotFirst;

    private int currentIndex = 0;

    void Start()
    {
        menuItems[0].Play(menuInAnimation);
    }

    public void Goto(int menuIndex)
    {
        if (menuIndex == currentIndex || menuIndex >= menuItems.Length)
            return;

        menuItems[currentIndex].Play(menuOutAnimation);
        menuItems[menuIndex].Play(menuInAnimation);

        currentIndex = menuIndex;

        if (currentIndex == 0)
            onFirst?.Invoke();
        else
            onNotFirst?.Invoke();
    }

    public void First()
    {
        Goto(0);
    }

    public void Last()
    {
        Goto(menuItems.Length - 1);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    /*public int startupMenuItemIndex;
    public List<GameObject> menuItems = new List<GameObject>();

    private int previousMenuItemIndex = -1;

    void Start()
    {

        Goto(startupMenuItemIndex);
    }

    public void Goto(int menuItemIndex)
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            GameObject go = menuItems[i];

            Animator m = go.GetComponent<Animator>();

            if (m != null)
            {
                if (i == menuItemIndex)
                {
                    go.SetActive(true);

                    if (i != previousMenuItemIndex)
                        m.SetBool("Show", true);
                }
                else
                {
                    if (i == previousMenuItemIndex)
                        m.SetBool("Show", false);

                    StartCoroutine(SetActiveLater(go, false, 0.5f));
                }
            }
            else
            {
                go.SetActive(i == menuItemIndex);
            }
        }

        previousMenuItemIndex = menuItemIndex;
    }

    private IEnumerator SetActiveLater(GameObject obj, bool active, float time)
    {
        yield return new WaitForSeconds(time);

        obj.SetActive(active);
    }*/
}
