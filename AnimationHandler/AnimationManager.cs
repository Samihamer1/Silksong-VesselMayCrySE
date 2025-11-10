using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static Mono.Security.X509.X520;
using static tk2dSpriteAnimationClip;

namespace VesselMayCrySE.AnimationHandler
{
    public static class AnimationManager
    {
        public const float SPRITESCALE = 3.25f;
        private static GameObject? VMCSEAnimationCollections;
        private static GameObject? DevilSwordAnimator;
        private static GameObject? KingCerberusAnimator;
        private static GameObject? BalrogAnimator;
        
        //I know hard-coding png dimensions is a bad idea. I MIGHT fix it.
        //Just let me cook first, alright?
        public static void InitAnimations()
        {

            VMCSEAnimationCollections = new GameObject("VMCSESpriteCollections");
            GameObject.DontDestroyOnLoad(VMCSEAnimationCollections);

            #region DevilSword animations

            DevilSwordAnimator = CreateAnimationObject("DevilSword Animations");

            //DevilSword animations (for DevilSwordDante)

            //SlashEffect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.SlashEffect.spritesheet.png", "SlashEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 217, 192);
            SetFrameToTrigger(DevilSwordAnimator, "SlashEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(DevilSwordAnimator, "SlashEffect", 3);

            //SlashAltEffect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.SlashAltEffect.spritesheet.png", "SlashAltEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 266, 237);
            SetFrameToTrigger(DevilSwordAnimator, "SlashAltEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(DevilSwordAnimator, "SlashAltEffect", 3);

            //SlashUpEffect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.SlashUpEffect.spritesheet.png", "SlashUpEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 215, 241);
            SetFrameToTrigger(DevilSwordAnimator, "SlashUpEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(DevilSwordAnimator, "SlashUpEffect", 3);

            //DownspikeEffect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.DownspikeEffect.spritesheet.png", "DownSpikeEffect", 20, tk2dSpriteAnimationClip.WrapMode.Once, 4, 1, 1);
            SetFrameToTrigger(DevilSwordAnimator, "DownSpikeEffect", 0); // To activate the damage frames within NailSlash
            SetFrameToTrigger(DevilSwordAnimator, "DownSpikeEffect", 3);

            //Downspike
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.Downspike.spritesheet.png", "DownSpike", 16, tk2dSpriteAnimationClip.WrapMode.Once, 2, 270, 250);

            //Downspike Red
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.DownSpikeRed.spritesheet.png", "DownSpike Red", 16, tk2dSpriteAnimationClip.WrapMode.Once, 2, 270, 250);

            //Downspike Antic
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.DownspikeAntic.spritesheet.png", "DownSpike Antic", 18, tk2dSpriteAnimationClip.WrapMode.Once, 3, 270, 250);

            //Drive Antic
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.DownspikeAntic.spritesheet.png", "Drive Antic", 12, tk2dSpriteAnimationClip.WrapMode.Once, 3, 270, 250);

            //Dashstab Antic
            //LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.Downspike.spritesheet.png", "DownSpike", 16, tk2dSpriteAnimationClip.WrapMode.Once, 2, 270, 250);

            //Dashstab
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.Dashstab.spritesheet.png", "Dashstab", 18, tk2dSpriteAnimationClip.WrapMode.Once, 3, 313, 192);

            //HighTime Effect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.HighTimeEffect.spritesheet.png", "HighTimeEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 4, 322, 304);
            SetFrameToTriggerRedSlash(DevilSwordAnimator, "HighTimeEffect", 2);

            //Reactor Effect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.Reactor.spritesheet.png", "ReactorEffect", 36, tk2dSpriteAnimationClip.WrapMode.Once, 6, 158, 38);

            //Drive Slash
            CloneAnimationTo(DevilSwordAnimator, "Hornet CrestWeapon Shaman Anim", "Slash_Charged", "DriveSlash", 40);

            //RoundTrip Antic
            CloneAnimationTo(DevilSwordAnimator, "Hornet CrestWeapon Shaman Anim", "Slash_Charged", "RoundTrip Antic", 35);

            //Drive Slash Fast
            CloneAnimationTo(DevilSwordAnimator, "Hornet CrestWeapon Shaman Anim", "Slash_Charged", "DriveSlashFast", 80);

            //RoundTripEffect
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.RoundTripEffect.spritesheet.png", "RoundTripEffect", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 4, 251, 86);

            //ChaserBladeIdle
            LoadAnimationTo(DevilSwordAnimator, "VesselMayCrySE.Resources.DevilSword.ChaserBladeIdle.1.png", "ChaserBlade Idle", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 1, 29, 236);

            //MillionStab
            CloneAnimationTo(DevilSwordAnimator, AnimationLibraryNames.WANDERER, "Slash_Charged Effect", "MillionStab", 20);

            //MillionStabFast
            CloneAnimationTo(DevilSwordAnimator, AnimationLibraryNames.WANDERER, "Slash_Charged Effect", "MillionStabFast", 30);

            //Dash Stab Effect
            CloneAnimationTo(DevilSwordAnimator, "Knight", "DashStabEffect", "DashStabEffect", 24);
            SetFrameToTriggerRedSlash(DevilSwordAnimator, "DashStabEffect", 3);

            #endregion

            #region KingCerberus animations
            KingCerberusAnimator = CreateAnimationObject("KingCerberus Animations");
            //Slash
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.WITCH, "Slash", "Slash", 24);

            //SlashAlt
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.WITCH, "SlashAlt", "SlashAlt", 24);

            //DownSlash
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.WANDERER, "DownSlash", "DownSlash", 24);

            //SlashEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.SlashEffect.spritesheet.png", "SlashEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 3, 302, 244);
            SetFrameToTrigger(KingCerberusAnimator, "SlashEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SlashEffect", 2);

            //SlashAltEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.SlashAltEffect.spritesheet.png", "SlashAltEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 3, 289, 245);
            SetFrameToTrigger(KingCerberusAnimator, "SlashAltEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SlashAltEffect", 2);

            //SlashUpEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.SlashUpEffect.spritesheet.png", "SlashUpEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 3, 313, 246);
            SetFrameToTrigger(KingCerberusAnimator, "SlashUpEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SlashUpEffect", 2);

            //SlashDownEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.SlashDownEffect.spritesheet.png", "SlashDownEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 3, 249, 198);
            SetFrameToTrigger(KingCerberusAnimator, "SlashDownEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SlashDownEffect", 2);

            //Dash Attack
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.DashAttack.spritesheet.png", "Dash Attack", 24, tk2dSpriteAnimationClip.WrapMode.Once, 2, 294, 210);
            SetFrameToTrigger(KingCerberusAnimator, "SlashDownEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SlashDownEffect", 2);

            //Dash Attack Antic
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.DashAttackAntic.spritesheet.png", "Dash Attack Antic", 10, tk2dSpriteAnimationClip.WrapMode.Once, 2, 294, 210);

            //Dash Attack Recover
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.DEFAULT, "Dash Attack Recover", "Dash Attack Recover", 18);

