using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocialSphere : MonoBehaviour
{
	private Person owner;
	public float radius = 1f;

	void Awake ()
	{
		this.owner = this.transform.parent.GetComponent<Person>();
	}
	
	void Update ()
	{
	
	}

	public void SetRadius(float newRadius)
	{
		this.radius = newRadius;
		this.GetComponent<SphereCollider>().radius = this.radius;
	}

	public bool Contains(Person P)
	{
		return (Vector3.Distance(owner.transform.position, P.transform.position) <= radius);
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		SocialSphere otherSphere = otherCollider.GetComponent<SocialSphere>();
		if (otherSphere != null)
		{
			Person otherPerson = otherCollider.GetComponent<SocialSphere>().owner;
			if (otherPerson != null && otherPerson != owner)
			{
				owner.CreateRelationship(otherPerson);
			}
		}
	}

	void OnTriggerStay(Collider otherCollider)
	{
		SocialSphere otherSphere = otherCollider.GetComponent<SocialSphere>();
		if (otherSphere != null)
		{
			Person otherPerson = otherCollider.GetComponent<SocialSphere>().owner;
			if (otherPerson != null && otherPerson != owner)
			{
				owner.CreateRelationship(otherPerson);
			}
		}
	}
}
