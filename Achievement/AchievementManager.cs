using BFM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SingletonBehaviour<AchievementManager>
{
    public AchievementData data { get; private set; }

    private void Start()
    {
        AchievementDataInit();
    }

    private void AchievementDataInit()
    {
        TestFunction();
        //API 연결
    }

    private void TestFunction()
    {
        data = new AchievementData();

        foreach (string monsterId in CSVReader.Read("MonsterTable").Keys)
        {
            if (int.TryParse(monsterId, out int id))
            {
                data.monsterKillCount.Add(id, 0);
                data.deathFrom.Add(id, 0);
            }
        }

        foreach (string characterId in CSVReader.Read("CharacterTable").Keys)
        {
            if (int.TryParse(characterId, out int id))
            {
                data.characterUnlock.Add(id, false);
            }
        }

        foreach (string stageId in CSVReader.Read("StageTable").Keys)
        {
            if (int.TryParse(stageId, out int id))
            {
                data.stagePlayCount.Add(id, 0);
                data.isStageClear.Add(id, false);
                data.stageClearCount.Add(id, 0);
                data.stageLoseCount.Add(id, 0);
            }
        }

        foreach (string itemId in CSVReader.Read("ItemTable").Keys)
        {
            if (int.TryParse(itemId, out int id))
            {
                data.useItemCount.Add(id, 0);
            }
        }

        foreach (string structureId in CSVReader.Read("FieldStructureTable").Keys)
        {
            if (int.TryParse(structureId, out int id))
            {
                data.gimmickCount.Add(id, 0);
            }
        }

        foreach (string skillId in CSVReader.Read("SkillTable").Keys)
        {
            if (int.TryParse(skillId, out int id))
            {
                data.skillLevelCount.Add(id, 0);
            }
        }
        foreach (string skillId in CSVReader.Read("PassiveTable").Keys)
        {
            if (int.TryParse(skillId, out int id))
            {
                data.skillLevelCount.Add(id, 0);
            }
        }
    }

    public int GetSoulUnlockCount(SOUL_UNLOCK condition, List<int> param)
    {
        switch (condition)
        {
            case SOUL_UNLOCK.KILLMONSTER:
                int total = 0;
                foreach (int id in param)
                {
                    total += data.monsterKillCount[id];
                }
                return total;
            case SOUL_UNLOCK.LEVELUP:
                if (param.Count == 0)
                {
                    return data.maxLevel;
                }
                return data.levelUpCount;
            case SOUL_UNLOCK.OPENCHARACTER:
                bool unlock = false;
                foreach (int id in param)
                {
                    unlock &= data.characterUnlock[id];
                }
                
                return unlock ? 1 : 0;
            case SOUL_UNLOCK.CLEARSTAGE:
                int clear = 0;
                foreach (int id in param)
                {
                    clear += data.stageClearCount[id];
                }
                return clear;
            case SOUL_UNLOCK.USEGIMMICK:
                int use = 0;
                foreach (int id in param)
                {
                    use += data.gimmickCount[id];
                }
                return use;
            case SOUL_UNLOCK.USEKEY:
                return data.useKeyCount;
            case SOUL_UNLOCK.OPENBOX:
                return data.useBoxCount;
            case SOUL_UNLOCK.DEATH:
                if (param[0] == 0)
                {
                    return data.deathTotalCount;
                }

                int death = 0;
                foreach (int level in param)
                {
                    foreach (int lv in data.deathCount.Keys)
                    {
                        if (lv <= level)
                        {
                            death += data.deathCount[lv];
                        }
                    }
                }
                return death;
            case SOUL_UNLOCK.LEVELUPSKILL:
                return data.skillLevelCount[param[0]];
            case SOUL_UNLOCK.PLAYGAME:
                if (param[0] == -1)
                {
                    return data.stageLoseTotalCount;
                }
                else if (param[0] == 0)
                {
                    return data.stagePlayTotalCount;
                }
                else if (param[0] == 1)
                {
                    return data.stageClearTotalCount;
                }
                return 0;
            case SOUL_UNLOCK.USEITEM:
                int item = 0;
                foreach (int id in param)
                {
                    item += data.useItemCount[id];
                }
                return item;
            case SOUL_UNLOCK.DONTMOVE:
                return data.dontMoveTime;
            case SOUL_UNLOCK.PLAYTIME:
                return data.playTime;
            default:
                return 0;
        }
    }
}
