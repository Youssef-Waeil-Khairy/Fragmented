using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    public class DebugShower : MonoBehaviour
    {
        public Color DebugColor = Color.red;
        public float DebugSize = 0.2f;
        public bool OnlyShowOnSelected = false;
        public bool IsPointDisplayer = true;
        public bool IsWireSphere = false;
        public bool IsBoxCollider = false;
        public BoxCollider boxCollider = null;

        private void OnValidate()
        {
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            if (!OnlyShowOnSelected)
            {
                Color prevColor = Gizmos.color;
                Gizmos.color = DebugColor;
                if (IsPointDisplayer)
                {
                    if (IsWireSphere) Gizmos.DrawWireSphere(transform.position, DebugSize);
                    else Gizmos.DrawSphere(transform.position, DebugSize);
                }
                else if (IsBoxCollider)
                {
                    // ChatGPT fix start
                    // this fix from ChatGPT is to correctly preview the obstacle's positions along the line renderer
                    // Save original matrix
                    Matrix4x4 oldMatrix = Gizmos.matrix;

                    // Apply full transform (position, rotation, scale)
                    Gizmos.matrix = transform.localToWorldMatrix;

                    // Draw collider preview
                    Gizmos.DrawCube(boxCollider.center, boxCollider.size);

                    // Restore matrix
                    Gizmos.matrix = oldMatrix;
            
                    // ChatGPT fix end
                }
                Gizmos.color = prevColor;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (OnlyShowOnSelected)
            {
                Color prevColor = Gizmos.color;
                Gizmos.color = DebugColor;
                if (IsPointDisplayer)
                {
                    if (IsWireSphere) Gizmos.DrawWireSphere(transform.position, DebugSize);
                    else Gizmos.DrawSphere(transform.position, DebugSize);
                }
                else if (IsBoxCollider)
                {
                    // ChatGPT fix start
                    // this fix from ChatGPT is to correctly preview the obstacle's positions along the line renderer
                    // Save original matrix
                    Matrix4x4 oldMatrix = Gizmos.matrix;

                    // Apply full transform (position, rotation, scale)
                    Gizmos.matrix = transform.localToWorldMatrix;

                    // Draw collider preview
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

                    // Restore matrix
                    Gizmos.matrix = oldMatrix;
            
                    // ChatGPT fix end
                }
                Gizmos.color = prevColor;
            }
        }
    }
}