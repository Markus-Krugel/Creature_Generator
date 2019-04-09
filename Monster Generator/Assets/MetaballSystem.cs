using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    // http://paulbourke.net/geometry/polygonise/
    // https://github.com/Nesh108/Unity_MetaBalls_Liquids

    class MetaballSystem : MonoBehaviour
    {
        private Lattice lattice;

        // The list of all metaballs
        public List<Metaball> metaballs;

        public float isoLevel = 1.95f;

        Vector3[] vertices = new Vector3[300000], normals = new Vector3[300000];
        Vector2[] uv;
        int[] triangles;

        int trianglePointer = 0;
        int vertexPointer = 0;

        int frameCount = 0;

        // Stores all possible solutions
        private int[,] triTable = new int[,]
    {
            {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
            {3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
            {3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
            {3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
            {9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
            {9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
            {2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
            {8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
            {9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
            {4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
            {3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
            {1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
            {4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
            {4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
            {9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
            {5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
            {2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
            {9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
            {0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
            {2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
            {10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
            {4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
            {5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
            {5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
            {9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
            {0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
            {1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
            {10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
            {8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
            {2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
            {7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
            {9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
            {2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
            {11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
            {9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
            {5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
            {11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
            {11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
            {1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
            {9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
            {5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
            {2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
            {0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
            {5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
            {6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
            {3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
            {6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
            {5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
            {1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
            {10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
            {6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
            {8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
            {7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
            {3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
            {5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
            {0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
            {9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
            {8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
            {5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
            {0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
            {6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
            {10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},

            {10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
            {8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
            {1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
            {3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
            {0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
            {10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
            {3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
            {6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
            {9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
            {8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
            {3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
            {6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
            {0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
            {10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
            {10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
            {2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
            {7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
            {7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
            {2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
            {1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
            {11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
            {8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
            {0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
            {7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
            {10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
            {2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
            {6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
            {7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
            {2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
            {1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
            {10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
            {10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
            {0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
            {7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
            {6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
            {8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
            {9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
            {6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
            {4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
            {10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
            {8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
            {0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
            {1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},

            {8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
            {10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
            {4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
            {10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
            {5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
            {11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
            {9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
            {6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
            {7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
            {3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
            {7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
            {9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
            {3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
            {6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
            {9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
            {1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
            {4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
            {7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
            {6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
            {3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
            {0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
            {6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
            {0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
            {11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
            {6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
            {5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
            {9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
            {1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
            {1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
            {10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
            {0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
            {5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
            {10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
            {11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
            {9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
            {7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
            {2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
            {8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
            {9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
            {9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
            {1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
            {9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
            {9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
            {5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
            {0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
            {10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
            {2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
            {0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
            {0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
            {9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
            {5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
            {3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
            {5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
            {8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
            {0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
            {9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
            {0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
            {1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
            {3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
            {4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
            {9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
            {11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
            {11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
            {2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
            {9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
            {3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
            {1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
            {4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
            {4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
            {0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
            {3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
            {3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
            {0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
            {9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
            {1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
    };

        private int[] edgeTable = new int[]
        {
            0x0, 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
            0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
            0x190, 0x99, 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
            0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
            0x230, 0x339, 0x33, 0x13a, 0x636, 0x73f, 0x435, 0x53c,
            0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
            0x3a0, 0x2a9, 0x1a3, 0xaa, 0x7a6, 0x6af, 0x5a5, 0x4ac,
            0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
            0x460, 0x569, 0x663, 0x76a, 0x66, 0x16f, 0x265, 0x36c,
            0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
            0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff, 0x3f5, 0x2fc,
            0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
            0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55, 0x15c,
            0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
            0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc,
            0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
            0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
            0xcc, 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
            0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
            0x15c, 0x55, 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
            0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
            0x2fc, 0x3f5, 0xff, 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
            0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
            0x36c, 0x265, 0x16f, 0x66, 0x76a, 0x663, 0x569, 0x460,
            0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
            0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa, 0x1a3, 0x2a9, 0x3a0,
            0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
            0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33, 0x339, 0x230,
            0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
            0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99, 0x190,
            0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
            0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0
        };

        public void StartSystem()
        {
            metaballs = new List<Metaball>();

            // get the metaballs in the scene
            GameObject[] metaballObjects = GameObject.FindGameObjectsWithTag("Metaball");
            for (int i = 0; i < metaballObjects.Length; i++)
            {
                metaballs.Add(metaballObjects[i].GetComponent<Metaball>());
            }

            triangles = new int[1200000];
            lattice = new Lattice(30, 30, 30, this);            
        }

        public void UpdateSystem()
        {
            trianglePointer = 0;
            vertexPointer = 0;
            frameCount++;

            March();
            RenderMesh();
        }

        private void RenderMesh()
        {
            for (int i = 0; i < vertexPointer; i++)
            {
                Vector3 point = transform.TransformPoint(normals[i]).normalized;
                vertices[i].x = (point.x + 1f) * 0.5f;
                vertices[i].y = (point.y + 1f) * 0.5f;
            }

            Mesh mesh = ((MeshFilter)GetComponent("MeshFilter")).mesh;
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.normals = normals;
        }

        private void March()
        {
            int x, y, z;

            for (int i = 0; i < metaballs.Count; i++)
            {
                z = (int) ((metaballs[i].PosZ + 0.5f) * lattice.dimensionZ);
                x = (int) ((metaballs[i].PosX + 0.5f) * lattice.dimensionX);
                y = (int) ((metaballs[i].PosY + 0.5f) * lattice.dimensionY);

                while(z >= 0)
                {
                    LatticeCube cube = lattice.GetCube(x, y, z);
                    if (cube != null && cube.lastFrame < frameCount)
                    {
                        if (DoCube(cube))
                        {
                            RecurseCube(cube);
                            z = -1;
                        }

                        cube.lastFrame = frameCount;
                    }
                    else
                        z = -1;
                }
            }
        }

        private void RecurseCube(LatticeCube cube)
        {
            int x = cube.posX;
            int y = cube.posY;
            int z = cube.posZ;

            LatticeCube adjacent;

            adjacent = lattice.GetCube(x + 1, y, z);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }

            adjacent = lattice.GetCube(x - 1, y, z);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }

            adjacent = lattice.GetCube(x, y + 1, z);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }

            adjacent = lattice.GetCube(x, y - 1, z);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }

            adjacent = lattice.GetCube(x, y, z + 1);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }

            adjacent = lattice.GetCube(x, y, z - 1);
            if (adjacent != null && adjacent.lastFrame < frameCount)
            {
                adjacent.lastFrame = frameCount;
                if (DoCube(adjacent))
                {
                    RecurseCube(adjacent);
                }
            }
        }

        private bool DoCube(LatticeCube cube)
        {
            int cubeIndex = 0;

            // check if the intensity of the point is over the treshhold
            if (cube.pointArray[0].CalculateIntensity() > isoLevel)
                cubeIndex |= 1;
            if (cube.pointArray[1].CalculateIntensity() > isoLevel)
                cubeIndex |= 2;
            if (cube.pointArray[2].CalculateIntensity() > isoLevel)
                cubeIndex |= 4;
            if (cube.pointArray[3].CalculateIntensity() > isoLevel)
                cubeIndex |= 8;
            if (cube.pointArray[4].CalculateIntensity() > isoLevel)
                cubeIndex |= 16;
            if (cube.pointArray[5].CalculateIntensity() > isoLevel)
                cubeIndex |= 32;
            if (cube.pointArray[6].CalculateIntensity() > isoLevel)
                cubeIndex |= 64;
            if (cube.pointArray[7].CalculateIntensity() > isoLevel)
                cubeIndex |= 128;

            int edgeIndex = edgeTable[cubeIndex];

            if (edgeIndex != 0)
            {
                if ((edgeIndex & 1) > 0)
                {
                    GenerateEdge(cube, 0, 0, 1);
                }

                if ((edgeIndex & 2) > 0)
                {
                    GenerateEdge(cube, 1, 1, 2);
                }

                if ((edgeIndex & 4) > 0)
                {
                    GenerateEdge(cube, 2, 2, 3);
                }

                if ((edgeIndex & 0x8) > 0)
                {
                    GenerateEdge(cube, 3, 3, 0);
                }

                if ((edgeIndex & 0x10) > 0)
                {
                    GenerateEdge(cube, 4, 4, 5);
                }

                if ((edgeIndex & 0x20) > 0)
                {
                    GenerateEdge(cube, 5, 5, 6);
                }

                if ((edgeIndex & 0x40) > 0)
                {
                    GenerateEdge(cube, 6, 6, 7);
                }

                if ((edgeIndex & 0x80) > 0)
                {
                    GenerateEdge(cube, 7, 7, 4);
                }

                if ((edgeIndex & 0x100) > 0)
                {
                    GenerateEdge(cube, 8, 0, 4);
                }

                if ((edgeIndex & 0x200) > 0)
                {
                    GenerateEdge(cube, 9, 1, 5);
                }

                if ((edgeIndex & 0x400) > 0)
                {
                    GenerateEdge(cube, 10, 2, 6);
                }

                if ((edgeIndex & 0x800) > 0)
                {
                    GenerateEdge(cube, 11, 3, 7);
                }

                int temp;
                int triangleIndex = 0;
                while(triTable[cubeIndex, triangleIndex] != -1)
                {
                    temp = cube.edgeArray[triTable[cubeIndex, triangleIndex + 2]].vertexIndex;
                    triangles[trianglePointer++] = temp;

                    temp = cube.edgeArray[triTable[cubeIndex, triangleIndex + 1]].vertexIndex;
                    triangles[trianglePointer++] = temp;

                    temp = cube.edgeArray[triTable[cubeIndex, triangleIndex]].vertexIndex;
                    triangles[trianglePointer++] = temp;

                    triangleIndex += 3;
                }

                return true;
            }
            else
                return false;
        }

        private void GenerateEdge(LatticeCube cube, int edgeIndex, int pointIndex, int pointIndex2)
        {
            Vector3 vector;
            LatticeEdge edge = cube.edgeArray[edgeIndex];

            if (edge.lastFrame < frameCount)
            {
                vector = lattice.PositionOnAxis(cube.pointArray[pointIndex], cube.pointArray[pointIndex2], edge.axis);
                edge.position = vector;
                edge.vertexIndex = vertexPointer;
                normals[vertexPointer] = CalculateNormal(vector);
                normals[vertexPointer++] = vector;
                edge.lastFrame = frameCount;
            }
        }

        private Vector3 CalculateNormal(Vector3 point)
        {
            Vector3 result = new Vector3();

            for (int i = 0; i < metaballs.Count; i++)
            {
                Vector3 current = new Vector3();
                current.x = point.x - metaballs[i].PosX;
                current.y = point.y - metaballs[i].PosY;
                current.z = point.z - metaballs[i].PosZ;
                float magnitude = current.magnitude;
                float power = .5f * (1f / (magnitude * magnitude * magnitude) * metaballs[i].power);
                result = result + (current * power);
            }

            return result.normalized;
        }

        private class LatticeEdge
        {
            public Vector3 position;
            public int vertexIndex;
            public int axis;
            public int lastFrame;

            public LatticeEdge(int axis)
            {
                this.axis = axis;
                lastFrame = 0;
            }
        }

        private class LatticePoint
        {
            public float intensity;
            // unneccessary?
            // public float powerX, powerY, powerZ;

            /* This is being used to go through all metaballs
               so that you can calculate the intensity of this point */
            private MetaballSystem system;
            public int lastFrame;

            private float posX, posY, posZ; 

            public float PosX
            {
                get { return posX; }
                set { posX = value; }
            }

            public float PosY
            {
                get { return posY; }
                set { posY = value; }
            }

            public float PosZ
            {
                get { return posZ; }
                set { posZ = value; }
            }

            public LatticePoint(float x, float y, float z, MetaballSystem system)
            {
                PosX = x;
                PosY = y;
                PosZ = z;
                this.system = system;
                lastFrame = 0;
            }

            public float CalculateIntensity()
            {
                float power = 0;

                if (lastFrame < system.frameCount)
                {
                    lastFrame = system.frameCount;

                    for (int i = 0; i < system.metaballs.Count; i++)
                    {
                        float blobPosX = system.metaballs[i].PosX;
                        float blobPosY = system.metaballs[i].PosY;
                        float blobPosZ = system.metaballs[i].PosZ;
                        float blobPower = system.metaballs[i].power;

                        power += (1 / Mathf.Sqrt(((blobPosX - PosX) * (blobPosX - PosX)) +
                                                 ((blobPosY - PosY) * (blobPosY - PosY)) +
                                                 ((blobPosZ - PosZ) * (blobPosZ - PosZ)))) * blobPower;
                    } 
                }

                intensity = power;

                return intensity;
            }
        }

        private class LatticeCube
        {
            public LatticeEdge[] edgeArray;
            public LatticePoint[] pointArray;
            public int posX, posY, posZ, lastFrame;

            public LatticeCube()
            {
                edgeArray = new LatticeEdge[12];
                pointArray = new LatticePoint[8];
                lastFrame = 0;
            }
        }

        private class Lattice
        {
            // The dimension of the Lattice
            public int dimensionX, dimensionY, dimensionZ;

            private LatticePoint[] pointArray;
            private LatticeEdge[] edgeArray;
            private LatticeCube[] cubeArray;

            private MetaballSystem system;

            public Lattice(int dimensionX, int dimensionY, int dimensionZ, MetaballSystem system)
            {
                this.dimensionX = dimensionX;
                this.dimensionY = dimensionY;
                this.dimensionZ = dimensionZ;
                this.system = system;

                GenerateLattice();
            }

            public LatticeCube GetCube(int x, int y, int z)
            {
                // checks if the position is inside the lattice
                if (x < 0 || y < 0 || z < 0 || x >= dimensionX || y >= dimensionY || z >= dimensionZ)
                {
                    return null;
                }

                return cubeArray[z + (y * (dimensionZ)) + (x * (dimensionZ) * (dimensionZ))];
            }

            public LatticePoint GetPoint(int x, int y, int z)
            {
                // checks if the position is inside the lattice
                if (x < 0 || y < 0 || z < 0 || x >= dimensionX || y >= dimensionY || z >= dimensionZ)
                {
                    return null;
                }

                return pointArray[z + (y * (dimensionZ + 1)) + (x * (dimensionZ + 1) * (dimensionY + 1))];
            }

            public Vector3 PositionOnAxis(LatticePoint a, LatticePoint b, int axis)
            {
                float multiplicator = (system.isoLevel - a.CalculateIntensity()) / 
                    (b.CalculateIntensity() - a.CalculateIntensity());
                Vector3 temp = new Vector3(a.PosX, a.PosY, a.PosZ);

                switch (axis)
                {
                    case 0:
                        temp[axis] = a.PosX + (multiplicator * (b.PosX - a.PosX));
                    break;
                    case 1:
                        temp[axis] = a.PosY + (multiplicator * (b.PosY - a.PosY));
                        break;
                    case 2:
                        temp[axis] = a.PosZ + (multiplicator * (b.PosZ - a.PosZ));
                        break;
                }

                return temp;                
            }

            public void GenerateLattice()
            {
                // calculate the amount of cubes, points and edges based on the dimensions
                int pointArrayAmount = ((dimensionX + 1) * (dimensionY + 1) * (dimensionZ + 1));
                int cubeArrayAmount = dimensionX * dimensionY * dimensionZ;
                int edgeArrayAmountTotal = (cubeArrayAmount * 3) + ((2 * dimensionX * dimensionY) + (2 * dimensionX * dimensionZ) + (2 * dimensionY * dimensionZ)) +
                    dimensionX + dimensionY + dimensionZ;
                int edgeArrayAmountNow = edgeArrayAmountTotal + ((dimensionX * dimensionY) + (dimensionX * dimensionZ) + (dimensionY * dimensionZ)) * 2;

                cubeArray = new LatticeCube[cubeArrayAmount];
                pointArray = new LatticePoint[pointArrayAmount];
                edgeArray = new LatticeEdge[edgeArrayAmountNow];

                // create all edges
                for (int i = 0; i < edgeArrayAmountNow; i++)
                {
                    edgeArray[i] = new LatticeEdge(-1);
                }

                // create all points
                int pointIndex = 0;
                for (float i = 0; i < dimensionX; i++)
                {
                    for (float j = 0; j < dimensionY; j++)
                    {
                        for (float k = 0; k < dimensionZ; k++)
                        {
                            pointArray[pointIndex] = new LatticePoint((i / dimensionX) - 0.5f, (j / dimensionY) -0.5f, (k / dimensionZ) -0.5f, system);
                            pointIndex++;
                        }
                    }
                }

                // create all cubes
                for (int i = 0; i < cubeArrayAmount; i++)
                {
                    cubeArray[i] = new LatticeCube();
                }

                LatticeCube cube;
                LatticeCube cube2;
                int cubeIndex = 0;
                int edgeIndex = 0;

                for (int i = 0; i < dimensionX; i++)
                {
                    for (int j = 0; j < dimensionY; j++)
                    {
                        for (int k = 0; k < dimensionZ; k++)
                        {
                            cube = cubeArray[cubeIndex];

                            cubeIndex++;

                            // set the position of the cube
                            cube.posX = i;
                            cube.posY = j;
                            cube.posZ = k;

                            // set the points of the cube
                            LatticePoint[] points = cube.pointArray;
                            points[0] = GetPoint(i, j, k);
                            points[1] = GetPoint(i + 1, j, k);
                            points[2] = GetPoint(i + 1, j + 1, k);
                            points[3] = GetPoint(i, j + 1, k);
                            points[4] = GetPoint(i, j, k + 1);
                            points[5] = GetPoint(i + 1, j, k + 1);
                            points[6] = GetPoint(i + 1, j + 1, k + 1);
                            points[7] = GetPoint(i, j + 1, k + 1);

                            if (points[0] == null || points[1] == null || points[2] == null || points[3] == null || points[4] == null || points[5] == null || points[6] == null || points[7] == null)
                                Debug.Log("Problem");

                            LatticeEdge[] edges = cube.edgeArray;

                            // set the axis of the edges
                            edges[5] = edgeArray[edgeIndex++];
                            edges[5].axis = 1;
                            edges[6] = edgeArray[edgeIndex++];
                            edges[6].axis = 0;
                            edges[10] = edgeArray[edgeIndex++];
                            edges[10].axis = 2;

                            // checks adjacent cubes and set their edges
                            cube2 = GetCube(i + 1, j, k);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[11] = edges[10];
                                cube2.edgeArray[7] = edges[5];
                            }

                            cube2 = GetCube(i, j + 1, k);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[4] = cube.edgeArray[6];
                                cube2.edgeArray[9] = cube.edgeArray[10];
                            }

                            cube2 = GetCube(i, j + 1, k + 1);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[0] = cube.edgeArray[6];
                            }

                            cube2 = GetCube(i + 1, j, k + 1);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[3] = cube.edgeArray[5];
                            }

                            cube2 = GetCube(i + 1, j + 1, k);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[8] = cube.edgeArray[10];
                            }

                            cube2 = GetCube(i, j, k + 1);
                            if (cube2 != null)
                            {
                                cube2.edgeArray[1] = cube.edgeArray[5];
                                cube2.edgeArray[2] = cube.edgeArray[6];
                            }

                            // set the remaining edges axis
                            if (edges[0] == null)
                            {
                                edges[0] = edgeArray[edgeIndex++];
                                edges[0].axis = 0;
                            }

                            if (edges[1] == null)
                            {
                                edges[1] = edgeArray[edgeIndex++];
                                edges[1].axis = 1;
                            }

                            if (edges[2] == null)
                            {
                                edges[2] = edgeArray[edgeIndex++];
                                edges[2].axis = 0;
                            }

                            if (edges[3] == null)
                            {
                                edges[3] = edgeArray[edgeIndex++];
                                edges[3].axis = 1;
                            }

                            if (edges[4] == null)
                            {
                                edges[4] = edgeArray[edgeIndex++];
                                edges[4].axis = 0;
                            }

                            if (edges[7] == null)
                            {
                                edges[7] = edgeArray[edgeIndex++];
                                edges[7].axis = 1;
                            }

                            if (edges[8] == null)
                            {
                                edges[8] = edgeArray[edgeIndex++];
                                edges[8].axis = 2;
                            }

                            if (edges[9] == null)
                            {
                                edges[9] = edgeArray[edgeIndex++];
                                edges[9].axis = 2;
                            }

                            if (edges[11] == null)
                            {
                                edges[11] = edgeArray[edgeIndex++];
                                edges[11].axis = 2;
                            }
                        }
                    }
                }
            }
        }
    }
}
