using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VesselMayCrySE.EffectHandler
{
    internal abstract class BaseEffect
    {
        internal Object? storedObject;

        /// <summary>
        /// To be overridden.
        /// Attempts to find the necessary Object to store.
        /// </summary>
        /// <returns>The Object to be stored</returns>
        public abstract Object? TryInitialiseObject();

        public BaseEffect()
        {
            storedObject = TryInitialiseObject();
        }

        public Object? GetObject()
        {
            return storedObject;
        }
    }
}
