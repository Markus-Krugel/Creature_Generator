using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class Metaball : MonoBehaviour
    {
        private float posX, posY, posZ;
        public float power;

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
    }
}
