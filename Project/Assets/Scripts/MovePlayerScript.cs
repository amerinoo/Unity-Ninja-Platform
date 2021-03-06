﻿#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
#define KEYBOARD_PLATFORM
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePlayerScript : MonoBehaviour
{

	[HideInInspector] public bool facingRight = true;
	[HideInInspector] public bool jump = false;

	public float maxSpeed = 8f;
	public float jumpForce = 700f;
	public LayerMask whatIsGround;

	private bool isGrounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;
	private float circleRadius = 0.2f;
	private Quaternion calibrationQuat;

	public Transform[] groundPoints;
	public bool dead;

	public SoundEffectsController sec;


	// Use this for initialization
	void Awake ()
	{
		sec = GetComponent<SoundEffectsController> ();
		anim = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void CalibrateAccelerometer ()
	{
		Vector3 accelerationSnapshot = Input.acceleration;
		Quaternion rotateQuat = Quaternion.FromToRotation (
			                        new Vector3 (0.0f, 0.0f, -1.0f), accelerationSnapshot);
		calibrationQuat = Quaternion.Inverse (rotateQuat);
	}

	// Update is called once per frame
	void Update ()
	{
		HandleInput ();
	}

	void FixedUpdate ()
	{
		if (!dead) {
			float horizontal = GetHorizontal ();
			isGrounded = IsGrounded ();
			HandleMovement (horizontal);
			Flip (horizontal);
			HandleLayers ();
			ResetValues ();
		}
	}

	float GetHorizontal ()
	{
		#if KEYBOARD_PLATFORM
		return Input.GetAxis ("Horizontal");
		#else
		return FixAcceleration (Input.acceleration);
		#endif
	}

	void HandleMovement (float horizontal)
	{
		if (rb2d.velocity.y < 0) {
			anim.SetBool ("land", true);
		}
		
		if (Mathf.Abs (horizontal) > 0.01) {
			rb2d.velocity = new Vector2 (horizontal * maxSpeed, rb2d.velocity.y);
		}

		if (isGrounded && jump) {
			isGrounded = false;
			rb2d.AddForce (new Vector2 (0f, jumpForce));
			anim.SetTrigger ("jump");
			sec.PlayJump ();
		}

		anim.SetFloat ("Speed", Mathf.Abs (horizontal));
	}

	void HandleInput ()
	{
		if (Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Jump"))
			jump = true;
	}

	void ResetValues ()
	{
		jump = false;
	}

	float FixAcceleration (Vector3 acceleration)
	{
		acceleration = calibrationQuat * acceleration;
		if (acceleration.x < 0.1 && acceleration.x > -0.1)
			return 0;
		else if (acceleration.x > 0.1)
			return 1;
		else
			return -1;

	}

	void Flip (float horizontal)
	{
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	bool IsGrounded ()
	{
		foreach (Transform point in groundPoints) {
			Collider2D[] colliders = Physics2D.OverlapCircleAll (point.position, circleRadius, whatIsGround);
			for (int i = 0; i < colliders.Length; i++) {
				if (!colliders [i].gameObject.Equals (gameObject)) {
					anim.ResetTrigger ("jump");
					anim.SetBool ("land", false);
					return true;
				}
			}
		}
		return false;
	}

	void HandleLayers ()
	{
		if (!isGrounded) {
			anim.SetLayerWeight (1, 1);
		} else {
			anim.SetLayerWeight (1, 0);
		}
	}
}
