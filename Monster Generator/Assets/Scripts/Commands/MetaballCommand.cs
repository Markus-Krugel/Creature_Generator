using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MetaballCommand : MonoBehaviour, ICommand
    {
        public GameObject metaballPrefab;

        public Vector3 Execute(Vector3 vector)
        {
            SpawnMetaball(vector);
            return new Vector3();
        }

        public void SpawnMetaball(Vector3 vector)
        {
            //GameObject metaball = Resources.Load<GameObject>("Metaball");
            //
            //Instantiate(metaball, vector, Quaternion.identity);
            //
            //Metaball values = metaball.GetComponent<Metaball>();
            //values.PosX = vector.x;
            //values.PosY = vector.y;
            //values.PosZ = vector.z;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = vector;
            cube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
    }
}
