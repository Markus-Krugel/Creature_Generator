using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class CreatureGenerator : MonoBehaviour
    {
        CommandExecuter executer;
        LindenmayerSystem lindenmayer;

        public void SetLindenmayer(LindenmayerSystem lindenmayer)
        {
            this.lindenmayer = lindenmayer;
        }

        private void Awake()
        {
            CreateCreature();
        }

        public void CreateCreature()
        {
            lindenmayer = new LindenmayerSystem();
            lindenmayer.CreateRuleset(1, 2, 2);

            executer = new CommandExecuter(lindenmayer.RunSystem("B", 3));
            executer.RunCommands();
        }
    }
}
