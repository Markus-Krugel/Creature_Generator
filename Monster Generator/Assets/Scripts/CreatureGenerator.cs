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

        private void Awake()
        {
            lindenmayer = new LindenmayerSystem();
            executer = new CommandExecuter();       
        }

        private void Start()
        {
            metaballSystem = GetComponent("MetaballSystem") as MetaballSystem;
            settings = GetComponent("Settings") as Settings;
            metaballSystem.ChangeIsoLevel(settings.isoLevel);

            CreateCreature();
        }

        private void Update()
        {
            metaballSystem.UpdateSystem();
        }

        public void CreateCreature()
        {
            
            lindenmayer.CreateRuleset(settings.amountHeads,settings.amountLegs,settings.amountArms);

            executer.SetCommandString(lindenmayer.RunSystem("L", 3));
            executer.RunCommands();

            metaballSystem.StartSystem();
            metaballSystem.UpdateSystem();
        }

        public void SetLindenmayer(LindenmayerSystem lindenmayer)
        {
            this.lindenmayer = lindenmayer;
        }

        public void SetExecuter(CommandExecuter executer)
        {
            this.executer = executer;
        }

        public void ChangeIsoLevel(float isoLevel)
        {
            settings.ChangeIsoLevel(isoLevel);
            metaballSystem.ChangeIsoLevel(isoLevel);
        }
    }
}
