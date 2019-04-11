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
            float metaballsAmount = 6;

            if (!spawned)
            {
                for (int i = 0; i < metaballsAmount; i++)
                {
                    Vector3 position = new Vector3((i / 10.0f) + 0.1f, (i / metaballsAmount) + 0.1f, (i / 5.0f) + 0.1f);
                    GameObject metaball = Instantiate(prefab, position, Quaternion.identity);

                    Metaball values = metaball.GetComponent<Metaball>();
                    values.PosX = position.x;
                    values.PosY = position.y;
                    values.PosZ = position.z;
                    values.power = .16f;
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
