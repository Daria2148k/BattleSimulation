using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerManager
{

    public class Player_Manager : MonoBehaviour
    {
        //event for UI
        public static event Action stopGameScreen;

        [SerializeField] GameObject Bullet;

        public float Radius;
        public int Speed;
        public float Damage;
        public float Health;

        public static string winner;

        private GameObject[] Enemies;
        int closestEnIndex; 
        Vector3 EnemyPos;


        //setting player stats randomly
        void setStats()
        {
            Radius = Random.Range(2f, 7f);
            GetComponent<SphereCollider>().radius = Radius;

            Speed = Random.Range(200, 700);

            Damage = Random.Range(3f, 7f);

            Health = 10f;
        }


        void Start()
        {
            setStats();

            Restart();
        }

        
        void Update()
        {
            Death();
        }

        //function to find all enemies and add to array
        public void FindAllEnemies()
        {
            switch (gameObject.tag)
            {
                case "Red":
                    Enemies = GameObject.FindGameObjectsWithTag("White");
                    break;
                case "White":
                    Enemies = GameObject.FindGameObjectsWithTag("Red");
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


        public IEnumerator FindAndShoot()
        {
            if (Enemies[closestEnIndex] == null) yield break;
            
            float time = 0f;
            
            //move player to enemy till player body collides with enemies radius 
            while (CheckDistance() && Enemies[closestEnIndex].tag != "Dead")
            {
                transform.position = Vector3.Lerp(transform.position, Enemies[closestEnIndex].transform.position, time/Speed);
                time += Time.deltaTime;
                yield return null;
            }

            //making and bullet and calling a coruitine to shoot
            while (Enemies[closestEnIndex].tag != "Dead")
            {
                GameObject tempBullet = Instantiate(Bullet,transform.position,Quaternion.identity);

                StartCoroutine(Shooter(tempBullet));

                Health-= Enemies[closestEnIndex].GetComponent<Player_Manager>().Damage;

                yield return new WaitForSeconds(1f);
                Destroy(tempBullet);
            }

            Restart();
            yield break;
        }

        bool CheckDistance()
        {
            bool flag= Vector3.Distance(transform.position, EnemyPos) > gameObject.transform.localScale.x / 2 ? true : false;
            return flag;
        }

        

        //function to animate death and change tag to differentiate dead player from alive
        public void Death()
        {
            if (Health < 0 && gameObject.tag!="Dead")
            {
                gameObject.tag = "Dead";

                transform.rotation = Quaternion.Euler(90, 0, 0);
                transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
            }
        }

        //function to restart coroutine, reset targets and end the game 
        public void Restart()
        {
            FindAllEnemies();

            if (Enemies == null)
            {
                return;
            }

            closestEnIndex = FindClosest(Enemies);

            if (closestEnIndex < 0 || closestEnIndex >= Enemies.Length)
            {
                Debug.Log(gameObject.tag+" won!");
                winner = gameObject.tag;

                stopGameScreen?.Invoke();
                return;
            }

            StartCoroutine(FindAndShoot());
        }

        //coroutine for bullet shooting animation 
        public IEnumerator Shooter(GameObject obj)
        {
            float time = 0f;
            while (time<2f)
            {
                if (gameObject.tag=="Dead"||!obj) yield break;
                
                obj.transform.position = Vector3.Lerp(obj.transform.position, Enemies[closestEnIndex].transform.position, time/2);
                time += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}