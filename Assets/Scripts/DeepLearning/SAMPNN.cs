using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Unity.Barracuda;


namespace DeepLearning
{
    public class SAMPNN : NeuralNetwork
    {
        public NNModel modelAsset;
        private Model m_RuntimeModel;
        private IWorker worker;
        Dictionary<string, Tensor> inputs = new Dictionary<string, Tensor>();

        public int x1_dim = 647;
        public int x2_dim = 2048;
        public int output_dim = 647;
        public int zDim = 64;

        private Tensor x1;
        private Tensor x2;
        private Tensor z;
        private float[] y;

        private bool verbose = false;

        int[,] Intervals;

        //Normalization data
        public string DataFolder = "Assets/NormData/MotionNet/";
        private Tensor x1mean, x2mean, x1std, x2std, Ymean, Ystd;

        protected override bool SetupDerived()
        {
            if (Setup)
            {
                return true;
            }
            LoadDerived();
            Setup = true;
            return true;
        }

        protected override bool ShutdownDerived()
        {
            if (Setup)
            {
                UnloadDerived();
                ResetPredictionTime();
                ResetPivot();
            }
            return false;
        }

        protected void LoadDerived()
        {
            m_RuntimeModel = ModelLoader.Load(modelAsset, verbose);
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RuntimeModel, verbose);

            x1 = new Tensor(1, x1_dim);
            x2 = new Tensor(1, x2_dim);
            z = new Tensor(1, zDim);

            y = new float[output_dim];

            x1mean = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, x1_dim }, TrainingUtils.ReadBinary(DataFolder + "x1mean.bin", x1_dim));
            x1std = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, x1_dim }, TrainingUtils.ReadBinary(DataFolder + "x1std.bin", x1_dim));

            x2mean = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, x2_dim }, TrainingUtils.ReadBinary(DataFolder + "x2mean.bin", x2_dim));
            x2std = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, x2_dim }, TrainingUtils.ReadBinary(DataFolder + "x2std.bin", x2_dim));

            Ymean = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, output_dim }, TrainingUtils.ReadBinary(DataFolder + "Ymean.bin", output_dim));
            Ystd = new Tensor(new int[] { 1, 1, 1, 1, 1, 1, 1, output_dim }, TrainingUtils.ReadBinary(DataFolder + "Ystd.bin", output_dim));

            Intervals = new[,] { { 0, x1_dim }, { x1_dim, x1_dim + x2_dim } };

        }

        protected void UnloadDerived()
        {

        }

        public void OnDestroy()
        {
            worker?.Dispose();
            foreach (var key in inputs.Keys)
            {
                inputs[key].Dispose();
            }
            inputs.Clear();
        }

        public void Normalize(ref Tensor X, Tensor Xmean, Tensor Xstd)
        {
            for (int i = 0; i < X.length; i++)
            {
                X[i] = (X[i] - Xmean[i]) / Xstd[i];
            }
        }

        public void UnNormalize(ref Tensor X, Tensor Xmean, Tensor Xstd)
        {
            for (int i = 0; i < X.length; i++)
            {
                X[i] = X[i] * Xstd[i] + Xmean[i];
            }
        }

        protected override void PredictDerived()
        {

            Normalize(ref x1, x1mean, x1std);
            Normalize(ref x2, x2mean, x2std);

            inputs["x1"] = x1;
            inputs["x2"] = x2;

            for (int i = 0; i < z.length; i++)
            {
                z[i] = RandomFromDistribution.RandomNormalDistribution(0.0f, 0.1f);
            }
            inputs["z"] = z;

            worker.Execute(inputs);
            Tensor output = worker.PeekOutput();
            UnNormalize(ref output, Ymean, Ystd);

            for (int i = 0; i < output.length; i++)
            {
                y[i] = output[i];
            }

        }

        public override void SetInput(int index, float value)
        {

            if (Setup)
            {

                if (index >= Intervals[0, 0] && index < Intervals[0, 1])
                {
                    x1[0, index - Intervals[0, 0]] = value;
                }
                else if (index >= Intervals[1, 0] && index < Intervals[1, 1])
                {
                    x2[0, index - Intervals[1, 0]] = value;
                }
                else
                {
                    throw new System.InvalidOperationException("Insertion exceded the allocated input size");
                }


            }
        }

        public override float GetOutput(int index)
        {
            if (Setup)
            {
                return y[index];
            }
            else
            {
                return 0f;
            }
        }

    }

}
