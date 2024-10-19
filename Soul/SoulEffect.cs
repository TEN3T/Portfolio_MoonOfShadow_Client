using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoulEffect
{
    #region 컨셉: 공격력
    MULTIATTACK,            //충돌한 적의 수 만큼 데미지 증폭
    ATTACK,                 //캐릭터 공격력
    //SingleDamage 그대로 쓰기

    MOVESPEED,              //이동 속도
    HP,                     //체력
    ARMOR,                  //받는 피해 감소

    RANGEDAMAGE,            //범위 스킬 데미지
    SKILLDAMAGE,            //모든 스킬 전체 데미지
    PROJECTILEDAMAGE,       //투사체 스킬 데미지

    BOSSDAMAGE,             //보스 몬스터 데미지
    NORMALDAMAGE,           //일반 몬스터 데미지
    SINGLEDAMAGE,           //단일 대상 데미지

    PENETRATEDAMAGE,        //관통 스킬 데미지
    BOOMDAMAGE,             //폭발 스킬 데미지
    ADDSKILL,               //추가 스킬 등장
    #endregion
}
