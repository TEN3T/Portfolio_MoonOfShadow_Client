using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoulAddButton : MonoBehaviour
{
    private TMP_InputField inputField;
    private Button button;

    private void Start()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        button = GetComponentInChildren<Button>();

        button.onClick.AddListener(test);
    }


    private void test()
    {
        //SoulManager.Instance.Add(new Soul(int.Parse(inputField.text)));
        DebugManager.Instance.PrintError("[SoulAddButton: Error] 임시로 폐쇄된 기능입니다.");
    }
}
