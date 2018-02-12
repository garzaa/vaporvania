using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;

	public Rigidbody2D pcrb;
	public float lookAheadRatio;
	public float lookAheadSpeed;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	public void SmallShake() {
		this.shakeAmount = .3f;
		this.shakeDuration = .1f;
	}

	public void TinyShake() {
		this.shakeAmount = .1f;
		this.shakeDuration = .01f;
	}

	void Update()
	{
		if (shakeDuration > 0) {
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else {
			shakeDuration = 0f;
		}

		Vector3 newPos = new Vector3(originalPos.x + (pcrb.velocity.x * lookAheadRatio), 
			originalPos.y + (pcrb.velocity.y * lookAheadRatio), 
			originalPos.z);
		Vector3.MoveTowards(this.transform.position, newPos, lookAheadSpeed);
		//this.transform.localPosition = newPos;
	}
}