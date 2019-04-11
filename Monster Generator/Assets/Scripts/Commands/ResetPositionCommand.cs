using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class ResetPositionCommand : MonoBehaviour, ICommand
    {
        Vector3 position;

        public ResetPositionCommand()
        {
            position = new Vector3(0, 0, 0);
        }

        public Vector3 Execute(Vector3 vector)
        {
            return position;
        }
    }
}
