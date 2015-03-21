using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomNetworkTest : MonoBehaviour
{
	public int numPersons;
	public GameObject PersonAIPrefab;

	void Start ()
	{
		for (int i = 0; i < numPersons; i++)
		{
			GameObject personObject = Instantiate(PersonAIPrefab) as GameObject;
			Person person = personObject.GetComponent<Person>();
			person.SetRandomAttributes();

			Vector3 randomPos = Random.insideUnitSphere * 20f;
			personObject.transform.position = randomPos;
		}
	}
	
	void Update ()
	{
	
	}
}
