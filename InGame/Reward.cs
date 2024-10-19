using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    private int[] rewardAmount;

    public void SetReward(int[] rewardID)
    {
        //TODO :: rewardID 검색하여 아이템 배열 저장
    }

    public void SetRewardAmount(int[] _rewardAmount)
    {
        rewardAmount = _rewardAmount;
    }
}
