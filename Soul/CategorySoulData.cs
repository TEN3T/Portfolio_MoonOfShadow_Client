using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySoulData
{
    public string categoryId { get; private set; }      //대분류 혼의 아이디 값
    public int order { get; private set; }              //대분류 혼의 화면 상에서 정렬 순서 낮 -> 높
    public string categoryName { get; private set; }    //로컬라이즈 아이디
    //언락 조건 텍스트 아이디
    public string image { get; private set; }           //대분류 혼의 일러스트 패스
    //언락 조건 이넘 배열
    //언락 조건 파람 배열
    public int soulCount { get; private set; }          //하위 혼 총 개수

    public void SetCategoryId(string categoryId) { this.categoryId = categoryId; }
    public void SetOrder(int order) { this.order = order; }
    public void SetCategoryName(string categoryName) { this.categoryName = categoryName; }
    public void SetImage(string image) { this.image = image; }
    public void SetSoulCount(int soulCount) { this.soulCount = soulCount; }
}
