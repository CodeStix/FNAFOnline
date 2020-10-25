using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;

public class PrizeCollection : MonoBehaviour
{
    public Prize[] prizeCollection;
    public GameObject prizeItemPrefab;
    [Space]
    public bool enableStats = true;
    [Header("UI")]
    public Image imageBorder;
    public Image image;
    public Text textInfo;
    public Text textValue;
    [Space]
    public string coinRewardStat = "stat";
    [Space]
    public string jsonDataFile = "prizesGranted.json";
    public RectTransform newPrizesPanel;
    public Text newPrizesText;
    public UnityEvent onNewPrize;
    //[Space]
    //public Sprite lockedSprite;

    private List<PrizeItem> prizeItems = new List<PrizeItem>();
    private JConfig<JPrize> prizes;

	void Start()
    {
        foreach(Prize p in prizeCollection)
        {
            GameObject obj = Instantiate(prizeItemPrefab, transform);
            PrizeItem item = obj.GetComponent<PrizeItem>();
            item.prize = p;
            obj.SetActive(true);
        }

        string filePath = Path.Combine(Application.persistentDataPath, jsonDataFile);

        prizes = new JConfig<JPrize>(filePath);

        for (int i = 0; i < transform.childCount; i++)
        {
            PrizeItem item = transform.GetChild(i).GetComponent<PrizeItem>();
            if (item != null)
            {
                if (enableStats)
                {
                    item.unlocked = item.prize.Granted();

                    bool b = prizes.GetBool(item.prize.name);

                    if (item.unlocked && !b)
                    {
                        prizes.Put(item.prize.name, true);

                        GameObject obj = new GameObject(item.prize.name);
                        Image im = obj.AddComponent<Image>();
                        im.sprite = item.prize.image;
                        im.preserveAspect = true;
                        im.transform.SetParent(newPrizesPanel);
                        im.transform.localScale = Vector2.one;

                        newPrizesText.text += " - " + item.prize.name;

                        Stats.j.Increment(coinRewardStat, item.prize.value);

                        if (onNewPrize != null)
                            onNewPrize.Invoke();

                        Debug.LogWarning("NEW PRIZE: " + item.prize.name);
                    }

                }
                prizeItems.Add(item);
            }
        }

        newPrizesText.text += " - ";
       /* if (newPrizesText.text.EndsWith(", "))
            newPrizesText.text = newPrizesText.text.Substring(newPrizesText.text.Length - 2);*/
    }

    /*void Update()
    {
        if (textTotalValue != null)
        {
            int value = 0;
            foreach(PrizeItem item in prizeItems)
            {
                value += item.unlocked ? item.prize.value : 0;
            }
            textTotalValue.text = "Fcoins: " + value;
        }
    }*/

    public void Display(PrizeItem item)
    {
        item.prize.ShowToUI(image, imageBorder, textInfo, textValue, item.unlocked);
    }
}
[System.Serializable]
public class JPrize : IKeyValuePair<string,string>
{
    public string name;
    public string value;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Value
    {
        get { return value; }
        set { this.value = value; }
    }
}