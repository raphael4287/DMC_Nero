using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EX_mode_charge : MonoBehaviour
{
    private ParticleScriptExplosion particleScriptExplosion;
    public Transform explosionPosition;
    private PhotonView photonView;
    private bool prevToggleState;
    public int Exceed;
    public Sound ChargeFail;
    public Sound ChargeSuccess;
    public GameObject triggerPrefab;
    public Transform trigger;
    private ItemToggle itemToggle;
    private float ChargeCooldownTimer;
    public float ChargeCooldown = 0.1f;
    public int chance = 10;
    public int playerDamage = 0;
    public int enemyDamage = 50;
    public float playerTumbleForce = 30f;
    public float playerTumbleTorque = 50f;
    public ParticleSystem fireParticleSystem;
    public ParticleSystem fireParticleSystem2;
    public ParticleSystem fireParticleSystem3;

    void Start()
    {
        itemToggle = GetComponent<ItemToggle>();
        particleScriptExplosion = GetComponent<ParticleScriptExplosion>();
        explosionPosition = transform.Find("Explosion Position");
        photonView = GetComponent<PhotonView>();

        if (itemToggle == null) Debug.LogError("ItemToggle 未掛載！");
        if (trigger == null) trigger = transform.Find("Trigger");
        if (trigger == null) Debug.LogError("Trigger 尚未指派！");
    }

    void Update()
    {
        if (itemToggle != null && itemToggle.toggleState != prevToggleState)
        {
            ChargeCooldownTimer = ChargeCooldown;
            if (Random.Range(0, chance) == 0 && Exceed < 3)
            {
                Exceed += 1;
                ChargeSuccess?.Play(trigger.position);
            }
            else
            {
                ChargeFail?.Play(trigger.position);
            }

            if (Exceed == 3)
            {
                ChargeSuccess?.Play(trigger.position);
            }

            prevToggleState = itemToggle.toggleState;
        }

        if (Exceed >= 1)
            fireParticleSystem?.Play(withChildren: false);
        else
            fireParticleSystem?.Stop(withChildren: false);
        if (Exceed >= 2)
            fireParticleSystem2?.Play(withChildren: false);
        else
            fireParticleSystem2?.Stop(withChildren: false);
        if (Exceed >= 3)
            fireParticleSystem3?.Play(withChildren: false);
        else
            fireParticleSystem3?.Stop(withChildren: false);

        var melee = GetComponent<ItemMelee>();
        if (melee != null && (melee.particleSystem.isPlaying || melee.particleSystemGroundHit.isPlaying))
        {
            OnHit();
        }
    }

    public void OnHit()
    {
        if (SemiFunc.IsMasterClientOrSingleplayer() && Exceed >= 1)
        {
            if (SemiFunc.IsMultiplayer())
                photonView.RPC("ExplosionRPC_RedQueen", RpcTarget.All);
            else
                ExplosionRPC_RedQueen();

            Exceed = 0;
        }
    }

    [PunRPC]
    public void ExplosionRPC_RedQueen()
    {
        if (particleScriptExplosion == null || explosionPosition == null)
        {
            Debug.LogError("爆炸生成失敗：必要參考為 null！");
            return;
        }

        var particlePrefabExplosion = particleScriptExplosion.Spawn(explosionPosition.position, 0.5f, 0, 250);
        if (particlePrefabExplosion == null) return;

        particlePrefabExplosion.SkipHurtColliderSetup = true;
        particlePrefabExplosion.HurtCollider.playerDamage = playerDamage;
        particlePrefabExplosion.HurtCollider.enemyDamage = enemyDamage * Exceed;
        particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
        particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
        particlePrefabExplosion.HurtCollider.playerTumbleForce = playerTumbleForce;
        particlePrefabExplosion.HurtCollider.playerTumbleTorque = playerTumbleTorque;
    }
}
