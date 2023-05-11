using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Wolfire;
using BepInEx;

namespace CiarenceUnbelievableModifications
{
    public static class TurretAmmoBoxBoom
    {
        public static bool verbose;
        public static void Enable()
        {
            ReceiverEvents.StartListening(ReceiverEventTypeGameObject.HitRobot, new UnityAction<ReceiverEventTypeGameObject, GameObject>(OnHitRobot));
        }

        public static void Disable()
        {
            ReceiverEvents.StopListening(ReceiverEventTypeGameObject.HitRobot, OnHitRobot);
        }

        private static void OnHitRobot(ReceiverEventTypeGameObject ev, GameObject robot)
        {
            TurretScript turret;
            if (robot.TryGetComponent<TurretScript>(out turret))
            {
                Debug.Log("shot robot is turret");
                if (!turret.ammo_alive)
                {
                    Debug.Log("turret's ammo box isn't alive. boom??");
                    if (turret.kds.last_shootable_query != null)
                    {
                        if (turret.kds.last_shootable_query.hit_collider.name == "ammo_box" && turret.ammo_belts > 0)
                        {
                            Debug.Log("turret's ammo box isn't alive. booming");
                            FireShrapnel(turret);
                        }
                        else
                        {
                            Debug.LogFormat("hit collider name was {0}, ammo count was {1}", turret.kds.last_shootable_query.hit_collider.name, turret.ammo_belts);
                        }
                    }
                    else
                    {
                        Debug.Log("last shootable query was null");
                    }
                }
                else
                {
                    Debug.Log("turret's ammo box is alive. sadge");
                }
            }
            else
            {
                Debug.Log("shot robot isn't turret");
            }
        }

        private static void FireShrapnel(TurretScript turret)
        {
            int bullet_count = (turret.bullets_per_belt * turret.ammo_belts); //to me, when a turret reloads, it loads an ammo belt from the ammo box to the place just behind the barrel. So if there's no ammo box left, no more ammo to explode.
            Transform point_muzzle_flash_turret = (Transform)typeof(TurretScript).GetField("point_muzzle_flash", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(turret);
            Transform shrapnel_source;
            shrapnel_source = turret.transform.Find("point_pivot/gun_pivot/gun_assembly/ammo_destroy");
            Debug.LogFormat("shrapnel_source is at {0}, transform parent is {1}", shrapnel_source.localPosition, shrapnel_source.parent.name);
            CartridgeSpec cartridge = (CartridgeSpec)typeof(TurretScript).GetField("cartridge_spec", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(turret);
            Debug.LogFormat("cartridge is: {0}", cartridge.diameter);
            //bullet_count += (turret.rifle_cocked) ? turret.bullets - 1 : turret.bullets; //ternary operator because why not, add all bullets currently "loaded" in the turret's magazine. if rifle is cocked subtract 1 round from the total
            Debug.LogFormat("bullet count is: {0}", bullet_count);
            for (int i = 0; i < bullet_count; i++)
            {
                Vector3 vector = LocalAimHandler.player_instance.RandomPointInCollider(1f) - shrapnel_source.transform.position;
                Vector3 vector2;
                if (i == 0 && (vector.magnitude < 10f || UnityEngine.Random.Range(0f, 1f) > 0.5f))
                {
                    vector2 = vector.normalized;
                }
                else
                {
                    Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                    onUnitSphere.z = Mathf.Abs(onUnitSphere.z);
                    vector2 = (shrapnel_source.transform.rotation * onUnitSphere).normalized;
                }
                BulletTrajectory bulletTrajectory = BulletTrajectoryManager.PlanTrajectory(shrapnel_source.transform.position + shrapnel_source.transform.forward * 0.05f, cartridge, vector2 * UnityEngine.Random.Range(0.1f, 1f), true);
                if (BulletTrajectoryManager.draw_debug_trajectory_path)
                {
                    bulletTrajectory.draw_path = BulletTrajectory.DrawType.Debug;
                }
                else
                {
                    bulletTrajectory.draw_path = BulletTrajectory.DrawType.Tracer;
                }
                bulletTrajectory.bullet_source = turret.gameObject;
                bulletTrajectory.bullet_source_entity_type = ReceiverEntityType.Turret;
                BulletTrajectoryManager.ExecuteTrajectory(bulletTrajectory);
            }
        }
    }
}
