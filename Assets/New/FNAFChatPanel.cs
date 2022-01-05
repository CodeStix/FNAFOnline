using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FNAFChatPanel : MonoBehaviour
{
    public FNAFChatMessage messagePrefab;
    public InputField inputField;
    public Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(Send);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnChatEvent += Instance_OnChatEvent;
    }

    private void OnDisable()
    {
        FNAFClient.Instance.OnChatEvent -= Instance_OnChatEvent;
    }

    private void Instance_OnChatEvent(object sender, FNAFChatEvent e)
    {
        string senderName = FNAFClient.Instance.GetRoom().users.First((u) => u.user.id == e.senderId).user.name;

        FNAFChatMessage message = Instantiate(messagePrefab, transform);
        message.transform.SetSiblingIndex(0);
        message.authorText.text = senderName;
        message.bodyText.text = e.message;
    }

    private void Send()
    {
        string message = inputField.text.Trim();
        if (message.Length <= 0)
            return;
        FNAFClient.Instance.ChatRequest(message);
        inputField.text = "";
        inputField.Select();
    }
}
