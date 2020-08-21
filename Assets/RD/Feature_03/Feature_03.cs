using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature_03 : MonoBehaviour
{
	public GameObject gMixedRealityPlaysapce;
	public GameObject gSlingShotPrefab;
	public GameObject gProjectilePrefab;

	public float gVelocity = 500;

	// Start is called before the first frame update
	void Start()
	{
		Physics.gravity = new Vector3(0, -0.5f, 0);
	}

	private bool mFIsLeftHandClosed = false;
	private bool mFIsRightHandGrab = false;
	private GameObject mSlingShot = null;
	private GameObject mProjectile = null;
	// Update is called once per frame
	void Update()
	{
		GameObject leftHand = null;
		GameObject rightHand = null;
		FindHandsObjectRoot(out leftHand, out rightHand);


		if (leftHand != null)
		{
			GameObject pinkyKnuckle = null;
			FindJoint(leftHand, "PinkyKnuckle", out pinkyKnuckle);

			if (CheckIfCloseHand(leftHand) && !mFIsLeftHandClosed)
			{
				Debug.Log("close left hand");
				mFIsLeftHandClosed = true;

				if (pinkyKnuckle != null)
				{

					mSlingShot = Instantiate(gSlingShotPrefab, pinkyKnuckle.transform.position, pinkyKnuckle.transform.rotation);
					//mSlingShot.transform.parent = pinkyKnuckle.transform;					
					Debug.Log("spawn slingshot");


				}
			}
			if (!CheckIfCloseHand(leftHand) && mFIsLeftHandClosed)
			{
				Debug.Log("open left hand");
				mFIsLeftHandClosed = false;

				Destroy(mSlingShot);
				mSlingShot = null;
			}

			// sync the position of slingshot
			if (mSlingShot != null && pinkyKnuckle != null)
			{
				mSlingShot.transform.position = Vector3.Lerp(mSlingShot.transform.position, pinkyKnuckle.transform.position, Time.deltaTime * 10);
				mSlingShot.transform.rotation = Quaternion.Lerp(mSlingShot.transform.rotation, pinkyKnuckle.transform.rotation, Time.deltaTime * 10);
			}
		}
		else
		{
			if (mSlingShot != null)
			{
				Destroy(mSlingShot);
				mSlingShot = null;
				mFIsLeftHandClosed = false;
			}
		}



		if (rightHand != null)
		{
			if (mSlingShot != null)
			{
				if (CheckIfGrab(rightHand) && !mFIsRightHandGrab)
				{
					Debug.Log("close right hand");
					mFIsRightHandGrab = true;

					GameObject indexTip = null;
					FindJoint(rightHand, "IndexTip", out indexTip);
					if (indexTip != null)
					{

						GameObject fakeProjectileball = GameObject.Find("Projectileball");
						if ((indexTip.transform.position - fakeProjectileball.transform.position).magnitude < 0.05)
						{

							/*if (mProjectile != null)
							{
								Destroy(mProjectile);
								mProjectile = null;
							}*/

							//mProjectile = Instantiate(gProjectilePrefab, indexTip.transform.position, indexTip.transform.rotation);
							//mProjectile.transform.parent = indexTip.transform;


							mProjectile = Instantiate(gProjectilePrefab, indexTip.transform.position, indexTip.transform.rotation);
							//mProjectile.transform.position = indexTip.transform.position;
							//mProjectile.transform.rotation = indexTip.transform.rotation;

							GameObject.Find("Projectileball").GetComponent<MeshRenderer>().enabled = false;
							Debug.Log("spawn projectile");
						}
					}
				}

				if (!CheckIfGrab(rightHand) && mFIsRightHandGrab)
				{

					Debug.Log("open right hand");
					mFIsRightHandGrab = false;

					if (mProjectile != null)
					{
						mProjectile.transform.parent = null;
						Rigidbody body = mProjectile.GetComponent(typeof(Rigidbody)) as Rigidbody;
						body.useGravity = true;
						body.AddForce(((mSlingShot.transform.position - (mSlingShot.transform.right.normalized * -0.21f)) - mProjectile.transform.position) * gVelocity);
						GameObject.Find("Projectileball").GetComponent<MeshRenderer>().enabled = true;
						mProjectile = null;
					}
				}

				if (mProjectile != null && mFIsRightHandGrab)
				{
					GameObject indexTip = null;
					FindJoint(rightHand, "IndexTip", out indexTip);
					if (indexTip != null)
					{
						mProjectile.transform.position = indexTip.transform.position;
						mProjectile.transform.rotation = indexTip.transform.rotation;
					}
				}
			}
		}
		else
		{
			//Projectile over eyesight 
			Debug.Log("right hand not detected");
			// has Projectile and  RightHandGrab
			if (mProjectile != null && mFIsRightHandGrab)
			{
				Debug.Log("right hand disappear and projectile !null");
				mFIsRightHandGrab = false;

				//mProjectile.transform.parent = null;

				Rigidbody body = mProjectile.GetComponent(typeof(Rigidbody)) as Rigidbody;
				body.useGravity = true;
				body.AddForce(((mSlingShot.transform.position - (mSlingShot.transform.right.normalized * -0.21f)) - mProjectile.transform.position) * gVelocity);
				GameObject.Find("Projectileball").GetComponent<MeshRenderer>().enabled = true;
				Debug.Log(mProjectile.transform.position);
			}
			else
			{
				Debug.Log("projectile null");
			}
		}
	}


	// **** **** **** **** ****	
	void FindHandsObjectRoot(out GameObject LeftHand, out GameObject RightHand)
	{
		LeftHand = RightHand = null;

		int playspaceChildCount = gMixedRealityPlaysapce.transform.childCount;
		for (int i = 0; i < playspaceChildCount; i++)
		{
			GameObject child = gMixedRealityPlaysapce.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains("Left_HandLeft".ToLower()))
			{
				LeftHand = child;
			}
			if (child.name.ToLower().Contains("Right_HandRight".ToLower()))
			{
				RightHand = child;
			}
			if (LeftHand != null && RightHand != null)
			{
				break;
			}
		}
	}

	void FindJoint(GameObject Hand, string JointName, out GameObject Joint)
	{
		Joint = null;

		for (int i = 0; i < Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains(JointName.ToLower()))
			{
				Joint = child;
			}
			if (Joint != null)
			{
				break;
			}
		}
	}

	bool CheckIfCloseHand(GameObject Hand)
	{
		if (Hand == null)
		{
			return false;
		}

		GameObject thumbTip = null;
		GameObject indexTip = null;
		GameObject middleTip = null;
		GameObject ringTip = null;
		GameObject pinkyTip = null;
		GameObject wrist = null;
		float checkVal = -1;

		for (int i = 0; i < Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains("ThumbTip".ToLower()))
			{
				thumbTip = child;
			}
			if (child.name.ToLower().Contains("IndexTip".ToLower()))
			{
				indexTip = child;
			}
			if (child.name.ToLower().Contains("MiddleTip".ToLower()))
			{
				middleTip = child;
			}
			if (child.name.ToLower().Contains("RingTip".ToLower()))
			{
				ringTip = child;
			}
			if (child.name.ToLower().Contains("PinkyTip".ToLower()))
			{
				pinkyTip = child;
			}
			if (child.name.ToLower().Contains("Wrist".ToLower()))
			{
				wrist = child;
			}
			if (thumbTip != null && indexTip != null && middleTip != null && ringTip != null && pinkyTip != null && wrist != null)
			{
				float lenTW = (thumbTip.transform.position - wrist.transform.position).magnitude;
				float lenIW = (indexTip.transform.position - wrist.transform.position).magnitude;
				float lenMW = (middleTip.transform.position - wrist.transform.position).magnitude;
				float lenRW = (ringTip.transform.position - wrist.transform.position).magnitude;
				float lenPW = (pinkyTip.transform.position - wrist.transform.position).magnitude;

				checkVal = (lenTW + lenIW + lenMW + lenRW + lenPW) / 5;

				break;
			}
		}

		if (checkVal != -1 && checkVal < 0.09)
		{
			return true;
		}
		return false;
	}

	bool CheckIfGrab(GameObject Hand)
	{
		if (Hand == null)
		{
			return false;
		}

		GameObject thumbTip = null;
		GameObject indexTip = null;
		float checkVal = -1;

		for (int i = 0; i < Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains("ThumbTip".ToLower()))
			{
				thumbTip = child;
			}
			if (child.name.ToLower().Contains("IndexTip".ToLower()))
			{
				indexTip = child;
			}
			if (thumbTip != null && indexTip != null)
			{
				checkVal = (thumbTip.transform.position - indexTip.transform.position).magnitude;

				break;
			}
		}

		if (checkVal != -1 && checkVal < 0.02)
		{
			return true;
		}
		return false;
	}

}
