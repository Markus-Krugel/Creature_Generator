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
        Rotate rotation;

        bool creatureGenerated = false;
        const int maxIterations = 15; 

        private void Awake()
        {
            lindenmayer = new LindenmayerSystem();
            executer = new CommandExecuter();       
        }

        private void Start()
        {
            metaballSystem = GetComponent("MetaballSystem") as MetaballSystem;
            settings = GetComponent("Settings") as Settings;
            rotation = GetComponent("Rotate") as Rotate;
            metaballSystem.ChangeIsoLevel(settings.isoLevel);
        }

        private void Update()
        {
            if(creatureGenerated)
                metaballSystem.UpdateSystem();
        }

        public void CreateCreature()
        {
            // Delete the metaballs of the previous creation
            if (creatureGenerated)
            {
                for (int i = 0; i < metaballSystem.metaballs.Count; i++)
                {
                    GameObject.Destroy(metaballSystem.metaballs[i].gameObject);          
                }

                metaballSystem.metaballs = metaballSystem.metaballs.Where(item => item != null).ToList();
            }

            // Create the ruleset for the creature generation
            lindenmayer.SetBodyPartsAmount(settings.amountHeads,settings.amountArms,settings.amountLegs);

            executer.SetCommandString(lindenmayer.RunSystem(maxIterations));
            executer.RunCommands();

            if(!creatureGenerated)
                metaballSystem.StartSystem();

            metaballSystem.CollectMetaballs();
            metaballSystem.UpdateSystem();

            creatureGenerated = true;
            rotation.active = true;
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
