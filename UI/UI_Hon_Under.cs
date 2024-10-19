using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Hon_Under : UI_Popup
{
    private Dictionary<string, Dictionary<string, object>> soulTable;
    private int[] soulIds;
    private int seonghonId;
    private GameObject hover;

    private enum Images
    {
        Hon_Box,
        Hon_MainBox,
        Back,
        Save,
    }

    private enum GameObjects
    {
        UnderSoul_1,
        UnderSoul_2,
        UnderSoul_3,
        UnderSoul_4,
        UnderSoul_5,
        UnderSoul_6,
        UnderSoul_7,
        UnderSoul_8,
        UnderSoul_9,
        UnderSoul_10,
        UnderSoul_11,
        UnderSoul_12,
        UnderSoul_13,
        UnderSoul_14,
        UnderSoul_15,
        UnderSoul_16,
        UnderSoul_17,
        UnderSoul_18,
    }

    private void Start()
    {
        hover = Instantiate(ResourcesManager.Load<GameObject>("Prefabs/UI/SoulExplainWindow"), transform);
        hover.SetActive(false);
    }

    public async void Setting(int mainCategoryId)
    {
        transform.GetComponent<Canvas>().enabled = false;
        soulIds = new int[18];

        Bind<Image>(typeof(Images));
        Array imageValue = Enum.GetValues(typeof(Images));
        for (int i = 0; i < imageValue.Length; i++)
        {
            BindUIEvent(GetImage(i).gameObject, (PointerEventData data) => { OnClickImage(data); }, Define.UIEvent.Click);
        }

        Bind<GameObject>(typeof(GameObjects));
        Array objectValue = Enum.GetValues(typeof(GameObjects));
        for (int i = 0; i < objectValue.Length; i++)
        {
            BindUIEvent(GetGameObject(i), (PointerEventData data) => { OnClickObject(data); }, Define.UIEvent.Click);
        }

        await Init(mainCategoryId);
        transform.GetComponent<Canvas>().enabled = true;
    }

    public async Task Init(int mainCategoryId)
    {
        this.soulTable = CSVReader.Read("UnderSoul");
        this.seonghonId = mainCategoryId;

        GetImage(1).sprite = ResourcesManager.Load<Sprite>("Arts/" + CSVReader.Read("MainCategorySoul", mainCategoryId.ToString(), "SoulMainImagePath").ToString());
        GetImage(0).GetComponentInChildren<TMP_Text>().text = LocalizeManager.Instance.GetText(CSVReader.Read("MainCategorySoul", mainCategoryId.ToString(), "SoulMainNameText").ToString());

        foreach (string id in soulTable.Keys)
        {
            try
            {
                if (mainCategoryId == Convert.ToInt32(soulTable[id]["SoulMainCategory"]))
                {
                    int num = 3 * (Convert.ToInt32(soulTable[id]["SoulColumnGroup"]) - 1) + Convert.ToInt32(soulTable[id]["SoulOrderInColumn"]) - 1;
                    GameObject underSoul = GetGameObject(num);
                    underSoul.GetComponent<Image>().sprite = ResourcesManager.Load<Sprite>("Arts/Hon/" + soulTable[id]["SoulImagePath"].ToString());
                    underSoul.GetComponentInChildren<TMP_Text>().text = LocalizeManager.Instance.GetText(soulTable[id]["SoulNameText"].ToString());
                    SoulExplainPopUp(underSoul);
                    //언락체크
                    if (Enum.TryParse(soulTable[id]["SoulUnlock"].ToString(), true, out SOUL_UNLOCK unlock))
                    {
                        if (int.TryParse(soulTable[id]["Count"].ToString(), out int count))
                        {
                            underSoul.transform.Find("Lock").GetComponent<Image>().enabled = !await APIManager.Instance.UnlockSoul(seonghonId, int.Parse(id), count);
                        }
                    }
                    soulIds[num] = Convert.ToInt32(id);
                }
            }
            catch (KeyNotFoundException e)
            {
                DebugManager.Instance.PrintError("[UI_Hon_Under: Error] UnderSoul 테이블에 빈 줄이 삽입되어 있습니다.");
            }
        }

        foreach (int id in SoulManager.Instance.GetSoulIdList(seonghonId))
        {
            for (int i = 0; i < soulIds.Length; i++)
            {
                if (soulIds[i] == id)
                {
                    GetGameObject(i).transform.Find("Shadow").GetComponent<Image>().enabled = false;
                }
            }
        }
    }

    public void IsSelected(GameObject obj, bool isSelected)
    {
        obj.transform.Find("Shadow").GetComponent<Image>().enabled = !isSelected;
    }

    public void OnClickImage(PointerEventData data)
    {
        Images imageValue = (Images)FindEnumValue<Images>(data.pointerClick.name);
        if ((int)imageValue < -1)
        {
            return;
        }

        DebugManager.Instance.PrintDebug(data.pointerClick.name);

        switch (imageValue)
        {
            case Images.Back:
                this.CloseUI<UI_Hon_Under>();
                break;
            case Images.Save:
                this.CloseUI<UI_Hon_Under>();
                Save();
                break;
            default:
                break;
        }
    }

    public void OnClickObject(PointerEventData data)
    {
        int soulNum = FindEnumValue<GameObjects>(data.pointerClick.name);
        if (soulNum < -1)
        {
            return;
        }

        //우클릭일 경우
        if (data.button == PointerEventData.InputButton.Right)
        {
        }
        else
        {
            if (data.pointerClick.transform.Find("Lock").GetComponent<Image>().enabled)
            {
                return;
            }

            switch(soulNum % 3)
            {
                case 0:
                    IsSelected(GetGameObject(soulNum), true);
                    IsSelected(GetGameObject(soulNum + 1), false);
                    IsSelected(GetGameObject(soulNum + 2), false);
                    break;
                case 1:
                    IsSelected(GetGameObject(soulNum - 1), false);
                    IsSelected(GetGameObject(soulNum), true);
                    IsSelected(GetGameObject(soulNum + 1), false);
                    break;
                case 2:
                    IsSelected(GetGameObject(soulNum - 2), false);
                    IsSelected(GetGameObject(soulNum - 1), false);
                    IsSelected(GetGameObject(soulNum), true);
                    break;
                default:
                    break;
            }
        }

    }

    public void Save()
    {
        SoulManager.Instance.SeonghonReset(seonghonId);
        for (int i = 0; i < Enum.GetValues(typeof(GameObjects)).Length; i++)
        {
            if (GetGameObject(i).transform.Find("Shadow").GetComponent<Image>().enabled == false)
            {
                SoulManager.Instance.Add(seonghonId, new Soul(soulIds[i]));
            }
        }

        SoulManager.Instance.PrintSoulList(seonghonId);
    }

    public void SoulExplainPopUp(GameObject obj)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((eventData) =>
        {
            Vector3 offset = obj.transform.position.x >= 0.0f ? Vector3.left : Vector3.right;
            offset += obj.transform.position;
            hover.GetComponent<RectTransform>().position = offset;
            hover.gameObject.SetActive(true);
        });
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((eventData) =>
        {
            hover.gameObject.SetActive(false);
        });
        trigger.triggers.Add(exit);
    }
}
