using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawnButton : MonoBehaviour
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
        int monsterId = int.Parse(inputField.text);
        MonsterSpawner.Instance.SpawnInit(monsterId);
        MonsterSpawner.Instance.SpawnMonster(monsterId, (Vector2)GameManager.Instance.player.transform.position + Random.insideUnitCircle * 5.0f);
    }
}
