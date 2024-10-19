using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private bool isWin = true;
    private Button endBtn;
    private Image fadeBox;
    private TextMeshProUGUI getItemText;

    private void Awake()
    {
        fadeBox = transform.Find("FadeBox").GetComponent<Image>();
        fadeBox.gameObject.SetActive(false);

        endBtn = GetComponentInChildren<Button>();
        endBtn.onClick.AddListener(ReStart);
        if (isWin)
        {
            endBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizeManager.Instance.GetText("UI_Result_Confirm");
        }
        else
        {
            endBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizeManager.Instance.GetText("UI_Result_Exit");
        }

        getItemText = transform.Find("GetItemText").GetComponent<TextMeshProUGUI>();
        getItemText.text = LocalizeManager.Instance.GetText("UI_Result_GetItem");
    }

    private void ReStart()
    {
        fadeBox.gameObject.SetActive(true);
        endBtn.interactable = false;
        StartCoroutine(SceneFadeOut());
    }

    private IEnumerator SceneFadeOut()
    {
        float alpha = 0f;
        WaitForSecondsRealtime sec = new WaitForSecondsRealtime(0.01f);
        while (alpha < 1.0f)
        {
            alpha += 0.01f;
            yield return sec;
            Color color = fadeBox.color;
            color.a = alpha;
            fadeBox.color = color;
        }
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("UI", LoadSceneMode.Single);
    }
}
