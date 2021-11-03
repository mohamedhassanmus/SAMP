#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class GoalNetExporter : EditorWindow
{

    public static EditorWindow Window;
    public static Vector2 Scroll;


    public bool ShowFiles = false;

    public string DataFolder = "Assets/Palette/SAMP/scenes";
    public string[] FilesNames = new string[] { };
    public bool[] Export = new bool[] { };
    public int PropResolution = 8;
    public int RandomSamples = 10;
    private int Index = -1;
    private float Progress = 0f;
    private float Performance = 0f;

    private int Start = 0;
    private int End = 0;

    private static bool Exporting = false;
    private static string Separator = " ";
    private static string Accuracy = "F5";

    [MenuItem("AI4Animation/GoalNetExporter")]
    static void Init()
    {
        Window = EditorWindow.GetWindow(typeof(GoalNetExporter));
        Scroll = Vector3.zero;
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView(Scroll);

        Utility.SetGUIColor(UltiDraw.Black);
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            Utility.ResetGUIColor();

            Utility.SetGUIColor(UltiDraw.Grey);
            using (new EditorGUILayout.VerticalScope("Box"))
            {
                Utility.ResetGUIColor();

                Utility.SetGUIColor(UltiDraw.Orange);
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    Utility.ResetGUIColor();
                    EditorGUILayout.LabelField("Exporter");
                }

                DataFolder = EditorGUILayout.TextField(DataFolder);
                RandomSamples = EditorGUILayout.IntField(RandomSamples);

                Utility.SetGUIColor(UltiDraw.White);
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    Utility.ResetGUIColor();
                    EditorGUILayout.LabelField("Export Path: " + GetExportPath());
                }

                Utility.SetGUIColor(UltiDraw.LightGrey);
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    Utility.ResetGUIColor();
                    Utility.SetGUIColor(UltiDraw.Cyan);
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        Utility.ResetGUIColor();
                        EditorGUILayout.LabelField("Files" + " [" + FilesNames.Length + "]");
                    }
                    ShowFiles = EditorGUILayout.Toggle("Show Files", ShowFiles);
                    if (FilesNames.Length == 0)
                    {
                        EditorGUILayout.LabelField("No files found.");
                    }
                    else
                    {
                        if (ShowFiles)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (Utility.GUIButton("Export All", UltiDraw.DarkGrey, UltiDraw.White))
                            {
                                for (int i = 0; i < FilesNames.Length; i++)
                                {

                                    Export[i] = true;

                                }
                            }
                            if (Utility.GUIButton("Export None", UltiDraw.DarkGrey, UltiDraw.White))
                            {
                                for (int i = 0; i < FilesNames.Length; i++)
                                {
                                    Export[i] = false;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            Start = EditorGUILayout.IntField("Start", Start);
                            End = EditorGUILayout.IntField("End", End);
                            if (Utility.GUIButton("Toggle", UltiDraw.DarkGrey, UltiDraw.White))
                            {
                                for (int i = Start - 1; i <= End - 1; i++)
                                {

                                    Export[i] = !Export[i];

                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            for (int i = 0; i < FilesNames.Length; i++)
                            {
                                Utility.SetGUIColor(Index == i ? UltiDraw.Cyan : Export[i] ? UltiDraw.Gold : UltiDraw.White);
                                using (new EditorGUILayout.VerticalScope("Box"))
                                {
                                    Utility.ResetGUIColor();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField((i + 1) + " - " + Path.GetFileNameWithoutExtension(FilesNames[i]), GUILayout.Width(200f));

                                    GUILayout.FlexibleSpace();
                                    if (Utility.GUIButton("O", Export[i] ? UltiDraw.DarkGreen : UltiDraw.DarkRed, UltiDraw.White, 50f))
                                    {
                                        Export[i] = !Export[i];
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                }



                if (!Exporting)
                {
                    if (Utility.GUIButton("Reload", UltiDraw.DarkGrey, UltiDraw.White))
                    {
                        Load();
                    }
                    if (Utility.GUIButton("Export Data", UltiDraw.DarkGrey, UltiDraw.White))
                    {
                        this.StartCoroutine(ExportDataSIGGRAPHAsia());
                    }
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, Progress * EditorGUILayout.GetControlRect().width, 25f), UltiDraw.Green.Transparent(0.75f));

                    EditorGUILayout.LabelField("Frames Per Second: " + Performance.ToString("F3"));

                    if (Utility.GUIButton("Stop", UltiDraw.DarkRed, UltiDraw.White))
                    {
                        Exporting = false;
                    }
                }
            }
        }


        EditorGUILayout.EndScrollView();
    }

    public void Load()
    {
        // Make this part a utility function
        string[] AssetsNames = AssetDatabase.FindAssets("t:Scene", new string[1] { DataFolder });
        FilesNames = new string[AssetsNames.Length];
        for (int i = 0; i < FilesNames.Length; i++)
        {
            FilesNames[i] = AssetDatabase.GUIDToAssetPath(AssetsNames[i]);
        }


        Export = new bool[FilesNames.Length];
        for (int i = 0; i < Export.Length; i++)
        {
            Export[i] = true;

        }

    }


    private StreamWriter CreateFile(string name)
    {
        string filename = string.Empty;
        string folder = Application.dataPath + "/../../ObjectExport/";
        if (!File.Exists(folder + name + ".txt"))
        {
            filename = folder + name;
        }
        else
        {
            int i = 1;
            while (File.Exists(folder + name + " (" + i + ").txt"))
            {
                i += 1;
            }
            filename = folder + name + " (" + i + ")";
        }
        return File.CreateText(filename + ".txt");
    }

    public CuboidMap GetInteractionGeometry(Interaction interaction)
    {

        CuboidMap sensor = new CuboidMap(new Vector3Int(PropResolution, PropResolution, PropResolution));
        sensor.Sense(interaction.GetCenter(), LayerMask.GetMask("Interaction"), interaction.GetExtents());
        return sensor;

    }


    public class Data
    {
        public StreamWriter File, Norm, Labels;

        public RunningStatistics[] Statistics = null;

        private Queue<float[]> Buffer = new Queue<float[]>();
        private Task Writer = null;

        private float[] Values = new float[0];
        private string[] Names = new string[0];
        private float[] Weights = new float[0];
        private int Dim = 0;

        private bool Finished = false;
        private bool Setup = false;

        public Data(StreamWriter file, StreamWriter norm, StreamWriter labels)
        {
            File = file;
            Norm = norm;
            Labels = labels;
            Writer = Task.Factory.StartNew(() => WriteData());
        }

        public int GetLength()
        {
            return Values.Length;
        }

        public void Feed(float value, string name, float weight = 1f)
        {
            if (!Setup)
            {
                ArrayExtensions.Add(ref Values, value);
                ArrayExtensions.Add(ref Names, name);
                ArrayExtensions.Add(ref Weights, weight);
            }
            else
            {
                Dim += 1;
                Values[Dim - 1] = value;
            }
        }

        public void Feed(float[] values, string name, float weight = 1f)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Feed(values[i], name + (i + 1), weight);
            }
        }

        public void Feed(bool[] values, string name, float weight = 1f)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Feed(values[i] ? 1f : 0f, name + (i + 1), weight);
            }
        }

        public void Feed(float[,] values, string name, float weight = 1f)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    Feed(values[i, j], name + (i * values.GetLength(1) + j + 1), weight);
                }
            }
        }

        public void Feed(bool[,] values, string name, float weight = 1f)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    Feed(values[i, j] ? 1f : 0f, name + (i * values.GetLength(1) + j + 1), weight);
                }
            }
        }

        public void Feed(Vector2 value, string name, float weight = 1f)
        {
            Feed(value.x, name + "X", weight);
            Feed(value.y, name + "Y", weight);
        }

        public void Feed(Vector3 value, string name, float weight = 1f)
        {
            Feed(value.x, name + "X", weight);
            Feed(value.y, name + "Y", weight);
            Feed(value.z, name + "Z", weight);
        }

        public void Feed(Quaternion value, string name, float weight = 1f)
        {
            Feed(value.x, name + "X", weight);
            Feed(value.y, name + "Y", weight);
            Feed(value.z, name + "Z", weight);
            Feed(value.w, name + "W", weight);

        }

        public void FeedXY(Vector3 value, string name, float weight = 1f)
        {
            Feed(value.x, name + "X", weight);
            Feed(value.y, name + "Y", weight);
        }

        public void FeedXZ(Vector3 value, string name, float weight = 1f)
        {
            Feed(value.x, name + "X", weight);
            Feed(value.z, name + "Z", weight);
        }

        public void FeedYZ(Vector3 value, string name, float weight = 1f)
        {
            Feed(value.y, name + "Y", weight);
            Feed(value.z, name + "Z", weight);
        }

        private void WriteData()
        {
            while (Exporting && (!Finished || Buffer.Count > 0))
            {
                if (Buffer.Count > 0)
                {
                    float[] item;
                    lock (Buffer)
                    {
                        item = Buffer.Dequeue();
                    }
                    //Update Mean and Std
                    for (int i = 0; i < item.Length; i++)
                    {
                        Statistics[i].Add(item[i]);
                    }
                    //Write to File
                    File.WriteLine(String.Join(Separator, Array.ConvertAll(item, x => x.ToString(Accuracy))));
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Store()
        {
            if (!Setup)
            {
                //Setup Mean and Std
                Statistics = new RunningStatistics[Values.Length];
                for (int i = 0; i < Statistics.Length; i++)
                {
                    Statistics[i] = new RunningStatistics();
                }

                //Write Labels
                for (int i = 0; i < Names.Length; i++)
                {
                    Labels.WriteLine("[" + i + "]" + " " + Names[i]);
                }
                Labels.Close();

                Setup = true;
            }

            //Enqueue Sample
            float[] item = (float[])Values.Clone();
            lock (Buffer)
            {
                Buffer.Enqueue(item);
            }

            //Reset Running Index
            Dim = 0;
        }

        public void Finish()
        {
            Finished = true;

            Task.WaitAll(Writer);

            File.Close();

            if (Setup)
            {
                //Write Mean
                float[] mean = new float[Statistics.Length];
                for (int i = 0; i < mean.Length; i++)
                {
                    mean[i] = Statistics[i].Mean();
                }
                Norm.WriteLine(String.Join(Separator, Array.ConvertAll(mean, x => x.ToString(Accuracy))));

                //Write Std
                float[] std = new float[Statistics.Length];
                for (int i = 0; i < std.Length; i++)
                {
                    std[i] = Statistics[i].Std();
                }
                Norm.WriteLine(String.Join(Separator, Array.ConvertAll(std, x => x.ToString(Accuracy))));
            }

            Norm.Close();
        }
    }

    [Serializable]
    public class LabelGroup
    {

        public string[] Labels;

        private int[] Indices;

        public LabelGroup(params string[] labels)
        {
            Labels = labels;
        }

        public string GetID()
        {
            string id = string.Empty;
            for (int i = 0; i < Labels.Length; i++)
            {
                id += Labels[i];
            }
            return id;
        }

        public void Setup(string[] references)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < references.Length; i++)
            {
                if (ArrayExtensions.Contains(ref Labels, references[i]))
                {
                    indices.Add(i);
                }
            }
            Indices = indices.ToArray();
        }

        public float Filter(float[] values)
        {
            float value = 0f;
            for (int i = 0; i < Indices.Length; i++)
            {
                value += values[Indices[i]];
            }
            if (value > 1f)
            {
                Debug.Log("Value larger than expected.");
            }
            return value;
        }

    }

    private string GetExportPath()
    {
        string path = Application.dataPath;
        path = path.Substring(0, path.LastIndexOf("/"));
        path = path.Substring(0, path.LastIndexOf("/"));
        path += "/ObjectExport";
        return path;
    }

    private IEnumerator ExportDataSIGGRAPHAsia()
    {

        Exporting = true;

        Progress = 0f;

        int total = 0;
        int items = 0;
        DateTime timestamp = Utility.GetTimestamp();

        Data X = new Data(CreateFile("Input"), CreateFile("InputNorm"), CreateFile("InputLabels"));
        Data Y = new Data(CreateFile("Output"), CreateFile("OutputNorm"), CreateFile("OutputLabels"));

        StreamWriter S = CreateFile("Sequences");


        for (int i = 0; i < FilesNames.Length; i++)
        {
            if (!Exporting)
            {
                break;
            }
            if (Export[i])
            {
                Index = i;

                var scene = EditorSceneManager.OpenScene(FilesNames[i], OpenSceneMode.Single);
                GameObject[] gameObjects = scene.GetRootGameObjects();

                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.SetActive(false);
                }

                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.SetActive(true);
                    if (gameObject == null || gameObject.name == "Main Camera" || gameObject.name == "Directional Light")
                    {
                        continue;
                    }
                    Interaction interaction = gameObject.GetComponent<Interaction>();
                    if (interaction == null)
                    {
                        continue;
                    }

                    Debug.Log("Exporting File: " + i + ", " + Path.GetFileNameWithoutExtension(FilesNames[i]));

                    //Scaling
                    Vector3 defaultScale = gameObject.transform.localScale;
                    RescaleInfo info = gameObject.GetComponent<RescaleInfo>();
                    int N = info == null ? 1 : RandomSamples;
                    for (int j = 0; j < N; j++)
                    {
                        gameObject.transform.localScale = info == null ? defaultScale : Utility.UniformVector3(info.Min, info.Max);

                        if (!Exporting)
                        {
                            break;
                        }

                        CuboidMap interactionGeometry = GetInteractionGeometry(interaction);

                        Matrix4x4 root = interaction.GetCenter();

                        //Input

                        //Interaction Geometry
                        for (int k = 0; k < interactionGeometry.Points.Length; k++)
                        {
                            X.Feed(interactionGeometry.References[k].GetRelativePositionTo(root), "InteractionPosition" + (k + 1));
                            X.Feed(interactionGeometry.Occupancies[k], "InteractionOccupancy" + (k + 1));
                        }

                        Y.Feed(interaction.GetContact("Hips").GetPosition().GetRelativePositionTo(root), "HipPosition");
                        Y.Feed(interaction.GetContact("Hips").GetForward().GetRelativeDirectionTo(root), "HipForward");

                        //Write Line
                        X.Store();
                        Y.Store();

                        total += 1;
                        items += 1;

                        Performance = items / (float)Utility.GetElapsedTime(timestamp);
                        timestamp = Utility.GetTimestamp();
                        items = 0;
                        Thread.Sleep(50);
                        yield return new WaitForSeconds(0f);

                    }
                    Progress = i / (FilesNames.Length * N);

                    gameObject.SetActive(false);
                }



            }


            ////Reset Progress
            //Progress = 0f;

            //Collect Garbage
            EditorUtility.UnloadUnusedAssetsImmediate();
            Resources.UnloadUnusedAssets();
            GC.Collect();


        }


        S.Close();

        X.Finish();
        Y.Finish();

        Index = -1;
        Exporting = false;
        yield return new WaitForSeconds(0f);

        Debug.Log("Exported " + total + " samples.");

    }






}
#endif