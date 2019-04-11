using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class RotationCommand : MonoBehaviour, ICommand
    {
        private Vector3 direction;

        public RotationCommand(Vector3 direction)
        {
            this.direction = direction;
        }

        public Vector3 Execute(Vector3 vector)
        {
            return ChangeRotation(vector);
        }

        public Vector3 ChangeRotation(Vector3 vector)
        {
            return vector + direction;
        }
    }
}
