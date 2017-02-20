namespace SpriteFactory {

    public class SF2DColliderGenHelper : UnityEngine.MonoBehaviour {

        private Data data;

        public Data CheckCollider() {
            if(data == null) data = new Data();

            // Get the Sprite component
            SpriteFactory.Sprite sprite = GetComponent<SpriteFactory.Sprite>();
            if(sprite == null) {
                UnityEngine.Debug.LogError("Sprite component not found!");
                return null;
            }

            // Get the Master Sprite
            SpriteFactory.GameMasterSprite masterSprite = sprite.masterSprite;
            if(masterSprite == null) {
                UnityEngine.Debug.LogError("MasterSprite not found!");
                return null;
            }

            // Get the editor frame
            SpriteFactory.Sprite.Frame frame = masterSprite.data.GetEditorPreviewFrame();
            if(frame == null) {
                UnityEngine.Debug.LogWarning(sprite.name + " has no frame data! Cannot regenrate mesh collider.");
                return null;
            }

            // Get the atlases
            SpriteFactory.Sprite.Atlas[] atlases = masterSprite.data.atlases;
            if(atlases == null || atlases.Length == 0) {
                UnityEngine.Debug.LogWarning(sprite.name + " has no atlases! Cannot regenrate mesh collider.");
                return null;
            }

            // Get the atlas for the editor frame
            SpriteFactory.Sprite.Atlas atlas = atlases[frame.atlasIndex];
            if(atlas == null) {
                UnityEngine.Debug.LogWarning(sprite.name + "'s atlas is missing! Cannot regenrate mesh collider.");
                return null;
            }

            // Read the atlas size, offset, and flipped data
            // PPU will be added in by the inspector since we have no access to it

            data.masterSprite = masterSprite;
            data.pixelScale = new UnityEngine.Vector2(atlas.width, atlas.height);
            data.unitOffset = new UnityEngine.Vector3(frame.frameOffset.x, frame.frameOffset.y, 0.0f);
            data.uvCoords = frame.uvCoords;
            data.atlasSize = atlas.width;
            data.isFlippedX = sprite.isFlippedX;
            data.isFlippedY = sprite.isFlippedY;

            return data;
        }

        public class Data {
            public SpriteFactory.GameMasterSprite masterSprite;
            public UnityEngine.Vector2 pixelScale;
            public UnityEngine.Vector3 unitOffset;
            public UnityEngine.Rect uvCoords;
            public float atlasSize;
            public bool isFlippedX;
            public bool isFlippedY;
        }
    }
}