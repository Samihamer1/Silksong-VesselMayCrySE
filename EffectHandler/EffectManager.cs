using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.EffectHandler.Audio;
using VesselMayCrySE.EffectHandler.GameObjects;
using Object = UnityEngine.Object;

namespace VesselMayCrySE.EffectHandler
{
    /// <summary>
    /// Differing from the ResourceLoader, EffectManager handles miscellaneous storage and activation of 'effects'.
    /// This could include GameObjects, AudioClips, Particles, ect.
    /// It does not load any external files. This behaviour is always delegated to ResourceLoader.
    /// </summary>
    internal static class EffectManager
    {
        #region AudioClip names
        internal static class AudioClipNames
        {
            public static readonly string WITCHSLASH = "Witch Slash";
            public static readonly string SILKSPEARZAP = "Silkspear Zap";
            public static readonly string FROSTOVERLAY = "Frost Overlay";
            public static readonly string SHARPDASH = "Sharp Dash";
        }
        #endregion

        #region GameObject names
        internal static class GameObjectNames
        {
            public static readonly string HUNTERSTAB = "Hunter Stab";
            public static readonly string FROSTANTIC = "Frost Antic";
            public static readonly string FROSTSLASHEFFECT = "Frost Slash Effect";
            public static readonly string FROSTHITEFFECT = "Frost Hit Effect";
            public static readonly string THUNDERORB = "Thunder Orb";
            public static readonly string THUNDEREXPLOSION = "Thunder Explosion";
            public static readonly string ROUNDTRIP = "Round Trip";
        }
        #endregion


        #region Effect keys and values
        private static Dictionary<string, Type> effectDictionary = new Dictionary<string, Type>()
        {
            [AudioClipNames.SILKSPEARZAP] = typeof(SilkspearZapAudioEffect),
            [AudioClipNames.WITCHSLASH] = typeof(WitchSlashAudioEffect),
            [GameObjectNames.HUNTERSTAB] = typeof(HunterStabObjectEffect),
            [GameObjectNames.FROSTANTIC] = typeof(FrostAnticObjectEffect),
            [AudioClipNames.FROSTOVERLAY] = typeof(FrostOverlayAudioEffect),
            [GameObjectNames.FROSTSLASHEFFECT] = typeof(FrostSlashEffect),
            [GameObjectNames.FROSTHITEFFECT] = typeof(FrostHitEffect),
            [GameObjectNames.THUNDERORB] = typeof(ThunderOrbObjectEffect),
            [GameObjectNames.THUNDEREXPLOSION] = typeof(ThunderExplosionObjectEffect),
            [AudioClipNames.SHARPDASH] = typeof(SharpDashAudioEffect),
            [GameObjectNames.ROUNDTRIP] = typeof(RoundTripObjectEffect),
        };
        #endregion

        private static Dictionary<string, BaseEffect> storedEffects = new Dictionary<string, BaseEffect>();

        /// <summary>
        /// To be called whenever the Hero has fully loaded. Not to be used before this.
        /// </summary>
        public static void Initialise()
        {
            storedEffects.Clear();
            foreach (string key in effectDictionary.Keys)
            {
                Type t = effectDictionary[key];
                try
                {
                    var effect = Activator.CreateInstance(t);

                    storedEffects.Add(key, (BaseEffect)effect);
                }
                catch (Exception e)
                {
                    VesselMayCrySEPlugin.Instance.LogError($"Effect '{key}' unable to be created. Is the type correct?");
                    continue;
                }

            }
        }

        /// <summary>
        /// Attempts to get the Object stored under the given 'objectName'.
        /// objectName should be a value from one of the EffectManager.[x]Names static class.
        /// The type of the returned Object is determined by the static class you pick from.
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns>The Object if found, null if not</returns>
        public static Object? Get(string objectName)
        {
            if (storedEffects.ContainsKey(objectName))
            {
                Object? obj = storedEffects[objectName].GetObject();
                if (obj == null)
                {
                    VesselMayCrySEPlugin.Instance.LogError($"Object {objectName} found, but stored value is null.");
                }
                return obj;
            }
            VesselMayCrySEPlugin.Instance.LogError($"Object {objectName} not found.");
            return null;
        }
    }
}
