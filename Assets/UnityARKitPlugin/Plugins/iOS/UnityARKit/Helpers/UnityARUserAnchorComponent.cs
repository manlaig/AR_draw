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
            // creating a user anchor at the gameobject's position and rotation
            UnityARUserAnchorData anchor = UnityARSessionNativeInterface.GetARSessionNativeInterface().AddUserAnchorFromGameObject(this.gameObject);
            m_AnchorId = anchor.identifierStr;
            
            //UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += RemoveAnchor;
        }

        /*
        void RemoveAnchor(ARUserAnchor anchor)
        {
            if(anchor.identifier == m_AnchorId)
                Debug.Log("Deleting user anchor");
                //Destroy(gameObject);
        }
        */
    }
}