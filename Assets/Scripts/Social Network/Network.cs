using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network : MonoBehaviour
{
	public static Network instance;

	public GameObject personPrefab;
	public GameObject relationshipPrefab;

	public List<Person> people = new List<Person>();
	public Dictionary<Person[], Relationship> relationships = new Dictionary<Person[], Relationship>(new RelationshipComparerer()); 

	void Awake ()
	{
		instance = this;
	}
	
	void Update ()
	{
	
	}

	public Relationship CreateRelationship(Person A, Person B)
	{
		if (!people.Contains(A))
		{
			people.Add(A);
		}

		if (!people.Contains(B))
		{
			people.Add(B);
		}

		if (relationships.ContainsKey(new Person[2]{A, B}))
		{
			//Debug.Log("Relationship already exists");
			return relationships[new Person[2]{A, B}];
		}
		else if (relationships.ContainsKey(new Person[2]{B, A}))
		{
			return relationships[new Person[2]{B, A}];
		}
		else
		{
			GameObject relationshipObject = Instantiate(relationshipPrefab) as GameObject;
			relationshipObject.name = "" + A.id + " <-> " + B.id;
			Relationship relationshipAB = relationshipObject.GetComponent<Relationship>();
			relationshipAB.SetPersons(A, B);
			relationships.Add(new Person[2]{A, B}, relationshipAB);
			return relationshipAB;
		}
	}

	public void DestroyRelationship(Person A, Person B)
	{
		if (!people.Contains(A))
		{
			people.Add(A);
		}

		if (!people.Contains(B))
		{
			people.Add(B);
		}

		if (relationships.ContainsKey(new Person[2]{A, B}))
		{
			//Debug.Log("Relationship already exists");
			relationships.Remove(new Person[2]{A, B});
		}
		else if (relationships.ContainsKey(new Person[2]{B, A}))
		{
			relationships.Remove(new Person[2]{B, A});
		}
	}

	public void AddRelationship(Relationship R)
	{
		this.CreateRelationship(R.A, R.B);
	}

	public Relationship GetRelationship(Person A, Person B)
	{
		if (relationships.ContainsKey(new Person[2]{A, B}))
		{
			return relationships[new Person[2]{A, B}];
		}
		else if (relationships.ContainsKey(new Person[2]{B, A}))
		{
			return relationships[new Person[2]{B, A}];
		}
		else
		{
			//Debug.Log("Relationship doesn't exist");
		}

		return null;
	}

	public void PingFromTo(Person A, Person B)
	{
		if (relationships.ContainsKey(new Person[2]{A, B}))
		{
			relationships[new Person[2]{A, B}].Ping(A);
		}
		else if (relationships.ContainsKey(new Person[2]{B, A}))
		{
			relationships[new Person[2]{B, A}].Ping(A);
		}
		else
		{
			Debug.Log("Relationship doesn't exist");
		}
	}
}





public class RelationshipComparerer : IEqualityComparer<Person[]>
{
	public bool Equals(Person[] x, Person[] y)
	{
		return ((x[0] == y[0] && x[1] == y[1]) || (x[1] == y[0] && x[0] == y[1]));
	}

	public int GetHashCode(Person[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
            	if (obj[i] != null)
            	{
                	result = result * 23 + obj[i].Id() * 7;
                }
            }
        }
        return result;
    }
}
