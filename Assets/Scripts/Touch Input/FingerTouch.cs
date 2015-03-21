using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerTouch : Finger
{
	// This Finger object represents ACTUAL touchscreen input, each one representing a single finger's touch on the touchscreen
	
	// The Input.touch object being represented by this Finger object.
	private Touch touch;

	void Awake()
	{
		this.SetValidity(true);
		this.UpdatePrevPositionLists();
		this.FindStartHitObject();

		this.isFresh = true;
	}

	// Find the input.touch for this FingerTouch (it doesn't update itself)
	public void FindTouch()
	{
		foreach (Touch touch in Input.touches)
		{
			if (touch.fingerId == this.GetTouchId())
			{
				this.SetTouch(touch);
			}
		}
	}

	// Gets the id of this finger's touch
	public int GetTouchId()
	{
		return (int)((this.id) - 1);
	}

	// Gets the input.touch that initialized this finger
	public Touch GetTouch()
	{
		this.FindTouch();
		return this.touch;
	}

	// Sets the touch that initialized this finger
	public void SetTouch(Touch touch)
	{
		this.touch = touch;
		this.SetTouchId(touch);
	}

		// Sets the id of this finger given a touch
		internal void SetTouchId(Touch touch)
		{
			this.SetId(touch.fingerId + 1);
		}

	public override Vector2 GetScreenPosition()
	{
		Touch fingerTouch = this.GetTouch();
		Vector2 screenPos = new Vector2(fingerTouch.position.x, fingerTouch.position.y);
		
		return screenPos;
	}

	public override bool isMouse()
	{
		return false;
	}

	public override bool isTouch()
	{
		return true;
	}

	public override string ToString() 
	{
		string output = "";
		output += "FingerTouch " + this.GetId() + " ";

		Vector2 worldPos = this.GetWorldPosition();
		output += "@ {" + worldPos.x + ", " + worldPos.y + "} ";


		Vector2 worldVel = this.GetWorldVelocity();
		output += " with velocity {" + worldVel.x + ", " + worldVel.y + "}";

		return output;
	}

	// Draws this finger's ray from the camera (green for fingertouch, red for invalid)
	public override void Draw()
	{
		if (this.GetValidity())
		{
			Debug.DrawRay(this.GetRayOrigin3(), Vector3.forward * 2f, Color.green);
		}
		else
		{
			Debug.DrawRay(this.GetRayOrigin3(), Vector3.forward * 2f, Color.red);
		}
	}
}
