using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class CommandExecuter : MonoBehaviour
    {
        string commandString;
        Vector3 position = new Vector3(0.1f, 0.1f, 0.1f);
        Quaternion rotation = Quaternion.identity;

        Dictionary<char, ICommand> commandDictionary = new Dictionary<char, ICommand>();

        public CommandExecuter()
        {
            FillCommandDictionary();
        }

        public CommandExecuter(string commandString)
        {
            this.commandString = commandString;
            FillCommandDictionary();
        }

        public void SetCommandString(string commandString)
        {
            this.commandString = commandString;
        }

        public void FillCommandDictionary()
        {
            MetaballCommand metaball = new MetaballCommand();
            PositionCommand moveLeft = new PositionCommand(new Vector3(-0.1f, 0, 0));
            PositionCommand moveRight = new PositionCommand(new Vector3(0.1f, 0, 0));
            PositionCommand moveUp = new PositionCommand(new Vector3(0, 0.1f, 0));
            PositionCommand moveDown = new PositionCommand(new Vector3(0, -0.1f, 0));
            PositionCommand moveForward = new PositionCommand(new Vector3(0, 0, 0.1f));
            PositionCommand moveBackwards = new PositionCommand(new Vector3(0, 0, -0.1f));
            ResetPositionCommand resetX = new ResetPositionCommand();
            ResetPositionCommand resetY = new ResetPositionCommand();
            ResetPositionCommand resetZ = new ResetPositionCommand();

            commandDictionary.Add('M', metaball);
            commandDictionary.Add('L', moveLeft);
            commandDictionary.Add('R', moveRight);
            commandDictionary.Add('U', moveUp);
            commandDictionary.Add('D', moveDown);
            commandDictionary.Add('F', moveForward);
            commandDictionary.Add('B', moveBackwards);
            commandDictionary.Add('X', resetX);
            commandDictionary.Add('Y', resetY);
            commandDictionary.Add('Z', resetZ);
        }

        public void RunCommands()
        {
            // go through every char in the string
            for (int i = 0; i < commandString.Length; i++)
            {
                // if metaball command just spawn at the position
                if(commandString[i] == 'M')
                    commandDictionary[commandString[i]].Execute(position);
                // if reset command then reset the correct dimension
                else if(commandString[i] == 'X')
                    position.x = commandDictionary[commandString[i]].Execute(position).x;
                else if (commandString[i] == 'Y')
                    position.y = commandDictionary[commandString[i]].Execute(position).y;
                else if (commandString[i] == 'Z')
                    position.z = commandDictionary[commandString[i]].Execute(position).z;
                // else it is a move command and then adjust the position accordingly
                else 
                    position = commandDictionary[commandString[i]].Execute(position);
            }
        }   
    }
}
