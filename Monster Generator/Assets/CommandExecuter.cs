using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class CommandExecuter
    {
        string commandString;
        Vector3 position = Vector3.one;
        Quaternion rotation = Quaternion.identity;

        public CommandExecuter(string commandString)
        {
            this.commandString = commandString;
        }

        public void FillCommandDictionary()
        {
                
        }

        public void RunCommands()
        {
            for (int i = 0; i < commandString.Length; i++)
            {
                switch(commandString[i])
                {
                    case 'M':
                        SpawnMetaball(3);
                        break;
                    case 'P':
                        ChangePosition(1, 1, 1);
                        break;
                    case 'R':
                        ChangeRotation(5, 5, 5);
                        break;
                    case 'A':
                        AddRotation(30, 0, 0);
                        break;
                    case 'C':
                        AddPosition(1, 1, 1);
                        break;
                }
            }
        }   

        private void ChangeRotation(float x, float y, float z)
        {
            rotation.eulerAngles = new Vector3(x, y, z);
        }

        private void AddRotation(float x, float y, float z)
        {
            rotation.eulerAngles += new Vector3(x, y, z);
        }

        private void ChangePosition(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
        }

        private void AddPosition(float x, float y, float z)
        {
            position += new Vector3(x, y, z);
        }

        private void SpawnMetaball(float size)
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = position;
            
            // spawn metaball with rotation, position and size
        }
    }
}
