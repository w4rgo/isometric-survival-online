using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpriteFactory.Editor {

    [InitializeOnLoad]
    public static class EditorHelper {

        static EditorHelper() {
            EditorManager.externalEditorTools = new ExternalEditorTools();
        }

        private class ExternalEditorTools : IExternalEditorTools {

            public void FixTextureImporterSettings(string pathToTexture, ref bool deselected, ref List<string> texturesToImportPaths) {
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(pathToTexture);
                if(importer == null) return;

                // Check settings
                bool changed = false;

#if UNITY_3_5 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
                if(importer.textureType != TextureImporterType.Advanced) importer.textureType = TextureImporterType.Advanced;

                if(importer.DoesSourceTextureHaveAlpha()) { // alpha texture
                    if(importer.textureFormat != TextureImporterFormat.ARGB32) { // set format to ARGB32 so we don't deal with double compression
                        changed = true;
                        importer.textureFormat = TextureImporterFormat.ARGB32;
                    }
                } else { // non-alpha texture
                    if(importer.textureFormat != TextureImporterFormat.RGB24) { // set format to RGB24 so we don't deal with double compression
                        changed = true;
                        importer.textureFormat = TextureImporterFormat.RGB24;
                    }
                }
#elif UNITY_5_5_OR_NEWER
                if(importer.textureCompression != TextureImporterCompression.Uncompressed) {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }
#endif

                if(!importer.isReadable) { // make read/writeable
                    changed = true;
                    importer.isReadable = true;
                }
                if(importer.mipmapEnabled) { // make sure mip maps are disabled on the individual frames
                    changed = true;
                    importer.mipmapEnabled = false;
                }
                if(importer.npotScale != TextureImporterNPOTScale.None) {
                    changed = true;
                    importer.npotScale = TextureImporterNPOTScale.None;
                }
                if(importer.wrapMode != TextureWrapMode.Clamp) { // always clamp textures
                    changed = true;
                    importer.wrapMode = TextureWrapMode.Clamp;
                }
                if(importer.maxTextureSize != 4096) {
                    changed = true;
                    importer.maxTextureSize = 4096;
                }


#if UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5 || UNITY_5_3_OR_NEWER
                if(importer.DoesSourceTextureHaveAlpha() && !importer.alphaIsTransparency) {
                    importer.alphaIsTransparency = true;
                    changed = true;
                }
#endif

#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5 || UNITY_5_3_OR_NEWER
                if(importer.spriteImportMode != SpriteImportMode.None) {
                    importer.spriteImportMode = SpriteImportMode.None;
                    changed = true;
                }
#endif

                if(!changed) return; // don't bother updating the texture as nothing changed

                // Deselect the textures if selected or we'll get GUI drawing errors in the inspector
                if(!deselected) { // && Selection.Contains(tex)) { // don't check if selection contains texture anymore so we don't have to load the texture
                    Selection.objects = new Object[0]; // just deselect everything
                    deselected = true; // flag so we don't check again
                }

                EditorUtility.SetDirty(importer); // flag for saving
                Debug.Log("Texture importer settings on " + pathToTexture + " were changed for compatibility.");
                texturesToImportPaths.Add(pathToTexture); // store path so we can import later
            }
        }
    }
}