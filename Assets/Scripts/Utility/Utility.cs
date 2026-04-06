using System;
using UnityEngine;

namespace Utility
{
    public class DebugShower : MonoBehaviour
    {
        public Color DebugColor = Color.red;
        public float DebugSize = 0.2f;
        public bool OnlyShowOnSelected = false;
        public bool IsWireSphere = false;

        private void OnDrawGizmos()
        {
            if (!OnlyShowOnSelected)
            {
                Color prevColor = Gizmos.color;
                Gizmos.color = DebugColor;
                if (IsWireSphere) Gizmos.DrawWireSphere(transform.position, DebugSize);
                else Gizmos.DrawSphere(transform.position, DebugSize);
                Gizmos.color = prevColor;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (OnlyShowOnSelected)
            {
                Color prevColor = Gizmos.color;
                Gizmos.color = DebugColor;
                if (IsWireSphere) Gizmos.DrawWireSphere(transform.position, DebugSize);
                else Gizmos.DrawSphere(transform.position, DebugSize);
                Gizmos.color = prevColor;
            }
        }
    }
}