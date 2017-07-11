using UnityEngine;
using System.Collections;

namespace ImprovedPerlinNoiseProject
{

    public enum NOISE_STLYE {  FBM = 0, TURBULENT = 1, RIDGED = 2 }

    public class GPUPerlinNoise
    {

        public Texture2D PermutationTable1D { get; private set; }

        public Texture2D PermutationTable2D { get; private set; }

        public Texture2D Gradient2D { get; private set; }

        public Texture2D Gradient3D { get; private set; }

        public Texture2D Gradient4D { get; private set; }

        private const int SIZE = 256;

        private int[] m_perm = new int[SIZE + SIZE];

        public GPUPerlinNoise(int seed)
        {
            Random.InitState(seed);

            int i, j, k;
            for (i = 0; i < SIZE; i++)
            {
                m_perm[i] = i;
            }

            while (--i != 0)
            {
                k = m_perm[i];
                j = Random.Range(0, SIZE);
                m_perm[i] = m_perm[j];
                m_perm[j] = k;
            }

            for (i = 0; i < SIZE; i++)
            {
                m_perm[SIZE + i] = m_perm[i];
            }

        }

        public void LoadResourcesFor2DNoise()
        {
            LoadPermTable1D();
            LoadGradient2D();
        }

        public void LoadResourcesFor3DNoise()
        {
            LoadPermTable2D();
            LoadGradient3D();
        }

        public void LoadResourcesFor4DNoise()
        {
            LoadPermTable1D();
            LoadPermTable2D();
            LoadGradient4D();
        }

        void LoadPermTable1D()
        {
            if (PermutationTable1D != null) return;

            PermutationTable1D = new Texture2D(SIZE, 1, TextureFormat.Alpha8, false, true);
            PermutationTable1D.filterMode = FilterMode.Point;
            PermutationTable1D.wrapMode = TextureWrapMode.Repeat;

            for (int x = 0; x < SIZE; x++)
            {
                PermutationTable1D.SetPixel(x, 1, new Color(0, 0, 0, (float)m_perm[x] / (float)(SIZE - 1)));
            }

            PermutationTable1D.Apply();
        }

        /// <summary>
        /// This is special table that has been optimesed for 3D noise.
        /// It can also be use in 4D noise for some optimisation but the 1D perm table is still needed 
        /// </summary>
        private void LoadPermTable2D()
        {
            if (PermutationTable2D) return;

            PermutationTable2D = new Texture2D(SIZE, SIZE, TextureFormat.ARGB32, false, true);
            PermutationTable2D.filterMode = FilterMode.Point;
            PermutationTable2D.wrapMode = TextureWrapMode.Repeat;

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    int A = m_perm[x] + y;
                    int AA = m_perm[A];
                    int AB = m_perm[A + 1];

                    int B = m_perm[x + 1] + y;
                    int BA = m_perm[B];
                    int BB = m_perm[B + 1];

                    PermutationTable2D.SetPixel(x, y, new Color((float)AA / 255.0f, (float)AB / 255.0f, (float)BA / 255.0f, (float)BB / 255.0f));
                }
            }

            PermutationTable2D.Apply();
        }

        private void LoadGradient2D()
        {
            if (Gradient2D) return;

            Gradient2D = new Texture2D(8, 1, TextureFormat.RGB24, false, true);
            Gradient2D.filterMode = FilterMode.Point;
            Gradient2D.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < 8; i++)
            {
                float R = (GRADIENT2[i * 2 + 0] + 1.0f) * 0.5f;
                float G = (GRADIENT2[i * 2 + 1] + 1.0f) * 0.5f;

                Gradient2D.SetPixel(i, 0, new Color(R, G, 0, 1));
            }

            Gradient2D.Apply();

        }

        private void LoadGradient3D()
        {
            if (Gradient3D) return;

            Gradient3D = new Texture2D(SIZE, 1, TextureFormat.RGB24, false, true);
            Gradient3D.filterMode = FilterMode.Point;
            Gradient3D.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < SIZE; i++)
            {
                int idx = m_perm[i] % 16;

                float R = (GRADIENT3[idx * 3 + 0] + 1.0f) * 0.5f;
                float G = (GRADIENT3[idx * 3 + 1] + 1.0f) * 0.5f;
                float B = (GRADIENT3[idx * 3 + 2] + 1.0f) * 0.5f;

                Gradient3D.SetPixel(i, 0, new Color(R, G, B, 1));
            }

            Gradient3D.Apply();

        }

        private void LoadGradient4D()
        {
            if (Gradient4D) return;

            Gradient4D = new Texture2D(SIZE, 1, TextureFormat.ARGB32, false, true);
            Gradient4D.filterMode = FilterMode.Point;
            Gradient4D.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < SIZE; i++)
            {
                int idx = m_perm[i] % 32;

                float R = (GRADIENT4[idx * 4 + 0] + 1.0f) * 0.5f;
                float G = (GRADIENT4[idx * 4 + 1] + 1.0f) * 0.5f;
                float B = (GRADIENT4[idx * 4 + 2] + 1.0f) * 0.5f;
                float A = (GRADIENT4[idx * 4 + 3] + 1.0f) * 0.5f;

                Gradient4D.SetPixel(i, 0, new Color(R, G, B, A));
            }

            Gradient4D.Apply();

        }

        private static float[] GRADIENT2 = new float[]
        {
        0, 1,
        1, 1,
        1, 0,
        1, -1,
        0, -1,
        -1, -1,
        -1, 0,
        -1, 1,
        };

        private static float[] GRADIENT3 = new float[]
        {
        1,1,0,
        -1,1,0,
        1,-1,0,
        -1,-1,0,
        1,0,1,
        -1,0,1,
        1,0,-1,
        -1,0,-1,
        0,1,1,
        0,-1,1,
        0,1,-1,
        0,-1,-1,
        1,1,0,
        0,-1,1,
        -1,1,0,
        0,-1,-1,
        };

        private static float[] GRADIENT4 = new float[]
        {
        0, -1, -1, -1,
        0, -1, -1, 1,
        0, -1, 1, -1,
        0, -1, 1, 1,
        0, 1, -1, -1,
        0, 1, -1, 1,
        0, 1, 1, -1,
        0, 1, 1, 1,
        -1, -1, 0, -1,
        -1, 1, 0, -1,
        1, -1, 0, -1,
        1, 1, 0, -1,
        -1, -1, 0, 1,
        -1, 1, 0, 1,
        1, -1, 0, 1,
        1, 1, 0, 1,

        -1, 0, -1, -1,
        1, 0, -1, -1,
        -1, 0, -1, 1,
        1, 0, -1, 1,
        -1, 0, 1, -1,
        1, 0, 1, -1,
        -1, 0, 1, 1,
        1, 0, 1, 1,
        0, -1, -1, 0,
        0, -1, -1, 0,
        0, -1, 1, 0,
        0, -1, 1, 0,
        0, 1, -1, 0,
        0, 1, -1, 0,
        0, 1, 1, 0,
        0, 1, 1, 0,
        };

    }

}













