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
                tactsuitVr.Recoil("Blade", rightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerDamageReciver), "ReceiveAttack", new Type[] { typeof(IAttackBase), typeof(Vector3) })]
        public class bhaptics_ReceiveDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerDamageReciver __instance, Vector3 hitPos)
            {
                tactsuitVr.LOG("Damage Received.");
                // __instance.Player.CharacterPosition

            }
        }

        [HarmonyPatch(typeof(SoulAbsorber), "Absorption", new Type[] { typeof(SoulEffect) })]
        public class bhaptics_SoulAbsorbe
        {
            [HarmonyPostfix]
            public static void Postfix(SoulAbsorber __instance, ref ControllerHand ___viberationHand)
            {
                bool isRight = (___viberationHand == ControllerHand.Right);
                tactsuitVr.PlaybackHaptics("HeartBeat");
                tactsuitVr.LOG("SoulAbsorb: " + isRight.ToString());
            }
        }

    }
}
