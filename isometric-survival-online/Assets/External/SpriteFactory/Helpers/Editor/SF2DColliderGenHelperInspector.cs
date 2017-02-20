namespace SpriteFactory.Editor {

    [UnityEditor.CustomEditor(typeof(SpriteFactory.SF2DColliderGenHelper))]
    public class SF2DColliderGenHelperInspector : UnityEditor.Editor {

        private SpriteFactory.SF2DColliderGenHelper script;
        private UnityEngine.Component alphaMeshCollider = null;
        private bool initialized;

        private void OnEnable() {
            script = (SpriteFactory.SF2DColliderGenHelper)target;
            Init();
        }

        override public void OnInspectorGUI() {
            //DrawDefaultInspector();

            bool origGuiEnabled = UnityEngine.GUI.enabled; // store GUI state
            if(!initialized) UnityEngine.GUI.enabled = false;
            if(UnityEngine.GUILayout.Button("Rebuild 2D Collider Gen Collider")) {
                Rebuild2DCGCollider();
            }
            UnityEngine.GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void Init() {
            GetAlphaMeshCollider();
        }

        private void GetAlphaMeshCollider() {
            alphaMeshCollider = script.GetComponent("AlphaMeshCollider");
            if(alphaMeshCollider == null) {
                //Debug.LogWarning("No AlphaMeshCollider component was found on Sprite! SF2DCGHelper requires an AlphaMeshCollider component.");
                initialized = false;
                return;
            }
            initialized = true;
        }

        private void Rebuild2DCGCollider() {
            try {
                Init();
                if(!initialized) return;

                float ppu = GetPPU(); // get pixels per unit from GameSettings
                SpriteFactory.SF2DColliderGenHelper.Data data = script.CheckCollider(); // get the data we need from the helper script
                if(data == null) throw new System.Exception();


                UnityEngine.Component alphaMeshCollider = script.GetComponent("AlphaMeshCollider");
                if(alphaMeshCollider == null) throw new System.Exception("No AlphaMeshCollider component was found on Sprite! If you have removed the 2D Collider Gen collider, please remove the helper component");
                System.Type type_alphaMeshCollider = alphaMeshCollider.GetType();

                // Verify the 2D Collider Gen fields we need exist
                Verify2DCGFields(type_alphaMeshCollider);

                // Get the serialized objects
                UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(alphaMeshCollider);
                UnityEditor.SerializedProperty sp;
                so.Update();

                // CUSTOM TEX
                sp = so.FindProperty("mRegionIndependentParameters.mCustomTex");
                if(sp == null) FieldException("Required field not found!");

                UnityEngine.Texture2D tex = FindFrameTexture(data.masterSprite);
                if(tex == null) {
                    UnityEngine.Debug.LogWarning("Frame texture not found! 2D Collider Gen mesh collider will not generate correctly.");
                }
                sp.objectReferenceValue = tex;

                // CUSTOM SCALE
                sp = so.FindProperty("mRegionIndependentParameters.mCustomScale");
                if(sp == null) FieldException("Required field not found!");

                UnityEngine.Vector2 scale = data.pixelScale / ppu; // convert pixel scale to units
                sp.vector2Value = scale; // update scale

                // FLIP
                UnityEngine.Vector2 offset = data.unitOffset; // get offset which is affected by flip

                // FLIP X
                sp = so.FindProperty("mRegionIndependentParameters.mFlipHorizontal");
                if(sp == null) FieldException("Required field not found!");

                if(sp.boolValue != data.isFlippedX) sp.boolValue = data.isFlippedX;
                if(data.isFlippedX) {
                    offset.x *= -1.0f;
                }

                // FLIP Y
                sp = so.FindProperty("mRegionIndependentParameters.mFlipVertical");
                if(sp == null) FieldException("Required field not found!");

                if(sp.boolValue != data.isFlippedY) sp.boolValue = data.isFlippedY;
                if(data.isFlippedY) {
                    offset.y *= -1.0f;
                }

                // CUSTOM OFFSET
                sp = so.FindProperty("mRegionIndependentParameters.mCustomOffset");
                if(sp == null) FieldException("Required field not found!");

                sp.vector3Value = offset; // update offset

                // Finish up with the serialized object
                so.ApplyModifiedProperties();

                // Rebuild the 2DCG collider
                type_alphaMeshCollider.InvokeMember("RecalculateCollider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, alphaMeshCollider, null);

            } catch(System.Exception e) {
                UnityEngine.Debug.LogError(e.Message);
                UnityEngine.Debug.LogError("STACK TRACE: " + e.StackTrace);
            }
        }

        private void Verify2DCGFields(System.Type type_alphaMeshCollider) {
            System.Reflection.FieldInfo regionIndependentParameters = type_alphaMeshCollider.GetField("mRegionIndependentParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(regionIndependentParameters == null) FieldException("Required field not found!");

            System.Type type_rip = regionIndependentParameters.FieldType; // use .FieldType here because GetType returns MonoField instead of the proper type

            System.Reflection.FieldInfo fieldCustomTex = type_rip.GetField("mCustomTex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(fieldCustomTex == null) FieldException("Required field not found!");

            System.Reflection.FieldInfo fieldCustomScale = type_rip.GetField("mCustomScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(fieldCustomScale == null) FieldException("Required field not found!");

            System.Type type_fieldCustomScale = fieldCustomScale.FieldType;
            if(type_fieldCustomScale != typeof(UnityEngine.Vector2)) FieldException("Unexpected field type!");

            System.Reflection.FieldInfo fieldFlipH = type_rip.GetField("mFlipHorizontal", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if(fieldFlipH == null) FieldException("Required field not found!");

            System.Type type_fieldFlipH = fieldFlipH.FieldType;
            if(type_fieldFlipH != typeof(bool)) FieldException("Unexpected field type!");

            System.Reflection.FieldInfo fieldFlipV = type_rip.GetField("mFlipVertical", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if(fieldFlipV == null) FieldException("Required field not found!");

            System.Type type_fieldFlipV = fieldFlipV.FieldType;
            if(type_fieldFlipV != typeof(bool)) FieldException("Unexpected field type!");
        }

        private float GetPPU() {
            SpriteFactory.GameSettings gs = FileFinder.gameSettings;
            if(gs == null) throw new System.Exception("Sprite Factory GameSettings file not found!");
            return gs.pixelsPerUnit;
        }

        private UnityEngine.Texture2D FindFrameTexture(SpriteFactory.GameMasterSprite gameMasterSprite) {
            SpriteFactory.Editor.EditorMasterSpriteCore emsc = FileFinder.GetEditorMasterSprite(gameMasterSprite);
            if(emsc == null) throw new System.Exception("EditorMasterSprite could not be found!");

            SpriteFactory.Editor.EditorMasterSprite.Frame frame = emsc.data.GetEditorPreviewFrame();
            if(frame == null) return null; // no frame

            return frame.GetTexture();
        }

        public void FieldException(string msg) {
            throw new System.Exception(msg + " You may be using an incompatible version of 2D Collider Gen.");
        }

        private static class FileFinder {
            // THIS IS HACKED IN SO NO CHANGES HAD TO BE MADE TO SPRITE FACTORY
            // ONCE THIS IS FINALIZED, EXPOSE SOME FUNCTIONS IN THE EDITOR TO ALLOW THIS TO GET THE DATA WITHOUT
            // THIS HACK

            // Symbols
            private const string fileNameDelimiter = "~";
            private const string directoryDelimiter = "/";

            // File extensions
            private const string assetExtension = ".asset";

            // Folder names
            private const string topDirName = "SpriteFactory";
            private const string internalDirName = "Internal";
            private const string codeDirName = "Code";
            private const string editorDLLDirName = "Editor";
            private const string internalDataDirName = "Data";
            private const string saveAssetsDirName = "Assets";
            private const string spriteSaveDirName = "Sprites";
            private const string editorMasterSpriteSaveDirName = "Editor";
            private const string gameMasterSpriteSaveDirName = "Game";

            // Folder paths
            private const string unityAssetsPath = "Assets";
            private const string editorDLLRelPath = spriteEditorInternalPath + directoryDelimiter + codeDirName + directoryDelimiter + editorDLLDirName;
            private const string spriteEditorRoot = topDirName;
            private const string spriteEditorInternalPath = spriteEditorRoot + directoryDelimiter + internalDirName;
            private const string internalDataPath = spriteEditorInternalPath + directoryDelimiter + internalDataDirName;
            private const string saveDataPath = spriteEditorRoot + directoryDelimiter + "SaveData";

            // Asset folder paths
            private const string saveDataAssetsPath = saveDataPath + directoryDelimiter + saveAssetsDirName;
            private const string spriteSavePath = saveDataAssetsPath + directoryDelimiter + spriteSaveDirName;
            private const string editorMasterSpriteSavePath = spriteSavePath + directoryDelimiter + editorMasterSpriteSaveDirName;
            private const string gameMasterSpriteSavePath = spriteSavePath + directoryDelimiter + gameMasterSpriteSaveDirName;

            // File type identifiers
            private const string editorMasterSpriteTypeCode = "MS";
            private const string editorMasterSpriteCoreTypeCode = "MSC";
            private const string gameMasterSpriteTypeCode = "GMS";
            private const string spriteGroupTypeCode = "SG";

            // VARIABLES
            private static string _rootPath = null;
            private static SpriteFactory.GameSettings _gameSettings = null;

            // PROPERTIES
            public static string rootPath {
                get {
                    if(_rootPath != null && System.IO.File.Exists(_rootPath)) return _rootPath;

                    string[] files = System.IO.Directory.GetFiles(unityAssetsPath, "SpriteFactoryEditor.dll", System.IO.SearchOption.AllDirectories);
                    if(files == null || files.Length == 0) throw new System.Exception("Sprite Factory root path could not be found!");

                    string dllPath = ConvertBackslashes(files[0]);

                    string projPathToEditorDLL = ConvertBackslashes(System.IO.Path.GetDirectoryName(dllPath));
                    if(!projPathToEditorDLL.ToLower().EndsWith(editorDLLRelPath.ToLower())) throw new System.Exception("SpriteFactory directory structure has been modified! Failed to load Sprite Factory!");
                    _rootPath = projPathToEditorDLL.Substring(0, projPathToEditorDLL.Length - editorDLLRelPath.Length);
                    return _rootPath;
                }
            }
            public static SpriteFactory.GameSettings gameSettings {
                get {
                    if(_gameSettings != null) return _gameSettings;

                    string[] files = System.IO.Directory.GetFiles(unityAssetsPath, "GameSettings.asset", System.IO.SearchOption.AllDirectories);
                    if(files == null || files.Length == 0) return null;

                    for(int i = 0; i < files.Length; i++) {
                        string file = ConvertBackslashes(files[i]);
                        SpriteFactory.GameSettings settings = (SpriteFactory.GameSettings)UnityEditor.AssetDatabase.LoadAssetAtPath(file, typeof(SpriteFactory.GameSettings));
                        if(settings == null) continue;
                        _gameSettings = settings;
                    }

                    return _gameSettings;
                }
            }

            // METHODS
            public static SpriteFactory.Editor.EditorMasterSpriteCore GetEditorMasterSprite(SpriteFactory.GameMasterSprite gameMasterSprite) {
                string emsSavePath = rootPath + editorMasterSpriteSavePath;

                string gmsFilePath = UnityEditor.AssetDatabase.GetAssetPath(gameMasterSprite);
                string gmsFileName = System.IO.Path.GetFileNameWithoutExtension(gmsFilePath);

                // convert GMS file name to EMS file name
                string gmsFileNameL = gmsFileName.ToLower();
                string gmsTypeCodeL = gameMasterSpriteTypeCode.ToLower();
                string sgTypeCodeL = spriteGroupTypeCode.ToLower();
                string emscFileName;

                if(gmsFileNameL.StartsWith(gmsTypeCodeL)) { // this is an ungrouped master sprite
                    emscFileName = editorMasterSpriteTypeCode + gmsFileName.Substring(gameMasterSpriteTypeCode.Length, gmsFileName.Length - (gameMasterSpriteTypeCode.Length * 2)) + editorMasterSpriteCoreTypeCode; // strip start and end type codes replacing with ~MS~, end MSC
                } else if(gmsFileNameL.StartsWith(sgTypeCodeL)) { // this is a grouped master sprite
                    emscFileName = gmsFileName.Substring(0, gmsFileName.Length - gameMasterSpriteTypeCode.Length) + editorMasterSpriteCoreTypeCode; // strip off end and replace with ems core version
                    emscFileName.Replace(fileNameDelimiter + gameMasterSpriteTypeCode + fileNameDelimiter, fileNameDelimiter + editorMasterSpriteTypeCode + fileNameDelimiter); // replace middle ~GMS~ with ems version
                } else {
                    throw new System.Exception("Error trying to parse MasterSprite file name!");
                }

                string emscFullPath = emsSavePath + directoryDelimiter + emscFileName + assetExtension;
                if(!System.IO.File.Exists(emscFullPath)) return null;

                SpriteFactory.Editor.EditorMasterSpriteCore emsc = (SpriteFactory.Editor.EditorMasterSpriteCore)UnityEditor.AssetDatabase.LoadAssetAtPath(emscFullPath, typeof(SpriteFactory.Editor.EditorMasterSpriteCore));
                return emsc;
            }

            private static string ConvertBackslashes(string path) {
                if(path == null || path == "") return path;
                return path.Replace('\\', '/');
            }
        }
    }
}