namespace SpriteFactory {

    [UnityEngine.AddComponentMenu("")]
    [UnityEngine.RequireComponent(typeof(SpriteFactory.SpriteCamera))]
    [UnityEngine.ExecuteInEditMode]
    public class SpriteCameraGUIHelper : UnityEngine.MonoBehaviour {

        SpriteFactory.SpriteCamera spriteCamera;

        void Awake() {
            spriteCamera = GetComponent<SpriteFactory.SpriteCamera>();
        }

#if UNITY_EDITOR
        // Only call OnGUI in editor to prevent memory allocations from OnGUI in game builds

        void OnGUI() {
            spriteCamera.DrawOnGUI();
        }
#endif

    }
}