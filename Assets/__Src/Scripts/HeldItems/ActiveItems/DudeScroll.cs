using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DudeScroll : ActiveItem
{
    public AudioClip telesound;
    private ActiveUses uses;

    public override void ActivateHold() { }
    public override void ActivateRelease() { }

    public override void ActiveDown() {
        GameMaster.instance.sfxSource.PlayOneShot(telesound);
        TeleportPlayer evnt = TeleportPlayer.Create(GetComponent<ActiveItem>().Owner.entity);
        evnt.position = GameMaster.instance.LivePlayers[Random.Range(0, GameMaster.instance.LivePlayers.Count)].transform.position;
        evnt.Send();
        uses.Use();
    }

    public override void OnEquip() {
        uses = GetComponent<ActiveUses>();
    }

    public override void OnDequip() {
    }
}
