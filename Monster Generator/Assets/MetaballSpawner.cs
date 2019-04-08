using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class MetaballSpawner : MonoBehaviour {

        private bool spawned = false;
        public GameObject prefab;

        // Use this for initialization
        void Start() {

            
        }

        private void Awake()
        {
            if (!spawned)
            {
                for (int i = 0; i < 6; i++)
                {
                    Instantiate(prefab, new Vector3(0, i, 0), Quaternion.identity);

                    Metaball values = prefab.GetComponent<Metaball>();
                    values.PosX = 0;
                    values.PosY = i * 0.5f;
                    values.PosZ = 0;
                    values.power = 0.12f;
                }

                GetComponent<MetaballSystem>().StartSystem();
                GetComponent<MetaballSystem>().UpdateSystem();

                spawned = true;
            }
        }

        // Update is called once per frame
        void Update() {
            GetComponent<MetaballSystem>().UpdateSystem();
        }
    }
}
