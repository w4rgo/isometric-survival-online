using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Movement;
using Object = UnityEngine.Object;


[Serializable]
public class DirectionAnimation
{
    [SerializeField] public string name;
    [SerializeField] public int frames;
    [SerializeField] public int framerate = 25;

    public DirectionAnimation(int frames)
    {
        this.frames = frames;
    }
}

class SpriteToAnimation : EditorWindow
{
    public Object source;
    private string basename;
    private string currentOrder;
    private int currentAnimIndex = 0;

    private List<Sprite> newSprites = new List<Sprite>();

    public DirectionAnimation[] DirectionAnimations = { };

    public Direction[] DirectionsOrder =
    {
        Direction.W, Direction.NW, Direction.N, Direction.NE, Direction.E, Direction.SE, Direction.S, Direction.SW
    };

    [MenuItem("Window/Sprite To Animation")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpriteToAnimation));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("8 directional animation creator");
        EditorGUILayout.EndHorizontal();
        CreateDropAreaElements();
        CreateDirectionLists();
        var framesCount = CalculateFramesCount();
        CreateAnimationFilesButton(framesCount);
        CreateAnimationController();
    }

    private void CreateAnimationController()
    {

        var animationControllerName = EditorGUILayout.TextField("Animator name","");
        if (GUILayout.Button("Create animation controller"))
        {
            var targetFolder = CreateDirectoryString();
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath (targetFolder+"/" + animationControllerName);


        }
    }

    private int CalculateFramesCount()
    {
        if (newSprites.Count > 0)
        {
            var framesCount = DirectionAnimations.Aggregate(
                (prod, next) => new DirectionAnimation(prod.frames + next.frames));
            EditorGUILayout.LabelField("Frames Count", framesCount.frames.ToString());
            return framesCount.frames;
        }
        return 0;
    }

    private void CreateAnimationFilesButton(int framesCount)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create animation files"))
        {
            if (newSprites.Count > 0)
            {
                if (newSprites.Count != framesCount * 8)
                {
                    ShowNotification(new GUIContent("You have a wrong number of frames for this configuration"));
                }
                else
                {
                    var targetFolder = CreateDirectoryString();

                    currentAnimIndex = 0;

                    foreach (var direction in DirectionsOrder)
                    {
                        foreach (var directionAnimation in DirectionAnimations)
                        {
                            CreateAnimationClipFile(directionAnimation, direction, targetFolder);
                        }
                    }
                }
            }
            else
            {
                ShowNotification(new GUIContent("You need to drag the group of sprites first"));
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private static string CreateDirectoryString()
    {
        var targetFolder = EditorUtility.OpenFolderPanel("Choose directory", "", "");
        var indexOfAssets = targetFolder.IndexOf("Assets");
        targetFolder = targetFolder.Substring(indexOfAssets);
        return targetFolder;
    }

    private void CreateAnimationClipFile(DirectionAnimation directionAnimation, Direction direction, string targetFolder)
    {
        var name = direction + "_" + directionAnimation.name;

        AnimationClip animClip = new AnimationClip();
        animClip.frameRate = directionAnimation.framerate; // FPS
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[directionAnimation.frames];

        for (int i = 0; i < directionAnimation.frames; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i;
            spriteKeyFrames[i].value = newSprites[currentAnimIndex + i];

        }

        currentAnimIndex+= directionAnimation.frames;
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

        AssetDatabase.CreateAsset(animClip, targetFolder+"/"+name+".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Trying to create : " + targetFolder+"/"+name+".anim");
    }

    private void CreateDirectionLists()
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty directionAnimations = so.FindProperty("DirectionAnimations");
        SerializedProperty directions = so.FindProperty("DirectionsOrder");
        EditorGUILayout.PropertyField(directionAnimations, true); // True means show children
        EditorGUILayout.PropertyField(directions, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties
    }

    private void CreateDropAreaElements()
    {
        EditorGUILayout.BeginHorizontal();
        DropAreaGUI();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current loaded sprites: " + newSprites.Count);
        if (GUILayout.Button("Clear sprites"))
        {
            newSprites.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }


    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 30.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag all the sprites here");
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Sprite dragged_object in DragAndDrop.objectReferences)
                    {
                        newSprites.Add(dragged_object);
                    }
                }
                break;
        }
    }
}