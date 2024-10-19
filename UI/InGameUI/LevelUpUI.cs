using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    //private const int SKILL_MAX_LEVEL = 8;
    //private const int ACTIVE_SKILL_MAX_COUNT = 6;
    //private const int PASSIVE_SKILL_MAX_COUNT = 5;
    //private const int SKILL_MAX_COUNT = ACTIVE_SKILL_MAX_COUNT + PASSIVE_SKILL_MAX_COUNT;

    private Dictionary<string, Dictionary<string, object>> skillTable;
    private Dictionary<string, Dictionary<string, object>> passiveTable;

    private Transform body;
    private RectTransform bodyRect;
    private List<int> skillBenList;
    private SoundRequesterSFX soundRequester;
    //private List<int> skillNums = new List<int>();
    private Dictionary<int, int> skillIds;

    public bool isSelect { get; private set; }
    public List<SkillUI> skillUis { get; private set; } = new List<SkillUI>();
    //public int skillCount { get; private set; } = 0;
    public List<int> skills { get; private set; } = new List<int>();   //가진 스킬이 아니라 ui에 올라온 스킬 목록 (중복 방지)

    private void Awake()
    {
        try
        {
            skillBenList = (CSVReader.Read("BattleConfig", "SkillBenList", "ConfigValue") as List<string>).Select(int.Parse).ToList();
        }
        catch
        {
            skillBenList = new List<int>
            {
                Convert.ToInt32(CSVReader.Read("BattleConfig", "SkillBenList", "ConfigValue"))
            };
        }

        skillTable = CSVReader.Read("SkillTable");
        passiveTable = CSVReader.Read("PassiveTable");


        soundRequester = GetComponent<SoundRequesterSFX>();

        SkillNumRead();
        body = transform.Find("Body");
        bodyRect = body.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
    
    }

    private void CloseBox(int skillId)
    {

        if (skillId > 0)
        {
            SkillManager.Instance.SkillAdd(skillId, GameManager.Instance.player.transform, skillId / 10000 == 1 ? PlayerUI.Instance.activeSkillCount : PlayerUI.Instance.passiveSkillCount);
        }
        
        foreach (SkillUI ui in skillUis)
        {
            UIPoolManager.Instance.DeSpawnUI("SkillUI", ui);
        }
        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.DEMISE);
        }
        GameManager.Instance.UnPause();
        gameObject.SetActive(false);
        isSelect = true;
    }
    public void ShootLevelUPSound() {
        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.ACTIVE);
        }
    }
    public void SkillBoxInit(int num)
    {
        isSelect = false;
        float height = 175 + 120 * num;
        bodyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        int j = 0;
        for (int i = 0; i < num; i++)
        {
            int skillId = RandomSkillId();
            if (skillId < 0)
            {
                continue;
            }
            Vector2 pos = new Vector2(0, height * 0.5f - 175 - 120 * j++);
            SkillUI skillUi = (SkillUI)UIPoolManager.Instance.SpawnUI("SkillUI", body.transform, pos);
            if (skillId / 10000 == 1)
            {
                skillUi.UISetting(skillTable[skillId.ToString()]);
            }
            else if (skillId / 10000 == 2)
            {
                skillUi.UISetting(passiveTable[skillId.ToString()]);
            }
            skillUi.btn.onClick.RemoveAllListeners();
            skillUi.btn.onClick.AddListener(() => CloseBox(skillId));
            skillUis.Add(skillUi);
        }

        for (int i = j; i < num; i++)
        {
            Vector2 pos = new Vector2(0, height * 0.5f - 175 - 120 * i);
            SkillUI skillUi = (SkillUI)UIPoolManager.Instance.SpawnUI("SkillUI", body.transform, pos);
            skillUi.UISetting("Arts/Dummy", "", "");
            skillUi.btn.onClick.RemoveAllListeners();
            skillUi.btn.onClick.AddListener(() => CloseBox(-1));
        }
    }

    private int RandomSkillId()
    {
        int skillId = -1;
        if (PlayerUI.Instance.skillCount < SkillManager.SKILL_MAX_COUNT) //스킬칸이 남은 경우
        {
            skillIds = skillIds.FisherVateShuffle();
            //for (int i = 0; i < skillIds.Count; i++)
            foreach (int i in skillIds.Keys)
            {
                skillId = i;
                
                if (skills.Contains(skillId))
                {
                    continue;
                }
                if (skillId / 100 == 1 && PlayerUI.Instance.activeSkillCount == SkillManager.ACTIVE_SKILL_MAX_COUNT)
                {
                    continue;
                }
                if (skillId / 100 == 2 && PlayerUI.Instance.passiveSkillCount == SkillManager.PASSIVE_SKILL_MAX_COUNT)
                {
                    continue;
                }

                bool max = false;
                foreach (int id in SkillManager.Instance.skillList.Keys)
                {
                    if (id / 100 == skillId) //가지고 있는 스킬일 때
                    {
                        if (id % 100 != skillIds[skillId]) //만렙이 아니라면
                        {
                            skills.Add(skillId);
                            return id + 1;
                        }
                        else //만렙이라면
                        {
                            max = true;
                            break;
                        }
                    }
                }

                if (max)
                {
                    continue;
                }
                if (skillBenList.Contains(skillId))
                {
                    continue;
                }

                skills.Add(skillId);
                return skillId * 100 + 1;
            }
        }
        else
        {
            int index = UnityEngine.Random.Range(0, SkillManager.SKILL_MAX_COUNT);
            for (int i = 0; i < SkillManager.SKILL_MAX_COUNT; i++)
            {
                skillId = SkillManager.Instance.skillList.Keys.ElementAt((i + index) % SkillManager.SKILL_MAX_COUNT);
                if (skills.Contains(skillId / 100))
                {
                    continue;
                }
                if (skillId % 100 != skillIds[skillId / 100])
                {
                    skills.Add(skillId / 100);
                    return skillId + 1;
                }

            }
        }

        return -1;
    }

    private void SkillNumRead()
    {
        skillIds = new Dictionary<int, int>();

        foreach (string id in skillTable.Keys)
        {
            try
            {
                int i = Convert.ToInt32(id) / 100;
                if (!skillIds.ContainsKey(i))
                {
                    skillIds.Add(i, 1);
                    skillIds[i]++;
                }
                else
                {
                    skillIds[i]++;
                }
            }
            catch
            {
                continue;
            }
        }

        foreach (string id in passiveTable.Keys)
        {
            try
            {
                int i = Convert.ToInt32(id) / 100;
                if (!skillIds.ContainsKey(i))
                {
                    skillIds.Add(i, 1);
                }
                else
                {
                    skillIds[i]++;
                }
            }
            catch
            {
                continue;
            }
        }
    }
}
