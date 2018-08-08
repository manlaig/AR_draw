using UnityEngine;
using UnityEngine.XR.iOS;

namespace UnityEngine.XR.iOS
{
    public class UnityARUserAnchorComponent : MonoBehaviour
    {
        private string m_AnchorId;

        public string AnchorId { get { return m_AnchorId; } }

        void Start()
        {
            UnityARUserAnchorData anchor = UnityARSessionNativeInterface.GetARSessionNativeInterface().AddUserAnchorFromGameObject(this.gameObject);
            m_AnchorId = anchor.identifierStr;
            // the above line is creating a user anchor at the gameobject's position and rotation
            UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += RemoveAnchor;
        }

        // TODO: consider deleting the anchors when the game loades new worldmap because we want to see new drawing when loaded
        // to do that attach a function to the useranchordeleted event
        void RemoveAnchor(ARUserAnchor anchor)
        {
            if(anchor.identifier == m_AnchorId)
                Destroy(gameObject);
        }
    }
}