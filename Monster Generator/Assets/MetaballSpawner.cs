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
                Vector3 position = new Vector3(.16f, .26f, .16f);

                GameObject currentBall = Instantiate(prefab, position, Quaternion.identity);

                Metaball values = currentBall.GetComponent<Metaball>();
                values.PosX = position.x;
                values.PosY = position.y;
                values.PosZ = position.z;
                values.power = .13f;


                position = new Vector3(.13f, -.134f, .35f);

                currentBall = Instantiate(prefab, position, Quaternion.identity);

                values = currentBall.GetComponent<Metaball>();
                values.PosX = position.x;
                values.PosY = position.y;
                values.PosZ = position.z;
                values.power = .12f;


                position = new Vector3(-.18f, .125f, -.25f);

                currentBall = Instantiate(prefab, position, Quaternion.identity);

                values = currentBall.GetComponent<Metaball>();
                values.PosX = position.x;
                values.PosY = position.y;
                values.PosZ = position.z;
                values.power = .16f;


                position = new Vector3(-.13f, .23f, .255f);

                currentBall = Instantiate(prefab, position, Quaternion.identity);

                values = currentBall.GetComponent<Metaball>();
                values.PosX = position.x;
                values.PosY = position.y;
                values.PosZ = position.z;
                values.power = .13f;


                position = new Vector3(-.18f, .125f, .35f);

                currentBall = Instantiate(prefab, position, Quaternion.identity);

                values = currentBall.GetComponent<Metaball>();
                values.PosX = position.x;
                values.PosY = position.y;
                values.PosZ = position.z;
                values.power =  .12f;

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
