using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static Mono.Security.X509.X520;
using static tk2dSpriteAnimationClip;
using static VesselMayCrySE.AnimationHandler.AnimationLoader;

namespace VesselMayCrySE.AnimationHandler
{
    public static class AnimationLoader
    {
        #region Deserialisation classes for AtlasRoot
        public class Frame
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public class SpriteSourceSize
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public class SourceSize
        {
            public int w { get; set; }
            public int h { get; set; }
        }

        public class Pivot
        {
            public float x { get; set; }
            public float y { get; set; }
        }

        public class SpriteEntry
        {
            public Frame frame { get; set; }
            public bool rotated { get; set; }
            public bool trimmed { get; set; }
            public SpriteSourceSize spriteSourceSize { get; set; }
            public SourceSize sourceSize { get; set; }
            public Pivot pivot { get; set; }
        }

        public class AtlasRoot
        {
            public Dictionary<string, SpriteEntry> frames { get; set; }
        }

        #endregion

        #region Deseralisation classes for AnimationDataRoot

        public class AnimationData
        {
            public int fps { get; set; }

            public Dictionary<string, string> triggers { get; set; }

            public bool loop { get; set; }

            public int loopfromframe { get; set; }
        }

        #endregion

        public static void LoadAtlasAnimationsTo(tk2dSpriteAnimation library, string atlasPath, string atlasJsonPath, string animDataJsonPath)
        {
            Texture2D atlasTexture = ResourceLoader.LoadTexture2D(atlasPath);
            AtlasRoot? atlasData = ResourceLoader.LoadJSONFile<AtlasRoot>("VesselMayCrySE.Resources.testatlas.json");
            if (atlasData == null) { VesselMayCrySEPlugin.Instance.LogError($"JSON at {atlasJsonPath} not found."); return; }

            Dictionary<string, AnimationData>? animationData = ResourceLoader.LoadJSONFile<Dictionary<string, AnimationData>>("VesselMayCrySE.Resources.testanimdata.json");
            if (animationData == null) { VesselMayCrySEPlugin.Instance.LogError($"JSON at {atlasJsonPath} not found."); return; }


            List<string> names = new();
            List<Rect> regions = new();
            List<Vector2> anchors = new();


            Dictionary<string, Dictionary<int,SpriteEntry>> animationSpriteLists = new(); // {AnimationName, {{1, SpriteEntry1}, {2, SpriteEntry2}}}

            foreach (string key in atlasData.frames.Keys)
            {
                string[] splitKey = key.Split('/');
                string animationKey = splitKey[splitKey.Length - 2];

                // The Animation name will always be the penultimate directory, the last being the png itself.
                // eg.
                // RandomFolder1/../RandomFolder294/AnimationFolder/frame.png

                //Checking that the png is named correctly (integer.png)
                string pngName = splitKey[splitKey.Length - 1];
                string[] splitPngName = pngName.Split(".");
                if (splitPngName.Length != 2) { continue; }

                int frameNumber;
                if (!int.TryParse(splitPngName[0], out frameNumber)) { continue; }

                //All good

                //Add to the animationSpriteList
                if (animationSpriteLists.ContainsKey(animationKey))
                {
                    Dictionary<int, SpriteEntry> dictionary = animationSpriteLists[animationKey];
                    dictionary[frameNumber] = atlasData.frames[key];
                } else
                {
                    animationSpriteLists.Add(animationKey, new Dictionary<int, SpriteEntry>
                    {
                        [frameNumber] = atlasData.frames[key]
                    });
                }

            }

            //Processing each frame and adding to the three lists for collection data (names, regions, anchors)
            foreach (string animationName in animationSpriteLists.Keys)
            {
                Dictionary<int, SpriteEntry> spriteEntries = animationSpriteLists[animationName];

                int count = spriteEntries.Count;

                #region setup frames
                //To search frames in order
                for (int i = 1; i < count+1; i++)
                {                   
                    if (!spriteEntries.ContainsKey(i)) { VesselMayCrySEPlugin.Instance.LogError($"{animationName} frame {i} expected but not found!"); continue; }
                    SpriteEntry spriteEntry = spriteEntries[i];

                    //Searching for animation data
                    if (!animationData.ContainsKey(animationName)) { continue; }

                    //Checking frame is not null
                    Frame frame = spriteEntry.frame;
                    if (frame == null) { continue; }

                    //Begin processing entry
                    names.Add(animationName + i); // 'AnimationFolder{frameNumber}'
                    regions.Add(new Rect(frame.x, frame.y, frame.w, frame.h));

                    //Region calculations for trimmed sprites
                    Vector2 originalCenterPx = new Vector2(
                        spriteEntry.sourceSize.w * spriteEntry.pivot.x,
                        spriteEntry.sourceSize.h * spriteEntry.pivot.y
                    );

                    Vector2 offsetPx = new Vector2(
                        spriteEntry.spriteSourceSize.x,
                        spriteEntry.spriteSourceSize.y
                    );

                    Vector2 untrimmedPivotPx = originalCenterPx - offsetPx;

                    anchors.Add(new Vector2(untrimmedPivotPx.x, untrimmedPivotPx.y));
                }
                #endregion
            }

            //Creating collection data
            tk2dSpriteCollectionData data = tk2dSpriteCollectionData.CreateFromTexture(
                atlasTexture,
                tk2dSpriteCollectionSize.PixelsPerMeter(100f),
                names.ToArray(),
                regions.ToArray(),
                anchors.ToArray());

            //Putting spritecollectindata in the right place in the hierarchy so it doesn't get destroyed
            if (AnimationManager.VMCSEAnimationCollections != null)
            {
                data.gameObject.transform.parent = AnimationManager.VMCSEAnimationCollections.transform;
            }

            //loading each animation
            foreach (string animationName in animationSpriteLists.Keys)
            {
                if (!animationData.ContainsKey(animationName)) { continue; }
                AnimationData animData = animationData[animationName];
                
                int frameCount = animationSpriteLists[animationName].Count;

                LoadSingleAnimationTo(library, animationName, data, animData, frameCount);
            }
        }

