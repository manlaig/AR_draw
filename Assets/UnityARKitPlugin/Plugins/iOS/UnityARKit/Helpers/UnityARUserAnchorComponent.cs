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
            DontDestroyOnLoad(this.gameObject);

            UnityARUserAnchorData anchor = UnityARSessionNativeInterface.GetARSessionNativeInterface().AddUserAnchorFromGameObject(this.gameObject);
            m_AnchorId = anchor.identifierStr;
            // the above line is creating a user anchor at the gameobject's position and rotation
        }
    }
}