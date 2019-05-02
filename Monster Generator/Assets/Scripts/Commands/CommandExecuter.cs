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
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        int maxXBodyPosition;
        int minXBodyPosition;
        int maxZBodyPosition;
        int minZBodyPosition;

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
            ResetPositionCommand resetAll = new ResetPositionCommand();

            /* Move left is P because L was already taken for the leg rule in the lindenmayer system
                and it would only complicate it more if both have same char
                Move backwards is N because B was already taken for the body rule in the lindenmayer system
                and it would only complicate it more if both have same char
            */
            commandDictionary.Add('P', moveLeft);
            commandDictionary.Add('M', metaball);
            commandDictionary.Add('R', moveRight);
            commandDictionary.Add('U', moveUp);
            commandDictionary.Add('D', moveDown);
            commandDictionary.Add('F', moveForward);
            commandDictionary.Add('N', moveBackwards);
            commandDictionary.Add('X', resetX);
            commandDictionary.Add('Y', resetY);
            commandDictionary.Add('Z', resetZ);
            commandDictionary.Add('Q', resetAll);
        }

        public void RunCommands()
        {
            // go through every char in the string
            for (int i = 0; i < commandString.Length; i++)
            {
                // if metaball command just spawn at the position
                if (commandString[i] == 'M')
                    commandDictionary[commandString[i]].Execute(position);
                // if reset command then reset the correct dimension
                else if(commandString[i] == 'X')
                    position.x = commandDictionary[commandString[i]].Execute(position).x;
                else if (commandString[i] == 'Y')
                    position.y = commandDictionary[commandString[i]].Execute(position).y;
                else if (commandString[i] == 'Z')
                    position.z = commandDictionary[commandString[i]].Execute(position).z;
                // else it is a move command or resetPosition and then adjust the position accordingly
                else 
                    position = commandDictionary[commandString[i]].Execute(position);
            }
        }   
    }
}
