using GlobalEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.Components.ObjectComponents
{
    /// <summary>
    /// Intended to be placed within the ThunderOrbObjectEffect.
    /// Turns the orb into a ThunderClap
    /// </summary>
    internal class ThunderOrb : MonoBehaviour
    {
        private float SPEED = 7.5f;
        private float scale = 1;
        private HealthManager? target;
        private float MAXDIST = 20;

        private Rigidbody2D? rigidbody;
        private float radius = 0.35f;

        private HealthManager? GetClosestEnemy(float maxDist)
        {
            HealthManager[] managers = GameObject.FindObjectsByType<HealthManager>(FindObjectsSortMode.None);
            HealthManager? closest = null;
            float olddist = 9999;
            foreach (HealthManager manager in managers)
            {
                if (!manager.gameObject.activeSelf) { continue; }
                if (manager.gameObject.layer != (int)PhysLayers.ENEMIES) { continue; }

                float newdist = (manager.gameObject.transform.position - HeroController.instance.gameObject.transform.position).magnitude;
                if (newdist > maxDist) { continue; }

                if (closest == null) { closest = manager; olddist = newdist; continue; }

                if (newdist < olddist) { closest = manager; olddist = newdist; continue; }
            }

            return closest;
        }
        private void Start()
        {
            target = GetClosestEnemy(MAXDIST);
            scale = -HeroController.instance.transform.GetScaleX();
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector3 offset = new Vector3(scale * SPEED, 0);

            //This is way too laggy. Just a handful of them drops me to 20fps.
            //target = GetClosestEnemy(MAXDIST);

            if (target != null)
            {
                Vector3 direction = (target.gameObject.transform.position - gameObject.transform.position).normalized;

                offset = direction * SPEED;
            }

            if (rigidbody != null)
            {
                rigidbody.SetVelocity(offset.x, offset.y);
            }

            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D h in hit)
            {
                if (h.gameObject.layer == (int)PhysLayers.ENEMIES || h.gameObject.layer == (int)PhysLayers.TERRAIN)
                {
                    Destroy(gameObject);
                }
            }
        }

        private IEnumerator StopAfter(CircleCollider2D go, float time)
        {
            yield return new WaitForSeconds(time);
            go.enabled = false;
        }

        private void OnDestroy()
        {
            GameObject? explosion = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.THUNDEREXPLOSION);
            if (explosion != null)
            {
                GameObject clone = GameObject.Instantiate(explosion);
                clone.transform.position = gameObject.transform.position;
                clone.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                GameObject? ptcast = clone.ChildChain("parent", "Pt Cast");
                if (ptcast != null)
                {
                    ptcast.SetActive(false);
                }

                DamageEnemies dmg = clone.GetComponentInChildren<DamageEnemies>();
                if (dmg != null)
                {
                    dmg.isNailAttack = true;
                    dmg.useNailDamage = true;
                    dmg.stepsPerHit = 10;
                    dmg.nailDamageMultiplier = 0.6f;
                    dmg.stunDamage = 0.6f;
                    dmg.gameObject.name = AttackNames.THUNDERCLAP;
                    dmg.gameObject.GetComponent<CircleCollider2D>().enabled = true;
                    GameManager.instance.StartCoroutine(StopAfter(dmg.gameObject.GetComponent<CircleCollider2D>(), 0.3f));
                }

                clone.SetActive(true);
            }
        }

        private void OnTriggerEnter2D(Collision2D col)
        {
            Destroy(gameObject);
        }
    }
}
