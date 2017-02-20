namespace SpriteFactory.Editor {

    [UnityEditor.CustomEditor(typeof(SpriteFactory.Sprite))]
    class SpriteFactorySpriteInspector : UnityEditor.Editor {

        private SpriteFactory.Editor.SpriteInspector owner;

        private void OnEnable() {
            owner = new SpriteFactory.Editor.SpriteInspector(this);
            owner.OnEnable();
        }

        override public void OnInspectorGUI() {
            owner.OnInspectorGUI();
        }
    }
}