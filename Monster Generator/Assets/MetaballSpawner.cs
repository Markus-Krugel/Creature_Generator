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
                float metaballsAmount = 6;

                for (int i = 0; i < metaballsAmount; i++)
                {
                    Vector3 position = new Vector3(i / metaballsAmount + 0.1f, i / metaballsAmount, i / metaballsAmount - 0.1f);

                    GameObject currentBall = Instantiate(prefab, position, Quaternion.identity);

                    Metaball values = currentBall.GetComponent<Metaball>();
                    values.PosX = position.x;
                    values.PosY = position.y;
                    values.PosZ = position.z;
                    values.power = 0.62f;
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
