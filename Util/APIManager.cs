using JetBrains.Annotations;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : SingleTon<APIManager>
{
    Crypto crypto;
    WebRequestManager requestManager;
    string steamID;

    public APIManager()
    {
        crypto = new Crypto();
        requestManager = new WebRequestManager(crypto);
    }

    public void SetPublicKeyPEM(TextAsset pem)
    {
        crypto.SetPublicKeyPEM(pem);

    }
    public void SetPrivateKeyPEM(TextAsset pem)
    {
        crypto.SetPrivateKeyPEM(pem);

    }

    public void SetSteamID(string steamid) {
        steamID = steamid;
    }

    public string GetSteamID() {
        if (steamID.Equals(""))
        {
            UIStatus.Instance.steam_id = SteamUser.GetSteamID().ToString();
          
        }
        else
        {
            UIStatus.Instance.steam_id = steamID;
        }

        return UIStatus.Instance.steam_id;
    }

    public async Task<bool> CheckNicknameDuplicated(string data)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("name", data);
       DuplicatedNickName duplicatedNickName=  (DuplicatedNickName)await requestManager.Get<DuplicatedNickName>(APIAddressManager.REQUEST_CHECKNAME, sendData);
        return duplicatedNickName.data.isDuplicated;
    }
    public async Task<NormalResult> TryRegist(string name, string nickname)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        sendData.Add("name", nickname);
        return (NormalResult)await requestManager.Post<NormalResult>(APIAddressManager.REQUEST_REGIST, sendData);
    }
    public async Task<bool> TryLogin(string name)
    {
        DebugManager.Instance.PrintDebug("[WebRequest] " + "Reqested Login ");
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        LoginResult result = (LoginResult)await requestManager.Post<LoginResult>(APIAddressManager.REQUEST_LOGIN, sendData);

        DebugManager.Instance.PrintDebug("[WebRequest] "+"Login Result for "+name+" result : "+result.statusCode);
        if (result.statusCode == 200) { 
            UIStatus.Instance.nickname = result.data.name;
            return true;
        }
        return false;

    }
    public async Task<StartGame> StartGame(string name, string nickname)
    {
        DebugManager.Instance.PrintDebug("[WebRequest] " + "Reqested GetStartData ");
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", name);
        sendData.Add("name", nickname);
        StartGame startGame = (StartGame)await requestManager.Get<StartGame>(APIAddressManager.REQUEST_GAME_START, sendData);
        UIStatus.Instance.steam_id = startGame.data.steam_id;
        UIStatus.Instance.high_stage = startGame.data.high_stage;
        UIStatus.Instance.last_stage = startGame.data.last_stage;
        UIStatus.Instance.last_is_finished = startGame.data.last_is_finished;
        UIStatus.Instance.last_is_clear = startGame.data.last_is_clear;
        UIStatus.Instance.last_saint_soul_type = startGame.data.last_saint_soul_type;
        UIStatus.Instance.last_soul1 = startGame.data.last_soul1;
        UIStatus.Instance.last_soul2 = startGame.data.last_soul2;
        UIStatus.Instance.last_soul3 = startGame.data.last_soul3;
        UIStatus.Instance.last_soul4 = startGame.data.last_soul4;
        UIStatus.Instance.last_soul5 = startGame.data.last_soul5;
        UIStatus.Instance.last_soul6 = startGame.data.last_soul6;
        UIStatus.Instance.selectedChar = UIStatus.Instance.GetSorcerer( startGame.data.last_character);
        UIStatus.Instance.key = startGame.data.key;
        UIStatus.Instance.hojin= startGame.data.hojin;
        UIStatus.Instance.seimei= startGame.data.seimei;
        UIStatus.Instance.macia= startGame.data.macia;
        UIStatus.Instance.sinwol = startGame.data.sinwol;
        UIStatus.Instance.siWoo= startGame.data.siWoo;
        UIStatus.Instance.ulises = startGame.data.ulises;
       return startGame;       
    }

    public async Task<BuySorcererResult> UnlockSorcerer(int sorcererId) {
        BuySorcererResult nr = null;
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        sendData.Add("character", UIStatus.Instance.GetSorcerer(sorcererId));

        nr = (BuySorcererResult)await requestManager.Patch<BuySorcererResult>(APIAddressManager.REQUEST_OPEN_SORCERER, sendData);
        UIStatus.Instance.key = nr.data.keys;
        return nr;
    }
    public async Task<OwnBoxResult> GetBox()
    {
        OwnBoxResult result = null;
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        result = (OwnBoxResult)await requestManager.Get<OwnBoxResult>(APIAddressManager.REQUEST_USER_OWN_BOX+GetSteamID(), sendData);
        return result;
    }
    public async Task<BoxOpenStart> BoxOpenStart(int id)
    {
        BoxOpenStart result = null;
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        sendData.Add("id", id);
        result = (BoxOpenStart)await requestManager.Patch<BoxOpenStart>(APIAddressManager.REQUEST_BOX_OPEN_START, sendData);
        return result;
    }
    public async Task<BoxOpen> BoxOpen(int id)
    {
        BoxOpen result = null;
        Dictionary<string,object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", GetSteamID());
        sendData.Add("id", id);
        result = (BoxOpen)await requestManager.Patch<BoxOpen>(APIAddressManager.REQUEST_BOX_OPEN, sendData);
        return result;
    }

    public async Task<NormalResult> SetSorcererRequest(string name)
    {
        DebugManager.Instance.PrintDebug("[WebRequest] " + "Reqested GetStartData ");
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("steam_id", UIStatus.Instance.steam_id);
        sendData.Add("character", name);
        NormalResult result = (NormalResult)await requestManager.Patch<NormalResult>(APIAddressManager.REQUEST_SELECT_SORCERER, sendData);
        
        return result;
    }

    /// <summary>
    /// 혼의 해금 여부를 반환해주는 함수
    /// </summary>
    /// <param name="mainSoulId">성흔 아이디</param>
    /// <param name="soulId">혼 아이디</param>
    /// <param name="count">최대치</param>
    /// <returns></returns>
    public async Task<bool> UnlockSoul(int mainSoulId, int soulId, int count)
    {
        DebugManager.Instance.PrintDebug("[WebRequest] Reqested Soul Progress (Main Soul ID: {0})", mainSoulId);

        Dictionary<string, object> sendData = new Dictionary<string, object>()
        {
            {"steam_id", GetSteamID() },
            {"souls_id", mainSoulId % 100},
        };

        SoulProgress soulProgress = (SoulProgress)await requestManager.Get<SoulProgress>(APIAddressManager.REQUEST_PROGRESS_SOUL, sendData);
        
        switch (soulId % 1000)
        {
            case 101:
                return soulProgress.data.soul1_count >= count;
            case 102:
                return soulProgress.data.soul2_count >= count;
            case 103:
                return soulProgress.data.soul3_count >= count;
            case 201:
                return soulProgress.data.soul4_count >= count;
            case 202:
                return soulProgress.data.soul5_count >= count;
            case 203:
                return soulProgress.data.soul6_count >= count;
            case 301:
                return soulProgress.data.soul7_count >= count;
            case 302:
                return soulProgress.data.soul8_count >= count;
            case 303:
                return soulProgress.data.soul9_count >= count;
            case 401:
                return soulProgress.data.soul10_count >= count;
            case 402:
                return soulProgress.data.soul11_count >= count;
            case 403:
                return soulProgress.data.soul12_count >= count;
            case 501:
                return soulProgress.data.soul13_count >= count;
            case 502:
                return soulProgress.data.soul14_count >= count;
            case 503:
                return soulProgress.data.soul15_count >= count;
            case 601:
                return soulProgress.data.soul16_count >= count;
            case 602:
                return soulProgress.data.soul17_count >= count;
            case 603:
                return soulProgress.data.soul18_count >= count;
            default:
                return false;
        }
    }

    /// <summary>
    /// 해당 성흔에 있는 혼들의 진척도 업데이트 (크기가 18인 int형 배열 필요)
    /// </summary>
    /// <param name="mainSoulId"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public async Task<NormalResult> SoulProgressUpdate(int mainSoulId, int[] array)
    {
        if (array.Length != 18)
        {
            DebugManager.Instance.PrintDebug("[WebRequest] SoulProgressUpdate Failed. (배열 사이즈를 확인해 주세요)");
            return null;
        }

        DebugManager.Instance.PrintDebug("[WebRequest] Patch Soul Progress ");

        Dictionary<string, object> sendData = new Dictionary<string, object>()
        {
            {"steam_id", GetSteamID() },
            {"souls_id", mainSoulId % 100},
            {"now_count_list", array},
        };

        NormalResult result = (NormalResult)await requestManager.Patch<NormalResult>(APIAddressManager.REQUEST_PROGRESS_SOUL_UPDATE, sendData);
        return result;
    }

}
