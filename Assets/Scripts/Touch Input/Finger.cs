using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Finger : MonoBehaviour
{
	protected internal int id = -1;
	// TOUCH : finger.id = touch.id + 1
	// MOUSE : finger.id = 0
	// UNINITIALIZED : finger.id = -1

	// Recorded previous positions for velocity/path calculation
	protected internal List<Vector2> prevWorldPositions = new List<Vector2>();
	protected internal List<Vector2> prevScreenPositions = new List<Vector2>();
	protected internal List<float> prevTimes = new List<float>();

	// The times that this finger started and ended (became invalid)
	protected internal float? startTime = null, endTime = null;
	protected internal bool isValid, isFresh;

	// The object that was hit when this touch was initiated 
	protected internal GameObject startHitObject;

	// The number of finger positions to record for velocity/path calculations
	private static readonly int recordCount = 10;

	void Update()
	{
		if (this.isFresh) {
			this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			this.gameObject.GetComponent<Collider2D>().enabled = true;
			this.isFresh = false;
		}
		
		this.gameObject.transform.position = GetWorldPosition3();
	}

	void FixedUpdate()
	{
		this.UpdatePrevPositionLists();
	}

	void LateUpdate()
	{
		this.Draw();
	}

	public void SetColor(Color color)
	{
		this.renderer.material.color = color;
	}

	public Color GetColor()
	{
		return this.renderer.material.color;
	}

	// Updates the lists of recorded previous positions
	protected internal void UpdatePrevPositionLists()
	{
		// don't allow list of previous positions to be longer than 10
		if (prevWorldPositions.Count > recordCount-1)
		{
			prevWorldPositions.RemoveAt(0);
			prevScreenPositions.RemoveAt(0);
			prevTimes.RemoveAt(0);
		}

		// update lists of previous input positions
		prevWorldPositions.Add(this.GetWorldPosition());
		prevScreenPositions.Add(this.GetScreenPosition());
		// update list of previous input times
		prevTimes.Add(Time.time);
	}

	// Set the current validity of this finger
	protected internal void SetValidity(bool validity)
	{
		if (validity)
		{
			this.isValid = true;
			this.SetStartTime();
		}
		else
		{
			this.isValid = false;
			this.SetEndTime();
		}
	}

		// Sets the time that this finger was initialized
		// SHOULD ONLY BE CALLED UPON INITIALIZATION
		protected internal void SetStartTime()
		{
			this.startTime = Time.time;
		}

		// Sets the time that this finger became invalid
		// SHOULD ONLY BE CALLED WHEN FINGER BECOMES INVALID
		protected internal void SetEndTime()
		{
			if (this.startTime != null)
			{	
				this.endTime = Time.time;
			}
		}

	// Gets the current validity of this finger
	protected internal bool GetValidity()
	{
		return this.isValid;
	}

	// Sets the id of this finger
	protected internal void SetId(int id)
	{
		this.id = id;
	}

	// Gets the nullable id of this touch
	public int GetId()
	{
		return this.id;
	}

	// Returns whether this finger is a touch
	public virtual bool isTouch() {		return false;	}

	// Returns whether this finger is a mouse
	public virtual bool isMouse() {		return false;	}

	// Gets the game object currently being hit by the finger
	public GameObject GetCurrentHitObject()
	{
		RaycastHit2D hit = Physics2D.Raycast(GetRayOrigin(), Vector2.zero);
		if (hit.collider != null)
		{
			return hit.collider.gameObject;
		}

		return null;
	}
	// Finds the object that this finger hit upon initialization
	// SHOULD ONLY BE CALLED DURING INITIALIZATION
	protected internal void FindStartHitObject()
	{
		this.startHitObject = GetCurrentHitObject();
	}

	// Gets the object this finger was hitting when it was initalized
	public GameObject GetStartHitObject()
	{
		return this.startHitObject;
	}

	// Gets whether this finger is currently hitting a specific game object
	public bool isHittingObject(GameObject go)
	{
		return (go != null && go == this.GetCurrentHitObject());
	}

	// Gets whether this finger was initialized hitting a specific game object
	public bool startedHittingObject(GameObject go)
	{
		return (go != null && go == this.GetStartHitObject());
	}

	// If this finger is valid, 					returns the duration of this finger since it was initialized
	// If this finger was valid but is now invalid, returns the duration of this finger during which it was valid
	// If this finger was never valid, 				returns null
	public float GetDuration()
	{
		float duration = 0f;

		if (this.GetValidity())
		{
			duration = (float)(Time.time - startTime);
		}
		else
		{
			if ((endTime - startTime) != null)
			{
				duration = (float)(endTime - startTime);
			}
		}

		return duration;
	}

	// Gets the total duration of the recording of previous finger positions
	protected internal float GetRecordedDuration()
	{
		return (prevTimes[0] - prevTimes[prevTimes.Count-1]);
	}

	// Gets the position of this finger in 2D screen coordinates
	public virtual Vector2 GetScreenPosition() { 	return Vector2.zero;	}

		// Gets the position of this finger in 3D screen coordinates
		protected internal Vector3 GetScreenPosition3() 
		{
			Vector2 screenPos = this.GetScreenPosition();
			return new Vector3(screenPos.x, screenPos.y, 0f);
		}

	// Gets the position of this finger in 2D world coordinates
	public Vector2 GetWorldPosition() 
	{
		Vector2 screenPos = this.GetScreenPosition();
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

		return worldPos;
	}

		// Gets the position of this finger in 3D world coordinates
		protected internal Vector3 GetWorldPosition3() 
		{
			Vector2 worldPos = this.GetWorldPosition();
			return new Vector3(worldPos.x, worldPos.y, 0f);
		}

	// Gets the 2D origin of a ray starting from the camera's plane and ending at this finger's current world position	
	protected internal Vector2 GetRayOrigin()
	{
		Vector2 rayOrigin = this.GetWorldPosition();

		return rayOrigin;

	}
		// Gets the 3D origin of a ray starting from the camera's plane and ending at this finger's current world position	
		protected internal Vector3 GetRayOrigin3()
		{
			Vector3 rayOrigin = this.GetWorldPosition3();
			rayOrigin = new Vector3(rayOrigin.x, rayOrigin.y, Camera.main.transform.position.z);

			return rayOrigin;
		}

	// Calculate finger screen velocity
	public Vector2 GetScreenVelocity()
	{
		return this.GetVelocity(prevScreenPositions);
	}
		
	// Calculate finger world velocity
	public Vector2 GetWorldVelocity() 
	{
		return this.GetVelocity(prevWorldPositions);
	}

	// Calculate finger world/screen velocity
	protected internal Vector2 GetVelocity(List<Vector2> prevPositions)
	{
		Vector2 velocity = Vector2.zero;
		if (prevPositions.Count > 1)
		{
			Vector2 sumDeltas = Vector2.zero;
			for (int i = 1; i < prevPositions.Count; i++)
			{
				sumDeltas += prevPositions[i] - prevPositions[i-1];
			}
			//sumDeltas += this.GetWorldPosition() - prevPositions[prevPositions.Count-1];

			velocity = sumDeltas / this.GetRecordedDuration();
		}

		return velocity;
	}

	// Draws this finger's ray from the camera
	public virtual void Draw() {}

	public virtual string ToString() 
	{
		string output = "";
		output += "NonTouch " + this.GetId() + " ";

		Vector2 worldPos = this.GetWorldPosition();
		output += "@ {" + worldPos.x + ", " + worldPos.y + "} ";


		Vector2 worldVel = this.GetWorldVelocity();
		output += " with velocity {" + worldVel.x + ", " + worldVel.y + "}";

		return output;
	}
}