using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public interface ICommand
    {

        Vector3 Execute(Vector3 vector);
    }
}
