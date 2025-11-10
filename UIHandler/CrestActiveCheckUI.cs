using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class CrestActiveCheckUI : MonoBehaviour
    {
        private InventoryToolCrestList? inventoryToolCrestList;
        private InventoryItemToolManager? manager;
        private void Awake()
        {
            if (DevilMenuUI.Instance == null) { return; }
            manager = DevilMenuUI.Instance.inventoryItemToolManager;
            if (manager == null) { return; }
            inventoryToolCrestList = DevilMenuUI.Instance.inventoryItemToolManager.crestList;
        }

        private void Update()
        {            
            if (inventoryToolCrestList == null) { return; }
            if (manager == null) { return; }

            if (!manager.gameObject.activeSelf) { return; }

            //only do this if tools page is open

            bool state = inventoryToolCrestList.CurrentCrest.name == "Devil";

            gameObject.SetActiveChildren(state);
        }

    }
}
