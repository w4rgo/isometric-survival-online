using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEditor.Animations;
using Object = UnityEngine.Object;


[Serializable]
public class DirectionAnimation
{
    [SerializeField] public string name;
    [SerializeField] public int frames;
    [SerializeField] public int framerate = 25;
    [SerializeField] public bool idle;
    [SerializeField] public bool running;

    public DirectionAnimation(int frames)
    {
        this.frames = frames;
    }
}


public class DirectionThresholds
{
    public float prev;
    public float next;

    public DirectionThresholds(float prev, float next)
    {
        this.prev = prev;
        this.next = next;
    }

    public static DirectionThresholds CalculateThreesholds(Direction direction)
    {
        switch (direction)
        {
            case Direction.N:
                return new DirectionThresholds(337.5f, 22.5f);
            case Direction.NE:
                return new DirectionThresholds(22.5f, 67.5f);
            case Direction.E:
                return new DirectionThresholds(67.5f , 112.5f);
            case Direction.SE:
                return new DirectionThresholds(112.5f, 157.5f);
            case Direction.S:
                return new DirectionThresholds(157.5f, 202.5f);
            case Direction.SW:
                return new DirectionThresholds(202.5f, 247.5f);
            case Direction.W:
                return new DirectionThresholds(247.5f, 292.5f);
            case Direction.NW:
                return new DirectionThresholds(292.5f, 337.5f);
        }
        return new DirectionThresholds(0f,0f);
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

    private string animationControllerName;

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
        animationControllerName = EditorGUILayout.TextField("Animator name", animationControllerName);
        if (GUILayout.Button("Create animation controller"))
        {
            var targetFolder = CreateDirectoryString();
            var path = targetFolder + "/" + animationControllerName + ".controller";
            Debug.Log(path);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            controller.AddParameter("direction", AnimatorControllerParameterType.Float);
            controller.AddParameter("speed", AnimatorControllerParameterType.Float);

            foreach (var directionAnimation in DirectionAnimations)
            {
                controller.AddParameter(directionAnimation.name, AnimatorControllerParameterType.Trigger);
            }

            var rootStateMachine = controller.layers[0].stateMachine;

            foreach (var direction in DirectionsOrder)
            {
                var currentStateMachine = rootStateMachine.AddStateMachine(direction.ToString());

                var directionTransition = rootStateMachine.AddAnyStateTransition(currentStateMachine);
                directionTransition.hasExitTime = false;
                var prev = DirectionThresholds.CalculateThreesholds(direction).prev;
                var next = DirectionThresholds.CalculateThreesholds(direction).next;

                if (direction == Direction.N)
                {
                    directionTransition.AddCondition(AnimatorConditionMode.Less, prev, "direction");
                    directionTransition.AddCondition(AnimatorConditionMode.Less, next, "direction");
                }
                else
                {
                    directionTransition.AddCondition(AnimatorConditionMode.Greater, prev, "direction");
                    directionTransition.AddCondition(AnimatorConditionMode.Less, next, "direction");
                }

                foreach (var directionAnimation in DirectionAnimations)
                {
                    var name = direction + "_" + directionAnimation.name;
                    var currentState = currentStateMachine.AddState(name);

                    if (directionAnimation.idle)
                    {
                        var idleTransition = rootStateMachine.AddAnyStateTransition(currentState);
                        idleTransition.AddCondition(AnimatorConditionMode.Less, 0.001f, "speed");
                    } else if (directionAnimation.running)
                    {
                        var runningTransition = rootStateMachine.AddAnyStateTransition(currentState);
                        runningTransition.AddCondition(AnimatorConditionMode.Greater, 0.001f, "speed");
                    }
                    else
                    {
                        var triggerTransition = rootStateMachine.AddAnyStateTransition(currentState);
                        triggerTransition.AddCondition(AnimatorConditionMode.Less, 0.001f, directionAnimation.name);
                    }



                }
            }
        }
    }

    private int CalculateFramesCount()
    {
        if (DirectionAnimations.Length > 0)
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

    private void CreateAnimationClipFile(DirectionAnimation directionAnimation, Direction direction,
        string targetFolder)
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

        currentAnimIndex += directionAnimation.frames;
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

        AssetDatabase.CreateAsset(animClip, targetFolder + "/" + name + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Trying to create : " + targetFolder + "/" + name + ".anim");
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
        so.Update();
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