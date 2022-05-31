using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerManager
{
    //separate class for player attributes
    public class Player_Stats
    {
        
    }

    public class Player_Manager : MonoBehaviour
    {
        //event for UI
        public static event Action stopGameScreen;

        [SerializeField] GameObject Bullet;

        public float Radius;
        public float Speed;
        public float Damage;
        public float Health;

        private GameObject[] Enemies;
        int closestEnIndex; //closest enemy index
        Vector3 EnemyPos;

        //instanse of player attributes
        Player_Stats playerStats = new Player_Stats();


        //setting player stats randomly
        void setStats()
        {
            Radius = Random.Range(5f, 14f);
            GetComponent<SphereCollider>().radius = Radius;

            Speed = Random.Range(10f, 50f);

            Damage = Random.Range(3f, 7f);

            Health = 10f;
        }


        void Start()
        {
            setStats();

            //function to find all enemies and add to array
            FindAllEnemies();
            
            //find closest enemy to player
            closestEnIndex = FindClosest(Enemies);
            
            EnemyPos = Enemies[closestEnIndex].transform.position;

            //main coroutine which calls on all others
            StartCoroutine(Routine());
        }

        
        void Update()
        {
            //function to animate death and change tag
            Death();
        }

        //function to find all enemies and add to array
        //each team has its own tags to recognize enemies
        public void FindAllEnemies()
        {
            switch (gameObject.tag)
            {
                case "TeamRed":
                    Enemies = GameObject.FindGameObjectsWithTag("TeamWhite");
                    break;
                case "TeamWhite":
                    Enemies = GameObject.FindGameObjectsWithTag("TeamRed");
                    break;
                default:
                    Enemies = null;
                    break;
            }
        }

        //basic distanse check to return an index of closest enemy
        public int FindClosest(GameObject[] array)
        {
            float closestDist = Mathf.Infinity;
            int ClosestIndex = -1;

            for (int i = 0; i < array.Length; i++)
            {
                float currentDist = Vector3.Distance(transform.position, array[i].transform.position);
                if (currentDist < closestDist)
                {
                    closestDist = currentDist;
                    ClosestIndex = i;
                }
            }
            return ClosestIndex;
        }

        //cheching collision of players radius with enemies radius
        private void OnTriggerStay(Collider other)
        {
            if (Enemies != null)
            {
                if (closestEnIndex >= 0 && closestEnIndex < Enemies.Length)
                {
                    if (other.CompareTag(Enemies[closestEnIndex].tag))
                    {
                        EnemyPos = other.ClosestPoint(transform.position);
                    }
                }
            }
        }

        //main coroutine
        public IEnumerator Routine()
        {
            if (Enemies[closestEnIndex] == null) yield break;
            
            float time = 0f;
            
            //move player to enemy till player body collides with enemies radius 
            while (Vector3.Distance(transform.position, EnemyPos) > gameObject.transform.localScale.x/2)
            {
                transform.position = Vector3.MoveTowards(transform.position, Enemies[closestEnIndex].transform.position, time/Speed);
                time += Time.deltaTime;
                yield return null;
            }

            //making and bullet and calling a coruitine to shoot
            while (Enemies[closestEnIndex].tag != "Dead")
            {
                //instantiate bullet
                GameObject tempBullet = Instantiate(Bullet,transform.position,Quaternion.identity);
                
                //move bullet towards enemy linearly
                StartCoroutine(Shooter(tempBullet));

                //player reciving damage from enemy while colliding and neither are dead
                Health-= Enemies[closestEnIndex].GetComponent<Player_Manager>().Damage;

                yield return new WaitForSeconds(1f);
                Destroy(tempBullet);
            }

            //"kill" closest enemy is enemies array
            Enemies[closestEnIndex] = null;

            //reastart coroutine with another coroutine and recet targets
            StartCoroutine(Restart());
            yield break;
        }

        //function to animate death and change tag to differentiate dead player from alive
        public void Death()
        {
            if (Health < 0)
            {
                gameObject.tag = "Dead";

                transform.rotation = Quaternion.Euler(90, 0, 0);
                transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
            }
        }

        //coroutine to restart Routine, reset targets and end the game 
        public IEnumerator Restart()
        {
            while (Enemies[closestEnIndex] == null)
            {
                FindAllEnemies();

                //precaution check
                if (Enemies == null)
                {
                    yield break;
                }

                
                closestEnIndex = FindClosest(Enemies);

                //'if' to check for endgame 
                if (closestEnIndex < 0 || closestEnIndex >= Enemies.Length)
                {
                    //print winner colour to console
                    Debug.Log(gameObject.tag+" won!");

                    //event for UI
                    stopGameScreen?.Invoke();
                    yield break;
                }

                EnemyPos = Enemies[closestEnIndex].transform.position;

                StartCoroutine(Routine());
            }
            yield break;
        }

        //coroutine for bullet shooting animation 
        public IEnumerator Shooter(GameObject obj)
        {
            float time = 0f;
            while (time<2f)
            {
                //stop shooting if dead
                if (gameObject.tag=="Dead"||!obj) yield break;
                
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, Enemies[closestEnIndex].transform.position, time/2);
                time += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}