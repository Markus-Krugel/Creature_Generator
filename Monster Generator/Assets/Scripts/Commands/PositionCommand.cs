using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class PositionCommand : MonoBehaviour, ICommand
    {
        private Vector3 direction;

        public PositionCommand(Vector3 direction)
        {
            this.direction = direction;
        }

        public Vector3 Execute(Vector3 vector)
        {
            return ChangePosition(vector);
        }

        public Vector3 ChangePosition(Vector3 vector)
        {
            return vector + direction;
        }
    }
}
