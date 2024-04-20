using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bhaptics.SDK2;
using MelonLoader;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        public bool suitDisabled = true;
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);


        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                BhapticsSDK2.Play("HeartBeat".ToLower());
                Thread.Sleep(1000);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            var res = BhapticsSDK2.Initialize("EbZ73nerOmcM3AOVoyr2", "Df9MuZU0Q9x2VEh27MwU");

            suitDisabled = res != 0;
            LOG("Starting HeartBeat and NeckTingle thread... " + res);
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }


        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void PlayBackHit(String key, float xzAngle, float yShift)
        {
            // two parameters can be given to the pattern to move it on the vest:
            // 1. An angle in degrees [0, 360] to turn the pattern to the left
            // 2. A shift [-0.5, 0.5] in y-direction (up and down) to move it up or down
            BhapticsSDK2.Play(key.ToLower(), 1f, 1f, xzAngle, yShift);
        }

        public void ShootRecoil(bool isRightHand, float intensity = 1.0f, bool twoHanded = false )
        {
            float duration = 1.0f;
            string postfix = "_L";
            string otherPostfix = "_R";
            if (isRightHand) { postfix = "_R"; otherPostfix = "_L"; }
            string key = "ShootRecoil" + postfix;
            if (twoHanded) key = "ShootRecoilTwohanded" + postfix;
            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);

        }

        public void BurstRecoil(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }

            string key = "BurstRecoil" + postfix;

            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void AbsorbSoul(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }

            string key = "AbsorbSoul" + postfix;

            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void TranformWeapon(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }

            string key = "TransformWeapon" + postfix;

            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void MeleeRecoil(bool isRightHand, bool twoHanded, float intensity = 1.0f)
        {
            float duration = 1.0f;
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string key = "MeleeRecoil" + postfix;
            if (twoHanded) key = "MeleeRecoilTwohanded" + postfix;

            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public bool IsPlaying(String effect)
        {
            return BhapticsSDK2.IsPlaying(effect.ToLower());
        }

        public void StopHapticFeedback(String effect)
        {
            BhapticsSDK2.Stop(effect.ToLower());
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            BhapticsSDK2.StopAll();
        }

        public void StopThreads()
        {
            StopHeartBeat();
        }


    }
}
