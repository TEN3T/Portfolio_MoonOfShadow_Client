using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulData
{
    public int soulId { get; private set; }             //하위 혼 아이디 값
    public string soulName { get; private set; }        //하위 혼 로컬라이즈 아이디
    public string soulExplain { get; private set; }     //하위 혼 효과 로컬라이즈 아이디
    public string image { get; private set; }           //하위 혼 이미지 패스
    public List<SoulEffect> soulEffects { get; private set; }
    public List<float> effectParams { get; private set; }
    public string categorySoul { get; private set; }    //속해 있는 대분류 혼 아이디
    public int colGroup { get; private set; }           //하위 혼 내 행 그룹
    public int orderInCol { get; private set; }         //행 그룹 내 정렬 순서
    //언락 조건 이넘 배열
    //언락 조건 파람 배열

    public void SetSoulId(int soulId) { this.soulId = soulId; }
    public void SetSoulName(string soulName) { this.soulName = soulName; }
    public void SetSoulExplain(string soulExplain) { this.soulExplain = soulExplain; }
    public void SetSoulImage(string image) { this.image = image; }
    public void SetSoulEffect(List<SoulEffect> soulEffects) { this.soulEffects = soulEffects; }
    public void SetEffectParams(List<float> effectParams) { this.effectParams = effectParams; }
    public void SetCategorySoul(string categorySoul) { this.categorySoul = categorySoul; }
    public void SetColGroup(int colGroup) { this.colGroup = colGroup; }
    public void SetOrderInCol(int orderInCol) { this.orderInCol = orderInCol; }
}
