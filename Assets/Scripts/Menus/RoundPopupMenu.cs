using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundPopupMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI roundNumberText, bodyText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPopup(string description, int roundNumber)
    {
        roundNumberText.text = "Round " + roundNumber;

        bodyText.text = description;
    }

    public void ClosePopup()
    {
        print("beeeeeeep");
        FindObjectOfType<BottomMenuHandler>().SetWaveButtonExternal();
        EventHandler.Instance.InvokeCloseRoundPopupEvent();
        gameObject.SetActive(false);
    }

}
