// This class exists to allow generating mouse events for some platforms
// but not for mobile platforms by using preprocessor directives.
// This is to avoid OnMouse event warnings when compiling for mobile platforms.

namespace SpriteFactory {

    [UnityEngine.AddComponentMenu("")]
    [UnityEngine.RequireComponent(typeof(SpriteFactory.SpriteCollider))]
    public class SpriteFactorySpriteColliderEventReporter : UnityEngine.MonoBehaviour {

        public SpriteFactory.SpriteCollider spriteCollider;

        // Mouse events are not generated for mobile devices
#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8

        // MOUSE EVENTS
        void OnMouseEnter() {
            spriteCollider.Send_MouseEnter();
        }

        void OnMouseOver() {
            spriteCollider.Send_MouseOver();
        }

        void OnMouseExit() {
            spriteCollider.Send_MouseExit();
        }

        void OnMouseDown() {
            spriteCollider.Send_MouseDown();
        }

        void OnMouseUpAsButton() {
            spriteCollider.Send_MouseUpAsButton();
        }

        void OnMouseDrag() {
            spriteCollider.Send_MouseDrag();
        }

#endif
    }
}