        private static void LoadSingleAnimationTo(tk2dSpriteAnimation library, string animationName, tk2dSpriteCollectionData data, AnimationData animData, int frameCount)
        {
            List<tk2dSpriteAnimationClip> list = library.clips.ToList<tk2dSpriteAnimationClip>();
            data.material.shader = HeroController.instance.GetComponent<MeshRenderer>().material.shader;

            //Setup frames
            tk2dSpriteAnimationFrame[] frameList = new tk2dSpriteAnimationFrame[frameCount];
            for (int i = 1; i < frameCount+1; i++)
            {
                int spriteId = data.GetSpriteIdByName(animationName + i, -1);
                if (spriteId == -1) { VesselMayCrySEPlugin.Instance.LogError($"Sprite {animationName + i} not found in collection data!"); continue; }

                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = data;
                frame.spriteId = spriteId;
                frameList[i-1] = frame;

                //add triggers
                if (animData.triggers != null && animData.triggers.ContainsKey(i.ToString()))
                {
                    frame.triggerEvent = true;
                    frame.eventInfo = animData.triggers[i.ToString()];
                }
            }

            tk2dSpriteAnimationClip.WrapMode wrapMode = animData.loop ? tk2dSpriteAnimationClip.WrapMode.Loop : tk2dSpriteAnimationClip.WrapMode.Once;
            //TODO: ADD SUPPORT FOR LOOPFROMFRAME


            //Setup animation clip
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = animationName;
            clip.fps = animData.fps;
            clip.frames = frameList;
            clip.wrapMode = wrapMode;
            clip.SetCollection(data);


            //Adding clip
            list.Add(clip);
            library.clips = list.ToArray();
            library.isValid = false; //to refresh the animation lookup
            library.ValidateLookup();
        }

        public static void Test()
        {
            LoadAtlasAnimationsTo(
                AnimationManager.GetBalrogAnimator().GetComponent<tk2dSpriteAnimation>(),
                "VesselMayCrySE.Resources.testatlas.png",
                "VesselMayCrySE.Resources.testatlas.json",
                "VesselMayCrySE.Resources.testanimdata.json"
            );
        }
    }
}
