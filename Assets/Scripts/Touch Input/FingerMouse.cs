using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerMouse : Finger
{
	// This Finger object represents a mouse-driven simulation of a finger on a touch-screen so you can test the touch-related stuff without deploying a build to a mobile device.
	void Awake()
	{
		this.SetId(0);
		this.SetValidity(true);
		this.UpdatePrevPositionLists();
		this.FindStartHitObject();
		this.isFresh = true;
	}

	public override Vector2 GetScreenPosition()
	{
		Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);	
		return screenPos;
	}

	public override bool isMouse()
	{
		return true;
	}

	public override bool isTouch()
	{
		return false;
	}

	// Draws this finger's ray from the camera (blue for fingermouse, red for invalid)
	public override void Draw()
	{
		if (this.GetValidity())
		{
			Debug.DrawRay(this.GetRayOrigin3(), Vector3.forward * 2f, Color.blue);
		}
		else
		{
			Debug.DrawRay(this.GetRayOrigin3(), Vector3.forward * 2f, Color.red);
		}
	}

	public override string ToString() 
	{
		string output = "";
		output += "FingerMouse " + this.GetId() + " ";

		Vector2 worldPos = this.GetWorldPosition();
		output += "@ {" + worldPos.x + ", " + worldPos.y + "} ";


		Vector2 worldVel = this.GetWorldVelocity();
		output += " with velocity {" + worldVel.x + ", " + worldVel.y + "}";

		return output;
	}
}
