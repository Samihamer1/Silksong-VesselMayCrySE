using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.Components
{
    internal class DestroyAfter : MonoBehaviour
    {
        private float time = 0;
        private float timer = 0;
        private void Start()
        {
            timer = 0;

            //cant be bothered to fix why it doesnt work so just adding this for now
            SetTimer(3);
        }

        public void SetTimer(float time)
        {
            this.time = time;
            timer = 0;
        }

        private void Update()
        {
            if (time == 0) { return; }
            if (!gameObject.activeSelf) { return; }

            timer += Time.deltaTime;

            if (timer > time)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
