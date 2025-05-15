using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ExplosionBullet : MonoBehaviour
{
    private ParticleScriptExplosion particleScriptExplosion;
    public Transform explosionPosition;
    private PhotonView photonView;
    public int playerDamage = 0;
    public int enemyDamage = 50;
    public float playerTumbleForce = 30f;
    public float playerTumbleTorque = 50f;
    public GameObject GunChargeDetect;
    public GameObject bullet;
    public ParticleSystem ParticleTest;
    public bool BulletExplode;
    public bool BulletNotExplode;

    void Start()
    {
        particleScriptExplosion = GetComponent<ParticleScriptExplosion>();
        photonView = GetComponent<PhotonView>();

        if (GunChargeDetect == null)
            Debug.LogError("GunChargeDetect 未指派！");
    }

    void Update()
    {
        if (GunChargeDetect != null)
        {
            var chargeScript = GunChargeDetect.GetComponent<blue_rose_charge>();
            if (chargeScript != null)
            {
                if (!chargeScript.charged)
                {
                    if (!BulletExplode)
                        BulletNotExplode = true;
                }
                else
                {
                    if (!BulletNotExplode)
                        BulletExplode = true;

                    chargeScript.charged = false;
                }
            }
        }

        var bulletComponent = GetComponent<ItemGunBullet>();
        if (bulletComponent != null && bulletComponent.bulletHit && BulletExplode)
        {
            Debug.Log("BulletHit");
            OnHit();
        }
    }

    public void OnHit()
    {
        if (SemiFunc.IsMultiplayer())
        {
            photonView.RPC("ExplosionRPC_BlueRose", RpcTarget.All);
        }
        else
        {
            ExplosionRPC_BlueRose();
        }
        BulletNotExplode = true;
        BulletExplode = false;
    }

    [PunRPC]
    public void ExplosionRPC_BlueRose()
    {
        if (particleScriptExplosion == null || explosionPosition == null)
        {
            Debug.LogError("Explosion 實例化失敗：必要物件未設定");
            return;
        }

        var particlePrefabExplosion = particleScriptExplosion.Spawn(explosionPosition.position, 0.5f, 0, 250);
        if (particlePrefabExplosion == null)
        {
            Debug.LogError("Explosion Spawn 回傳 null！");
            return;
        }

        particlePrefabExplosion.SkipHurtColliderSetup = true;
        particlePrefabExplosion.HurtCollider.playerDamage = playerDamage;
        particlePrefabExplosion.HurtCollider.enemyDamage = enemyDamage;
        particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
        particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
        particlePrefabExplosion.HurtCollider.playerTumbleForce = playerTumbleForce;
        particlePrefabExplosion.HurtCollider.playerTumbleTorque = playerTumbleTorque;
    }
}
