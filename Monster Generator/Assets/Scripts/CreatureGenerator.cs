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
        MetaballSystem metaballSystem;
        Settings settings;

        public void SetLindenmayer(LindenmayerSystem lindenmayer)
        {
            this.lindenmayer = lindenmayer;
        }

        private void Awake()
        {
            CreateCreature();
        }

        private void Update()
        {
            metaballSystem.UpdateSystem();
        }

        public void CreateCreature()
        {
            lindenmayer = new LindenmayerSystem();
            lindenmayer.CreateRuleset(1, 2, 2);

            executer = new CommandExecuter(lindenmayer.RunSystem("B", 3));
            executer.RunCommands();

            metaballSystem.StartSystem();
            metaballSystem.UpdateSystem();
        }
    }
}
