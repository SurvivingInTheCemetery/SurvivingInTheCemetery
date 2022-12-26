using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.PixelArtMonster_Dungeon;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ReaperSkill_Blue : MonoBehaviour
{
    public bool skillRetention = false;
    public float skillTime;
    public float skillCoolTime = 20f;
    public float skillRetentionTime = 5f;

    public GameObject bat;
    public float passiveTime;
    public float passiveCoolTime = 5f;

    public Animator animator;
    ReaperController reaper;
    ReaperButtonInput Input;
    Reaper reaperStat;

    void Start()
    {
        reaper = GetComponent<ReaperController>();
        Input = GetComponent<ReaperButtonInput>();
        reaperStat = GetComponent<Reaper>();
    }

    void Update()
    {
        skillTime += Time.deltaTime;
        passiveTime += Time.deltaTime;

        if(passiveCoolTime <= passiveTime)
        {
            Passive();
        }

        if (Input.inputSkill && skillCoolTime <= skillTime)
        {
            Skill();
        }

        if(skillTime >= skillRetentionTime && skillRetention)
        {
            reaperStat.reaperAttack = 50;
            skillTime = 0;
            reaper.speedMax = 2;
            reaper.acc = 8;
            reaper.brakeAcc = 8;
            skillRetention = false;
        }
    }
    public void Passive()
    {
        passiveTime = 0;
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject a = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PF_Bat"), GetComponent<Reaper>().gameObject.transform.position,Quaternion.identity);
        }
    }

    public void Skill()
    {
        animator.SetTrigger("canSkill");
        skillTime = 0;
        reaper.speedMax = 10;
        reaper.acc = 10;
        reaper.brakeAcc = 10;
        reaperStat.reaperAttack = 75;
        skillRetention = true;
    }
}
