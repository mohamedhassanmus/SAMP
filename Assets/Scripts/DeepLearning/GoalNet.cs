using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;


public class GoalNet : MonoBehaviour
{
    public NNModel modelAsset;

    private Model m_RuntimeModel;
    private IWorker worker;

    private bool verbose = false;

    int Xsize = 2048;
    public int Ysize = 6;
    public int Zsize = 3;

    Tensor X, Z;
    Tensor Xmean, Xstd, Ymean, Ystd;

    int PropResolution = 8;
    public string DataFolder = "Assets/NormData/GoalNet/";

    void Start()
    {
        if (modelAsset != null)
        {
            X = new Tensor(1, 2048);
            m_RuntimeModel = ModelLoader.Load(modelAsset, verbose);
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, m_RuntimeModel, verbose);

            Xmean = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, Xsize }, TrainingUtils.ReadBinary(DataFolder + "Xmean.bin", Xsize));
            Xstd = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, Xsize }, TrainingUtils.ReadBinary(DataFolder + "Xstd.bin", Xsize));

            Ymean = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, Ysize }, TrainingUtils.ReadBinary(DataFolder + "Ymean.bin", Ysize));
            Ystd = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, Ysize }, TrainingUtils.ReadBinary(DataFolder + "Ystd.bin", Ysize));

            Z = new Tensor(1, Zsize);

        }

    }

    public CuboidMap GetInteractionGeometry(Interaction interaction)
    {
        CuboidMap sensor = new CuboidMap(new Vector3Int(PropResolution, PropResolution, PropResolution));
        sensor.Sense(interaction.GetCenter(), LayerMask.GetMask("Interaction"), interaction.GetExtents());
        return sensor;
    }

    public void PredictGoal(Interaction interaction, string name)
    {
        if (name != null)
        {
            if (interaction.GetContactTransform("Hips_Pred") != null)
                interaction.RemoveContact();
        }

        Matrix4x4 root = interaction.GetCenter();

        //Prepare input
        CuboidMap interactionGeometry = GetInteractionGeometry(interaction);
        int i = 0;
        for (int k = 0; k < interactionGeometry.Points.Length; k++)
        {
            var pos = interactionGeometry.References[k].GetRelativePositionTo(root);
            X[i] = pos.x;
            X[i + 1] = pos.y;
            X[i + 2] = pos.z;
            X[i + 3] = interactionGeometry.Occupancies[k];
            i += 4;
        }

        // Normalize input
        TrainingUtils.Normalize(ref X, Xmean, Xstd);

        //Run model
        var inputs = new Dictionary<string, Tensor>();
        for (int k = 0; k < Z.length; k++)
        {
            Z[k] = RandomFromDistribution.RandomNormalDistribution(0.0f, 1.0f);
        }
        inputs["Z"] = Z;
        inputs["Cond"] = X;
        worker.Execute(inputs);

        // Get output
        Tensor output = worker.PeekOutput();
        //Normalize output
        TrainingUtils.UnNormalize(ref output, Ymean, Ystd);

        //Parse output
        var hip_pos = new Vector3(output[0], output[1], output[2]);
        hip_pos = hip_pos.GetRelativePositionFrom(root);
        var hip_forward = new Vector3(output[3], output[4], output[5]);
        hip_forward = hip_forward.GetRelativeDirectionFrom(root);
        var hip_rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(hip_forward, Vector3.up).normalized, Vector3.up);

        if (name != null) { interaction.AddContact("Hips_Pred", hip_pos, hip_rot); }
        else { interaction.AddContact(hip_pos, hip_rot); }
    }


    public void OnDestroy()
    {
        worker?.Dispose();
        Debug.Log("Destory being called");
        X.Dispose();
    }


}
