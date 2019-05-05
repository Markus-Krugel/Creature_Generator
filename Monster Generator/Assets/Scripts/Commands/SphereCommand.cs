using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class SphereCommand : MonoBehaviour, ICommand
    {
        Vector3 scale = new Vector3(0.07f, 0.07f, 0.07f);

        public Vector3 Execute(Vector3 vector)
        {
            SpawnSphere(vector);
            return new Vector3();
        }

        public void SpawnSphere(Vector3 vector)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = vector;
            sphere.transform.localScale = scale;
            sphere.tag = "Sphere";
        }
    }
}
