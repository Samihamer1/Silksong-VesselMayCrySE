using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
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



        public static void LoadAnimationTo(tk2dSpriteAnimation library, string atlasPath, string atlasJsonPath, string animDataJsonPath)
        {
            Texture2D atlasTexture = ResourceLoader.LoadTexture2D(atlasPath);
            AtlasRoot? atlasData = ResourceLoader.LoadJSONFile<AtlasRoot>("VesselMayCrySE.Resources.testatlas.json");
            if (atlasData == null) { VesselMayCrySEPlugin.Instance.LogError($"JSON at {atlasJsonPath} not found."); return; }

            //Counting frames for automation purposes
            Dictionary<string, int> frameCounter = new();
            foreach (string key in atlasData.frames.Keys)
            {
                //TODO: perhaps create a class for animation data
                //Get the animation key here, and use it to search the animData json
                //add to names, regions, and anchors here. dont wait until after
                string[] splitKey = key.Split('/');
                string animationKey = splitKey[splitKey.Length - 2];

                // The Animation name will always be the penultimate directory, the last being the png itself.
                // eg.
                // RandomFolder1/../RandomFolder294/AnimationFolder/frame.png

                //Add to the frame count
                if (frameCounter.ContainsKey(animationKey))
                {
                    frameCounter[animationKey]++;
                }
                else
                {
                    frameCounter.Add(animationKey, 1);
                }
            }

            string animationLibraryName = "Test";
            string animationName = "RevolverEffect";
            int frameCount = 4;

            List<string> names = new();
            List<Rect> regions = new();
            List<Vector2> anchors = new();

            foreach (string animationName in frameCounter.Keys)
            {
                int frameCount = frameCounter[animationName];

                for (int i = 1; i < frameCount; i++)
                {
                    string frameName = animationName + i;
                }
            }


            for (int i = 1; i < frameCount + 1; i++)
            {
                string frameName = $"{animationLibraryName}/{animationName}/{i}.png";

                SpriteEntry entry = json.frames[frameName];

                if (entry == null) { continue; }
                Frame frame = entry.frame;

                names.Add(animationName + i);
                regions.Add(new Rect(frame.x, frame.y, frame.w, frame.h));

                //Region calculations for trimmed sprites
                Vector2 originalCenterPx = new Vector2(
                    entry.sourceSize.w * entry.pivot.x,
                    entry.sourceSize.h * entry.pivot.y
                );

                Vector2 offsetPx = new Vector2(
                    entry.spriteSourceSize.x,
                    entry.spriteSourceSize.y
                );

                Vector2 untrimmedPivotPx = originalCenterPx - offsetPx;

                anchors.Add(new Vector2(untrimmedPivotPx.x, untrimmedPivotPx.y));
            }

            tk2dSpriteCollectionData data = tk2dSpriteCollectionData.CreateFromTexture(
                atlasTexture,
                tk2dSpriteCollectionSize.PixelsPerMeter(100f),
                names.ToArray(),
                regions.ToArray(),
                anchors.ToArray());


            GameObject obj = AnimationManager.GetBalrogAnimator().gameObject;

            AnimationManager.LoadAnimationTo(obj, data, animationName, 5, tk2dSpriteAnimationClip.WrapMode.Loop, frameCount);
        }

        public static void Test()
        {
            //Sprite atlas = LoadAtlas("VesselMayCrySE.Resources.testatlas.png");

            Texture2D atlasTexture = ResourceLoader.LoadTexture2D("VesselMayCrySE.Resources.testatlas.png");


            AtlasRoot? json = ResourceLoader.LoadJSONFile<AtlasRoot>("VesselMayCrySE.Resources.testatlas.json");
            if (json == null) { VesselMayCrySEPlugin.Instance.LogError("JSON is null"); return; }

            Dictionary<string, int> frameCounter = new();

            foreach (string key in json.frames.Keys)
            {
                string[] splitKey = key.Split('/');
                string animationKey = splitKey[splitKey.Length - 2];

                // The Animation name will always be the penultimate directory, the last being the png itself.
                // eg.
                // RandomFolder1/../RandomFolder294/AnimationFolder/frame.png

                //Add to the frame count
                if (frameCounter.ContainsKey(animationKey)) {
                    frameCounter[animationKey]++;
                } else
                {
                    frameCounter.Add(animationKey, 1);
                }
            }


            VesselMayCrySEPlugin.Instance.log(frameCounter["RevolverEffect"].ToString());


            string animationLibraryName = "Test";
            string animationName = "RevolverEffect";
            int frameCount = 4;

            List<string> names = new();
            List<Rect> regions = new();
            List<Vector2> anchors = new();

            for (int i = 1; i < frameCount+1; i++)
            {
                string frameName = $"{animationLibraryName}/{animationName}/{i}.png";

                SpriteEntry entry = json.frames[frameName];

                if (entry == null) { continue; }
                Frame frame = entry.frame;

                names.Add(animationName + i);
                regions.Add(new Rect(frame.x, frame.y, frame.w, frame.h));

                //Region calculations for trimmed sprites
                Vector2 originalCenterPx = new Vector2(
                    entry.sourceSize.w * entry.pivot.x,
                    entry.sourceSize.h * entry.pivot.y
                );

                Vector2 offsetPx = new Vector2(
                    entry.spriteSourceSize.x,
                    entry.spriteSourceSize.y
                );

                Vector2 untrimmedPivotPx = originalCenterPx - offsetPx;

                anchors.Add(new Vector2(untrimmedPivotPx.x, untrimmedPivotPx.y));
            }

            tk2dSpriteCollectionData data = tk2dSpriteCollectionData.CreateFromTexture(
                atlasTexture,
                tk2dSpriteCollectionSize.PixelsPerMeter(100f),
                names.ToArray(),
                regions.ToArray(),
                anchors.ToArray());


            GameObject obj = AnimationManager.GetBalrogAnimator().gameObject;

            AnimationManager.LoadAnimationTo(obj, data, animationName, 5, tk2dSpriteAnimationClip.WrapMode.Loop, frameCount);
        }

        public static Sprite LoadAtlas(string path)
        {
            Texture2D texture = ResourceLoader.LoadTexture2D(path);
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.one / 2, 100.0f);
        }
    }
}
