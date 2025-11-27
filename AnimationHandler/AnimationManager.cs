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
        public const float SPRITESCALE = 4f;
        public static GameObject? VMCSEAnimationCollections;
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
            AnimationLoader.LoadAtlasAnimationsTo(GetDevilSwordAnimator(),
                "VesselMayCrySE.Resources.DevilSword.DevilSwordAtlas.png",
                "VesselMayCrySE.Resources.DevilSword.DevilSwordAtlas.json",
                "VesselMayCrySE.Resources.DevilSword.DevilSwordData.json");

            #endregion

            #region KingCerberus animations
            KingCerberusAnimator = CreateAnimationObject("KingCerberus Animations");

            //KingCerberus animations
            AnimationLoader.LoadAtlasAnimationsTo(GetKingCerberusAnimator(),
                "VesselMayCrySE.Resources.KingCerberus.KingCerberusAtlas.png",
                "VesselMayCrySE.Resources.KingCerberus.KingCerberusAtlas.json",
                "VesselMayCrySE.Resources.KingCerberus.KingCerberusData.json");

            #endregion

            #region Balrog animations

            BalrogAnimator = CreateAnimationObject("Balrog Animations");

            //Balrog animations
            AnimationLoader.LoadAtlasAnimationsTo(GetBalrogAnimator(),
                "VesselMayCrySE.Resources.Balrog.BalrogAtlas.png",
                "VesselMayCrySE.Resources.Balrog.BalrogAtlas.json",
                "VesselMayCrySE.Resources.Balrog.BalrogData.json");

            #endregion
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

    }
}
