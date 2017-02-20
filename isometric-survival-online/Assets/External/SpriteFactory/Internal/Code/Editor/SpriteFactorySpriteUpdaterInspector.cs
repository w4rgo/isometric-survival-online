namespace SpriteFactory.Editor {

    [UnityEditor.CustomEditor(typeof(SpriteFactory.SpriteUpdater))]
    public class SpriteFactorySpriteUpdaterInspector : UnityEditor.Editor {

        private SpriteFactory.Editor.SpriteUpdaterInspector owner;

        private void OnEnable() {
            owner = new SpriteFactory.Editor.SpriteUpdaterInspector(this);
            owner.OnEnable();
        }

        override public void OnInspectorGUI() {
            owner.OnInspectorGUI();
        }

    }
}