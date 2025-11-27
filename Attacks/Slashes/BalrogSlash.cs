using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.Attacks.Slashes
{
    internal class BalrogSlash : StandardSlash
    {
        private Vector3 initialPosition;
        private Vector2 variance;
        private NailSlashTravel? travel;
        public void SetOffsetVariance(Vector2 variance)
        {
            NailSlashTravel travel = gameObject.GetComponent<NailSlashTravel>();
            if (travel == null) { VesselMayCrySEPlugin.Instance.LogError("BalrogSlash does not contain a NailSlashTravel to offset"); return; }

            this.travel = travel;
            initialPosition = gameObject.transform.localPosition;
            this.variance = variance;

            travel.slash.AttackStarting += VaryOffset;
        }

        private void VaryOffset()
        {
            if (travel == null) { return; }
            if (initialPosition == null) { return; }
            if (variance == null) { return; }

            float randX = UnityEngine.Random.Range(-1f, 1f);
            float randY = UnityEngine.Random.Range(-1f, 1f);

            Vector3 offset = new Vector2(variance.x * randX, variance.y * randY);
            travel.initialLocalPos = initialPosition + offset;
        }
    }
}