            //Dash Stab Effect
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.CLOAKLESS, "DashStabEffect", "DashStabEffect", 24);
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "DashStabEffect", 3);

            //Dash Stab Effect_Glow
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.CLOAKLESS, "DashStabEffect_Glow", "DashStabEffect_Glow", 24);

            //King Slayer Antic
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.WITCH, "Slash_Charged", "King Slayer Antic", 24);

            //RevolverEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.RevolverEffect.spritesheet.png", "RevolverEffect", 24, tk2dSpriteAnimationClip.WrapMode.Loop, 4, 445, 430);
            SetFrameToTrigger(KingCerberusAnimator, "RevolverEffect", 0);
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "RevolverEffect", 0);

            //Revolver
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.DEFAULT, "DownSpikeBounce 1", "Revolver", 65);

            //SwingEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.SwingEffect.spritesheet.png", "SwingEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 3, 428, 409);
            SetFrameToTrigger(KingCerberusAnimator, "SwingEffect", 0);
            SetFrameToTriggerRedSlash(KingCerberusAnimator, "SwingEffect", 2);

            //HotStuffEffect
            LoadAnimationTo(KingCerberusAnimator, "VesselMayCrySE.Resources.KingCerberus.HotStuffEffect.spritesheet.png", "HotStuffEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 3, 347, 191);
            SetFrameToTrigger(KingCerberusAnimator, "HotStuffEffect", 1);

            //HotStuff 
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.SHAMAN, "Slash_Charged", "HotStuff", 36);

            //Swing Antic
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.REAPER, "DownSpike Antic", "Swing Antic", 15);

            //Swing 
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.REAPER, "DownSpike", "Swing", 30);

            //Ice Age Antic
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.DEFAULT, "AirSphere Antic", "Ice Age Antic", 40);

            //IceAge
            CloneAnimationTo(KingCerberusAnimator, AnimationLibraryNames.DEFAULT, "AirSphere Attack", "Ice Age", 24);
            #endregion

            #region Balrog animations

            BalrogAnimator = CreateAnimationObject("Balrog Animations");

            #endregion

            AnimationLoader.Test();
        }

        internal static tk2dSpriteAnimation? GetDevilSwordAnimator()
        {
            if (DevilSwordAnimator == null)
            {
                VesselMayCrySEPlugin.Instance.LogError("Devilsword animator not found");
                return null;
            }
            return DevilSwordAnimator.GetComponent<tk2dSpriteAnimator>().library;
        }

        internal static tk2dSpriteAnimation? GetKingCerberusAnimator()
        {
            if (KingCerberusAnimator == null)
            {
                VesselMayCrySEPlugin.Instance.LogError("Cerberus animator not found");
                return null;
            }
            return KingCerberusAnimator.GetComponent<tk2dSpriteAnimator>().library;
        }

        internal static tk2dSpriteAnimation? GetBalrogAnimator()
        {
            if (BalrogAnimator == null)
            {
                VesselMayCrySEPlugin.Instance.LogError("Balrog animator not found");
                return null;
            }
            return BalrogAnimator.GetComponent<tk2dSpriteAnimator>().library;
        }

        private static void SetFrameToTrigger(GameObject animatorObject, string animationName, int frame) {
            animatorObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName(animationName).frames[frame].triggerEvent = true;
        }

        private static void SetFrameToTriggerRedSlash(GameObject animatorObject, string animationName, int frame)
        {
            SetFrameToTrigger(animatorObject, animationName, frame);
            animatorObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName(animationName).frames[frame].eventInfo = "RedSlash";
        }

        public static IEnumerator PlayAnimationThenDestroy(tk2dSpriteAnimator animator, string animationName)
        {
            yield return animator.PlayAnimWait(animationName);
            UnityEngine.Object.Destroy(animator.gameObject);
        }

        public static void CloneAnimationTo(GameObject animator, string libraryName, string animationName, string newAnimationName, float newFPS)
        {
            foreach (HeroController.ConfigGroup configGroup in HeroController.instance.configs)
            {
                HeroControllerConfig config = configGroup.Config;
                if (config == null) { continue; }

                tk2dSpriteAnimation library = config.heroAnimOverrideLib;
                if (library == null) {
                    if (libraryName == "Knight")
                    { //in case of default animation
                        library = HeroController.instance.GetComponent<tk2dSpriteAnimator>().library; //setting library to default
                    }
                    else
                    {
                        continue;
                    }
                }

                if (library.name == libraryName)
                {
                    tk2dSpriteAnimationClip clip = library.GetClipByName(animationName);
                    if (clip == null) { continue; }

                    tk2dSpriteAnimationClip clone = new tk2dSpriteAnimationClip();
                    clone.CopyFrom(clip);
                    clone.name = newAnimationName;
                    clone.fps = newFPS;

                    List<tk2dSpriteAnimationClip> list = animator.GetComponent<tk2dSpriteAnimator>().Library.clips.ToList<tk2dSpriteAnimationClip>();
                    list.Add(clone);

                    tk2dSpriteAnimation animation = animator.GetComponent<tk2dSpriteAnimator>().Library;
                    animation.clips = list.ToArray();
                    Helper.SetPrivateField<bool>(animation, "isValid", false); //to refresh the animation lookup
                    animation.ValidateLookup();
                    return;
                }
            }
        }

        private static GameObject CreateAnimationObject(string name)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = HeroController.instance.transform;
            obj.name = name;
            obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimation animation = obj.AddComponent<tk2dSpriteAnimation>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animation;
            animation.clips = new tk2dSpriteAnimationClip[0];
            UnityEngine.GameObject.DontDestroyOnLoad(obj);
            UnityEngine.GameObject.DontDestroyOnLoad(animator);
            return obj;
        }

        public static void LoadAnimationTo(GameObject animator, tk2dSpriteCollectionData spriteCollectionData, string name, int fps, tk2dSpriteAnimationClip.WrapMode wrapmode, int length)
        {
            List<tk2dSpriteAnimationClip> list = animator.GetComponent<tk2dSpriteAnimator>().Library.clips.ToList<tk2dSpriteAnimationClip>();
            spriteCollectionData.material.shader = HeroController.instance.GetComponent<MeshRenderer>().material.shader;

            if (VMCSEAnimationCollections != null)
            {
                spriteCollectionData.gameObject.transform.parent = VMCSEAnimationCollections.transform;
            }

            tk2dSpriteAnimationFrame[] list1 = new tk2dSpriteAnimationFrame[length];

            for (int i = 0; i < length; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = spriteCollectionData;
                frame.spriteId = i;


                list1[i] = frame;
            }

            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = name;
            clip.fps = fps;
            clip.frames = list1;
            clip.wrapMode = wrapmode;


            clip.SetCollection(spriteCollectionData);

            list.Add(clip);
            tk2dSpriteAnimation animation = animator.GetComponent<tk2dSpriteAnimator>().Library;
            animation.clips = list.ToArray();
            Helper.SetPrivateField<bool>(animation, "isValid", false); //to refresh the animation lookup
            animation.ValidateLookup();
        }

        private static void LoadAnimationTo(GameObject animator, string path, string name, int fps, tk2dSpriteAnimationClip.WrapMode wrapmode, int length, int xbound, int ybound)
        {
            List<tk2dSpriteAnimationClip> list = animator.GetComponent<tk2dSpriteAnimator>().Library.clips.ToList<tk2dSpriteAnimationClip>();

            Texture2D texture1 = ResourceLoader.LoadTexture2D(path);
            

            string[] names = new string[length];
            Rect[] rects = new Rect[length];
            Vector2[] anchors = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                names[i] = name + (i + 1).ToString();
                rects[i] = new Rect(i * xbound, i * ybound, xbound, ybound);
                anchors[i] = new Vector2(xbound/2, ybound/2);
            }

            tk2dSpriteCollectionData spriteCollectiondata = Tk2dHelper.CreateTk2dSpriteCollection(texture1, names, rects, anchors, new GameObject());
            spriteCollectiondata.material.shader = HeroController.instance.GetComponent<MeshRenderer>().material.shader;

            if (VMCSEAnimationCollections != null)
            {
                spriteCollectiondata.gameObject.transform.parent = VMCSEAnimationCollections.transform;
            }

            tk2dSpriteAnimationFrame[] list1 = new tk2dSpriteAnimationFrame[length];

            for (int i = 0; i < length; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = spriteCollectiondata;
                frame.spriteId = i;
                

                list1[i] = frame;
            }

            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = name;
            clip.fps = fps;
            clip.frames = list1;
            clip.wrapMode = wrapmode;


            clip.SetCollection(spriteCollectiondata);

            list.Add(clip);
            tk2dSpriteAnimation animation = animator.GetComponent<tk2dSpriteAnimator>().Library;
            animation.clips = list.ToArray();            
            Helper.SetPrivateField<bool>(animation, "isValid", false); //to refresh the animation lookup
            animation.ValidateLookup();
        }
    }
}
