using SKILLCONSTANT;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Vector2 playerDirection;
    private PlayerItem playerItem;
    private Transform shadow;
    private SpineManager spineManager;
    private WaitForSeconds invincibleTime;
    private StatusEffect statusEffect;
    private SoundRequester soundRequester;
    private AudioSource playerAudioSource;
    private AudioSource playerVoiceAudioSource;

    private const long VOICE = 15*1000;

    private int exState = 0;
    private int shootVoiceTimer = 0;
    private HpBar hpBar;
    private Vector3 hpBarPos = new Vector3(0.0f, -0.6f, 0.0f);

    private AudioClip[] startVoice;
    private AudioClip[] randomVoice;
    private AudioClip[] dieVoice;

    [SerializeField]
    public VoicePackItem[] voicePackItems;

    public Transform character { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public Vector2 lookDirection { get; private set; } //바라보는 방향
    public int exp { get; private set; }
    public int level { get; private set; }
    public int needExp { get; private set; }

    


    #region Mono
    private void Awake()
    {
        statusEffect = new StatusEffect();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerDirection = Vector3.zero;
        lookDirection = Vector3.left;
        character = transform.Find("Character");
        shadow = transform.Find("Shadow");
        spineManager = GetComponent<SpineManager>();
        playerManager = GetComponentInChildren<PlayerManager>();

        playerItem = GetComponentInChildren<PlayerItem>();
        gameObject.tag = "Player";
        invincibleTime = new WaitForSeconds(Convert.ToInt32(Convert.ToString(CSVReader.Read("BattleConfig", "InvincibleTime", "ConfigValue"))) / 1000.0f);
    }

    private void Start()
    {
        level = 1;
        needExp = Convert.ToInt32(CSVReader.Read("LevelUpTable", (level + 1).ToString(), "NeedExp"));
        hpBar = (HpBar)UIPoolManager.Instance.SpawnUI("HpBar", PlayerUI.Instance.transform.Find("HpBarUI"), transform.position);
        AudioSetting();
        ShootPlayerVoice(startVoice,true);
        //hpBar.HpBarSwitch(true);
    }

    /*
     *키보드 입력이랑 움직이는 부분은 안정성을 위해 분리시킴
     *Update -> 키보드 input
     *FixedUpdate -> movement
     */
    private void Update()
    {
    
        KeyDir();
        ShootPlayerVoice(randomVoice);
        hpBar.HpBarSetting(transform.position + hpBarPos, playerManager.playerData.currentHp, playerManager.playerData.hp);
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Movement, Animation
    //키보드 입력을 받아 방향을 결정하는 함수
    private void KeyDir()
    {
        if (!GameManager.Instance.playerTrigger)
        {
            playerDirection = Vector2.zero;
            playerRigidbody.velocity = Vector2.zero;
            return;
        }

        playerDirection.x = Input.GetAxisRaw("Horizontal");
        playerDirection.y = Input.GetAxisRaw("Vertical");

        if (playerDirection != Vector2.zero)
        {
            lookDirection = playerDirection; //쳐다보는 방향 저장
        }
    }

    private void Move()
    {
        spineManager.SetDirection(character, playerDirection);
        spineManager.SetDirection(shadow, playerDirection);
        playerRigidbody.velocity = playerDirection.normalized * playerManager.playerData.moveSpeed;

        if (playerRigidbody.velocity == Vector2.zero)
        {
            Vector3 pos = transform.localPosition;
            pos.y += 0.00005f;
            transform.localPosition = pos;
            spineManager.SetAnimation("Idle", true);
            if ((soundRequester != null && exState ==1) || (soundRequester != null && !soundRequester.isPlaying(SoundSituation.SOUNDSITUATION.IDLE)))
                soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.IDLE);
            exState =0;
        }
        else
        {
            spineManager.SetAnimation("Run", true, 0, playerManager.playerData.moveSpeed);
            if ((soundRequester != null && exState == 0)||(soundRequester != null && !soundRequester.isPlaying(SoundSituation.SOUNDSITUATION.RUN)))
                soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.RUN);
            
            exState = 1;
        }
    }

    private void ShootPlayerVoice(AudioClip[] audioClipList, bool ignore = false)
    {
        shootVoiceTimer++;

        if (shootVoiceTimer == VOICE || ignore)
        {
            if (randomVoice.Length != 0)
            {   
                shootVoiceTimer=0;
                int rnd = Random.Range(0, audioClipList.Length);
                DebugManager.Instance.PrintDebug("[SoundRequest] Player Shoot Sound " + rnd);
                playerVoiceAudioSource.PlayOneShot(audioClipList[rnd]);
            }
        }


    }
    public void DiePlayerVoice()
    {
         if (dieVoice.Length != 0)
            {

                int rnd = Random.Range(0, dieVoice.Length);
                DebugManager.Instance.PrintError("[SoundRequest] Player Shoot Die Sound " + rnd);
                 playerVoiceAudioSource.PlayOneShot(dieVoice[rnd]);
            }

    }
    #endregion

    #region Level
    //public void GetExp(int exp)
    //{
    //    this.exp += playerManager.playerData.ExpBuff(exp);

    //    if (this.exp >= needExp)
    //    {
    //        LevelUp();
    //    }
    //}

    public void UpdateGetItemRange()
    {
        playerItem.UpdateItemRange();
    }

    //private void LevelUp()
    //{
    //    exp -= needExp;
    //    needExp = Convert.ToInt32(CSVReader.Read("LevelUpTable", (++level + 1).ToString(), "NeedExp"));
    //    GameManager.Instance.playerUi.LevelTextChange(level);
    //    GameManager.Instance.playerUi.SkillSelectWindowOpen();
    //}
    #endregion

    #region Collider
    public IEnumerator Invincible(WaitForSeconds seconds)
    {
        spineManager.SetColor(Color.red);
        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.HIT);
        }

        yield return seconds;
        spineManager.SetColor(Color.white);
    }

    public IEnumerator Invincible()
    {
        yield return this.Invincible(invincibleTime);
    }
    #endregion

    #region Sound
    private void AudioSetting()
    {
        playerVoiceAudioSource = gameObject.AddComponent<AudioSource>();

        SoundManager.Instance.AddAudioSource("Skill", GetComponent<AudioSource>(), SettingManager.EFFECT_SOUND);
        SoundManager.Instance.AddAudioSource("PlayerVoice", playerVoiceAudioSource, SettingManager.VOCIE_SOUND);


        soundRequester = GetComponent<SoundRequesterSFX>();
        playerAudioSource = GetComponent<AudioSource>();

        playerAudioSource.volume = SoundManager.Instance.GetSettingSound( SettingManager.EFFECT_SOUND);
        playerVoiceAudioSource.volume = SoundManager.Instance.GetSettingSound(SettingManager.VOCIE_SOUND);

        startVoice = voicePackItems[0].GetSoundList();
        randomVoice = voicePackItems[1].GetSoundList();
        dieVoice = voicePackItems[2].GetSoundList();

    }
    #endregion

    #region STATUS_EFFECT
    public void RemoveStatusEffect(STATUS_EFFECT effect)
    {
        statusEffect.RemoveStatusEffect(effect);
    }

    public IEnumerator FireDot(int time, float dotDamage)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.FIRE))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.FIRE);
        WaitForSeconds sec = new WaitForSeconds(1.0f);
        for (int i = 0; i < time; i++)
        {
            if (soundRequester != null)
            {
                soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.BURNED);
            }

            StartCoroutine(Invincible());
            this.playerManager.playerData.CurrentHpModifier(-(int)dotDamage);
            yield return sec;
        }
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.FIRE);
    }

    public IEnumerator Slow(float time, float value)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.SLOW))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.SLOW);
        float decreaseValue = value * 0.01f * this.playerManager.playerData.moveSpeed;
        this.playerManager.playerData.MoveSpeedModifier(-decreaseValue);
        yield return new WaitForSeconds(time);
        this.playerManager.playerData.MoveSpeedModifier(decreaseValue);
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.SLOW);
    }

    public IEnumerator ChangeForm(float time, float id)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.TRANSITION))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.TRANSITION);

        SkeletonDataAsset asset = spineManager.GetSkeletonDataAsset();
        spineManager.SetSkeletonDataAsset(ResourcesManager.Load<Player>(CSVReader.Read("CharacterTable", id.ToString(), "CharacterPrefabPath").ToString()).transform.Find("Character").GetComponent<SkeletonAnimation>().skeletonDataAsset);
        spineManager.SetAnimation("Idle", true);
        playerManager.PlayerChange(playerManager.FindCharacter(Convert.ToString(id)));

        ActiveSkill prevSkill = (ActiveSkill)SkillManager.Instance.skillList[playerManager.playerData.basicSkillId];
        int newSkillId = Convert.ToInt32(CSVReader.Read("CharacterTable", id.ToString(), "SkillID_02"));
        SkillManager.Instance.SwapSkill(prevSkill.skillId, newSkillId);

        yield return new WaitForSeconds(time);

        statusEffect.RemoveStatusEffect(STATUS_EFFECT.TRANSITION);
        spineManager.SetSkeletonDataAsset(asset);
        spineManager.SetAnimation("Idle", true);
        playerManager.PlayerChange(playerManager.FindCharacter(Convert.ToString(GameManager.Instance.GetPlayerId())));

        SkillManager.Instance.SwapSkill(newSkillId, prevSkill.skillId);
    }

    #endregion
}
