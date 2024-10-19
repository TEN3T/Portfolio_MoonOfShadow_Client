using BFM;
using SKILLCONSTANT;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum SOUL_UNLOCK
{
    KILLMONSTER,           //몬스터 처치
    LEVELUP,               //레벨업
    OPENCHARACTER,         //캐릭터 해금
    CLEARSTAGE,            //스테이지 클리어
    USEGIMMICK,            //기믹 사용
    USEKEY,                //열쇠 사용
    OPENBOX,               //상자 오픈
    DEATH,                  //사망
    LEVELUPSKILL,         //스킬 레벨업
    PLAYGAME,              //스테이지 플레이
    USEITEM,               //아이템 사용
    DONTMOVE,              //움직이지 않은 시간
    PLAYTIME,              //플레이 타임
}

public class SoulManager : SingletonBehaviour<SoulManager>
{
    private Dictionary<int, List<Soul>> soulList;
    //private List<Soul> soulList;
    private Dictionary<SoulEffect, float> soulEffects;

    protected override void Awake()
    {
        base.Awake();
        //soulList = new List<Soul>();
        soulList = new Dictionary<int, List<Soul>>();
        soulEffects = new Dictionary<SoulEffect, float>();
    }

    #region Soul Data & Init
    public void SeonghonReset(int seonghon)
    {
        soulList.Remove(seonghon);
    }

    public void Add(int seonghon, Soul soul)
    {
        try
        {
            if (!soulList.ContainsKey(seonghon))
            {
                soulList.Add(seonghon, new List<Soul>() { soul, });
            }
            else
            {
                foreach (Soul s in soulList[seonghon])
                {
                    if (s.soulData.soulId == soul.soulData.soulId)
                    {
                        DebugManager.Instance.PrintError("[SoulManager] 이미 장착된 혼 입니다. (SoulID: {0})", soul.soulData.soulId);
                        return;
                    }
                }
                soulList[seonghon].Add(soul);
            }

            
            for (int i = 0; i < soul.soulData.soulEffects.Count; i++)
            {
                if (soulEffects.ContainsKey(soul.soulData.soulEffects[i]))
                {
                    soulEffects[soul.soulData.soulEffects[i]] += soul.soulData.effectParams[i];
                }
                else
                {
                    soulEffects.Add(soul.soulData.soulEffects[i], soul.soulData.effectParams[i]);
                }
            }
        }
        catch
        {
            DebugManager.Instance.PrintError("[Error: SoulManager] 혼 테이블을 체크해 주세요. (SoulID: {0})", soul.soulData.soulId);
        }
        
    }

    public void Remove(int seonghon, Soul soul)
    {
        try
        {
            foreach (Soul s in soulList[seonghon])
            {
                if (s.soulData.soulId == soul.soulData.soulId)
                {
                    soulList[seonghon].Remove(s);

                    for (int i = 0; i < soul.soulData.soulEffects.Count; i++)
                    {
                        soulEffects[soul.soulData.soulEffects[i]] -= soul.soulData.effectParams[i];
                    }
                }
            }
        }
        catch
        {
            DebugManager.Instance.PrintError("[Error: SoulManager] 혼 테이블을 체크해 주세요. (SoulID: {0})", soul.soulData.soulId);
        }
    }

    public void PrintSoulList(int seonghon)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Soul soul in soulList[seonghon])
        {
            sb.Append(soul.soulData.soulId);
            sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        DebugManager.Instance.PrintDebug(sb.ToString());
    }

    public List<int> GetSoulIdList(int seonghon)
    {
        if (!soulList.ContainsKey(seonghon))
        {
            return new List<int>();
        }

        List<int> list = new List<int>();
        foreach (Soul soul in soulList[seonghon])
        {
            list.Add(soul.soulData.soulId);
        }
        return list;
    }

    public void PrintAllSoulList()
    {
        StringBuilder sb = new StringBuilder();
        foreach (int seonghon in soulList.Keys)
        {
            foreach (Soul soul in soulList[seonghon])
            {
                sb.Append(soul.soulData.soulId);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
        }
        DebugManager.Instance.PrintDebug(sb.ToString());
    }

    //Default Mode: Plus
    //Plus: return 0
    //Multi: return 1
    public float GetEffect(SoulEffect soulEffect, float value)
    {
        return this.GetEffect(soulEffect, CALC_MODE.PLUS, value);
    }

    public float GetEffect(SoulEffect soulEffect, CALC_MODE mode, float value)
    {
        if (soulEffects.ContainsKey(soulEffect))
        {
            return mode == CALC_MODE.PLUS ? soulEffects[soulEffect] : soulEffects[soulEffect] * value;
        }

        return 0.0f;
    }
    #endregion
}
