using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NUnit.Framework;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Button[] LigthOnButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < LigthOnButton.Length; i++)
        {
            Button button = LigthOnButton[i];
            int index = i;
            button.onClick.AddListener(() => LigthOnButtonClick(index));
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "On";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LigthOnButtonClick(int index)
    {
        ToggleLight(index);

        if (0 < index)
        { 
            ToggleLight(index - 1);
            
        }

        if(index < LigthOnButton.Length - 1) { 

            ToggleLight(index + 1);
        }

    }

    void ToggleLight(int index)
    {
        Button button = LigthOnButton[index];
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = text.text == "On" ? "Off" : "On";
    }
}
