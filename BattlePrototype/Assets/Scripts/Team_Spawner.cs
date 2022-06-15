using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team_Spawner : MonoBehaviour
{
    [SerializeField] GameObject teamRed;
    [SerializeField] GameObject teamWhite;

    int numOfPlayers=40;

    float capsuleHeight=2.5f;

    void Start()
    {
        spawnPlayers(numOfPlayers);
    }

    //function to spawn
    public void spawnPlayers(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject tempPosWhite = Instantiate(teamWhite);
            tempPosWhite.transform.position = RandomPoint(transform.GetChild(0).GetComponent<Collider>());

            GameObject tempPosRed = Instantiate(teamRed);
            tempPosRed.transform.position = RandomPoint(transform.GetChild(1).GetComponent<Collider>());
        }
    }

    //basic boundaries random generator
    Vector3 RandomPoint(Collider collider)
    {
        Vector3 point = new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),capsuleHeight,
            Random.Range(collider.bounds.min.z, collider.bounds.max.z));

        //precaution check
        if (point != collider.ClosestPoint(point))
        {
            point = RandomPoint(collider);
        }

        return point;
    }
}
