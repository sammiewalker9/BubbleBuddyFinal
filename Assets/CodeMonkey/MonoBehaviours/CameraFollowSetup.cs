/* 
    ------------------- Code Monkey -------------------
    
    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
 
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.MonoBehaviours {

    /*
     * Easy set up for CameraFollow, it will follow the transform with zoom
     * */
    public class CameraFollowSetup : MonoBehaviour {

        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private Transform followTransform;
        [SerializeField] private float zoom;

        public Transform FollowTransform { get => followTransform; set => followTransform = value; }
        public Transform FollowTransform1 { get => followTransform; set => followTransform = value; }
        public CameraFollow CameraFollow { get => cameraFollow; set => cameraFollow = value; }
        public CameraFollow CameraFollow1 { get => cameraFollow; set => cameraFollow = value; }

        private void Start() {
            if (FollowTransform == null) {
                Debug.LogError("followTransform is null! Intended?");
                CameraFollow.Setup(() => Vector3.zero, () => zoom);
            } else {
                CameraFollow.Setup(() => FollowTransform.position, () => zoom);
            }
        }
    }

}