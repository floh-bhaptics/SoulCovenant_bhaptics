using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using VEGA.Weapon;
using VEGA;
using UnityEngine;
using VEGA.InGame.Attack;
using System.Threading;
using BNG;

[assembly: MelonInfo(typeof(SoulCovenant_bhaptics.SoulCovenant_bhaptics), "SoulCovenant_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("thirdverse", "soulcovenant")]

namespace SoulCovenant_bhaptics
{
    public class SoulCovenant_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(Weapon), "PlayHitEffect", new Type[] { typeof(Vector3) })]
        public class bhaptics_WeaponHits
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon __instance)
            {
                tactsuitVr.LOG("WeaponHits.");
                bool rightHand = (__instance.controllerHand == BNG.ControllerHand.Right);
                bool twoHanded = (__instance.WeaponAttackParameter.WeaponTypeValue == WeaponType.Critical);
                tactsuitVr.MeleeRecoil(rightHand, twoHanded);
            }
        }

        private static KeyValuePair<float, float> getAngleAndShift(Transform player, Vector3 hit)
        {
            // bhaptics pattern starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
            // y is "up", z is "forward" in local coordinates
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            Vector3 hitPosition = hit - player.position;
            Quaternion myPlayerRotation = player.localRotation;
            Vector3 playerDir = myPlayerRotation.eulerAngles;
            // get rid of the up/down component to analyze xz-rotation
            Vector3 flattenedHit = new Vector3(hitPosition.x, 0f, hitPosition.z);

            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float hitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            // check if cross product points up or down, to make signed angle myself
            Vector3 crossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (crossProduct.y > 0f) { hitAngle *= -1f; }
            // relative to player direction
            float myRotation = hitAngle - playerDir.y;
            // switch directions (bhaptics angles are in mathematically negative direction)
            myRotation *= -1f;
            // convert signed angle into [0, 360] rotation
            if (myRotation < 0f) { myRotation = 360f + myRotation; }


            // up/down shift is in y-direction
            // in Shadow Legend, the torso Transform has y=0 at the neck,
            // and the torso ends at roughly -0.5 (that's in meters)
            // so cap the shift to [-0.5, 0]...
            float hitShift = hitPosition.y;
            //tactsuitVr.LOG("HitShift: " + hitShift);
            float upperBound = 0.5f;
            float lowerBound = -0.5f;
            if (hitShift > upperBound) { hitShift = 0.5f; }
            else if (hitShift < lowerBound) { hitShift = -0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5]
            else { hitShift = (hitShift - lowerBound) / (upperBound - lowerBound) - 0.5f; }

            //tactsuitVr.LOG("Relative x-z-position: " + relativeHitDir.x.ToString() + " "  + relativeHitDir.z.ToString());
            //tactsuitVr.LOG("HitAngle: " + hitAngle.ToString());
            //tactsuitVr.LOG("HitShift: " + hitShift.ToString());

            // No tuple returns available in .NET < 4.0, so this is the easiest quickfix
            return new KeyValuePair<float, float>(myRotation, hitShift);
        }



        [HarmonyPatch(typeof(PlayerDamageReciver), "ReceiveAttack", new Type[] { typeof(IAttackBase), typeof(Vector3) })]
        public class bhaptics_ReceiveDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerDamageReciver __instance, Vector3 hitPos)
            {
                tactsuitVr.LOG("Damage Received.");
                // __instance.Player.CharacterPosition
                //Vector3 hitPosition = zombie.Position;
                Transform playerTransform = __instance.Player.PlayerCameraTransform;
                var angleShift = getAngleAndShift(playerTransform, hitPos);
                tactsuitVr.PlayBackHit("Impact", angleShift.Key, angleShift.Value);

            }
        }

        [HarmonyPatch(typeof(SoulAbsorber), "Absorption", new Type[] { typeof(SoulEffect) })]
        public class bhaptics_SoulAbsorbe
        {
            [HarmonyPostfix]
            public static void Postfix(SoulAbsorber __instance, ref ControllerHand ___viberationHand)
            {
                bool isRight = (___viberationHand == ControllerHand.Right);
                tactsuitVr.AbsorbSoul(isRight);
            }
        }

    }
}
