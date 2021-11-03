using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateObjectsPrefabs : EditorWindow
{
    public static EditorWindow Window;
    public static Vector2 Scroll;

    bool Running = false;
    public bool ShapeNet = true;


    [MenuItem("AI4Animation/CreateObjectsPrefabs")]
    static void Init()
    {
        Window = EditorWindow.GetWindow(typeof(CreateObjectsPrefabs));
        Scroll = Vector3.zero;
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView(Scroll);


        if (Utility.GUIButton("Create Prefabs", Running ? UltiDraw.Cyan : UltiDraw.Grey, Running ? UltiDraw.Black : UltiDraw.LightGrey))
            {
                if (ShapeNet)
            {
                CreatePrefabsShapeNet();

            }
                else
            {

            }
                CreatePrefabs();
            }

        EditorGUILayout.EndScrollView();

    }

    Bounds Getbound(GameObject gameObject)
    {
        var rends = gameObject.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
            return new Bounds();
        var b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            Debug.Log(b.center);
            b.Encapsulate(rends[i].bounds);
        }
        return b;
    }
    
    void CreatePrefabs()
    {
        //Running = true;
        //string rootFolder = "Assets/Palette/PSObjects/models";
        //string PrefabsFolder = "Assets/Palette/PSObjects/prefabs";
       
        //string[] modelNames = { "armchair", "highstool", "table", "sofa" };
        //for (int j = 0; j < modelNames.Length; j++)
        //{
        //    string currModelPath = modelNames[j];
           
        //    var emptyParent = new GameObject(modelNames[j]);
        //    emptyParent.layer = 8;

        //    var model = Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(Path.Combine(rootFolder, modelNames[j] + ".obj")));
        //    model.name = "Model";
        //    model.layer = 8;
        //    model.transform.Rotate(model.transform.right, -90);
        //    Bounds bounds = Getbound(model);


        //    model.transform.position = new Vector3(0, -bounds.min.y, 0);
        //    model.transform.SetParent(emptyParent.transform);


        //    VoxelCollider voxelCollider = emptyParent.AddComponent<VoxelCollider>();
        //    voxelCollider.Resolution = 25;
        //    voxelCollider.Generate();

        //    Interaction interaction  = emptyParent.AddComponent<Interaction>();
        //    interaction.AddContact("Hips", Vector3.zero, Quaternion.identity);;
        //    interaction.ShowContacts = true;

        //    RescaleInfo info = emptyParent.AddComponent<RescaleInfo>();
        //    //info.Max = new Vector3(5, 5, 5);
        //    //info.Min = new Vector3(3, 3, 3);

        //    //emptyParent.transform.localScale = new Vector3(4, 4, 4);

        //    //var instance = (GameObject)PrefabUtility.InstantiatePrefab(emptyParent);

        //    PrefabUtility.SaveAsPrefabAsset(emptyParent, Path.Combine(PrefabsFolder, Path.GetFileName(modelNames[j]) + ".prefab"));

        //    DestroyImmediate(emptyParent);
        //    DestroyImmediate(model);

              
        //}
        

        //Running = false;
        //Debug.Log("Prefabs created");
    }

    void CreatePrefabsShapeNet()
    {
        Running = true;
        string rootFolder = "Assets/Palette/SAMP/models";
        string PrefabsFolder = "Assets/Palette/SAMP/prefabs";


        string[] categories = Directory.GetDirectories(rootFolder);
        for (int i = 0; i < categories.Length; i++)
        {
            Debug.Log("Category "+ categories[i]);
            //if (Path.GetFileName(categories[i]) != "SofasTest")
            //    continue;
            string currOutFolder = Path.Combine(PrefabsFolder, Path.GetFileName(categories[i]));
            if (!Directory.Exists(currOutFolder))
                Directory.CreateDirectory(currOutFolder);
            //categories[i] = Path.GetFileName(categories[i]);
            string[] modelNames = Directory.GetDirectories(categories[i]);
            for (int j = 0; j < modelNames.Length; j++)
            {
                string currModelPath = Path.Combine(modelNames[j], "models", "model_normalized.obj");
                if (File.Exists(currModelPath))
                {
                    var emptyParent = new GameObject(Path.GetFileName(modelNames[j]));
                    emptyParent.layer = 8;

                    var model = Instantiate((GameObject)AssetDatabase.LoadMainAssetAtPath(currModelPath));
                    model.name = "Model";
                    model.layer = 8;
                    Bounds bounds = Getbound(model);
                    model.transform.position = new Vector3(0, -bounds.min.y, 0);
                    model.transform.Rotate(model.transform.up, 180);
                    model.transform.SetParent(emptyParent.transform);


                    VoxelCollider voxelCollider = emptyParent.AddComponent<VoxelCollider>();
                    voxelCollider.Resolution = 25;
                    voxelCollider.Generate();

                    Interaction interaction = emptyParent.AddComponent<Interaction>();
                    interaction.AddContact("Hips", voxelCollider.GetCenter(), Quaternion.identity);
                    interaction.ShowContacts = true;

                    RescaleInfo info = emptyParent.AddComponent<RescaleInfo>();
                    info.Max = new Vector3(5, 5, 5);
                    info.Min = new Vector3(3, 3, 3);

                    emptyParent.transform.localScale = new Vector3(1, 1, 1);

                    //var instance = (GameObject)PrefabUtility.InstantiatePrefab(emptyParent);

                    PrefabUtility.SaveAsPrefabAsset(emptyParent, Path.Combine(currOutFolder, Path.GetFileName(modelNames[j]) + ".prefab"));

                    DestroyImmediate(emptyParent);
                    DestroyImmediate(model);

                }
                else
                {
                    Debug.Log("File " + currModelPath + " Does not Exists");
                }

            }
        }

        Running = false;
        Debug.Log("Prefabs created");
    }



}
