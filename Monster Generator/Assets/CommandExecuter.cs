using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class CommandExecuter
    {
        string commandString;
        Dictionary<char, Delegate> commandDictionary;

        public CommandExecuter(string commandString)
        {
            this.commandString = commandString;
        }

        public void FillCommandDictionary()
        {
            commandDictionary = new Dictionary<char, Delegate>();
        }

        public void RunCommands()
        {
            for (int i = 0; i < commandString.Length; i++)
            {
                Delegate action = null;

                if (commandDictionary.TryGetValue(commandString[i], out action))
                {
                    //action.Method.
                }
                else
                    throw new Exception("This character does not exist in the commandDictionary");
            }
        }
    }
}
