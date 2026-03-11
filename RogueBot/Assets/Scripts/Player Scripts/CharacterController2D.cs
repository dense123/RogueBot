using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{ // Provided by Brackeys https://www.youtube.com/watch?v=dwcT-Dch0bA,
  // I altered the codes and removed the ones I'm not using

    [SerializeField] private float m_JumpForce = 600f; // Amount of force added when the player jumps.
    [SerializeField] private float m_DoubleJumpForce = 800f;					 
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	 

	// If this boolean is false, player cannot move in the air.
	// This is set false for when player is knocked back to create the
	// knockback effect.
	// .
	public bool m_AirControl = false;
	[SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
	
	// This will be set usually at the players foot, it will get the position
	// of the Game Object for the OverlapCircle function to detect whether the
	// object with GROUND tag has collided/overlapped with this position.
	// .
	[SerializeField] private Transform m_GroundCheck;							

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	
	// Checks if the player is grounded. If false, player is assumed to be in the air,
	// thus player cannot jump anymore (unless for double jumping).
	// Once player lands, it will run the function in the OnLandEvent that is taken from the
	// player script, which disables the jumping animation
	// .
	private bool m_Grounded;            
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool doubleJump;
	    
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	
	/*
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	*/

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		doubleJump = false;
		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated
		// as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project
		// settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, 
			m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump)
	{
		if (m_Grounded && !jump)
			doubleJump = false;


		// only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			// .
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			// And then smoothing it out and applying it to the character.
			// SmoothDamp lets the player movement be more smooth when moving
			// to another area, for a nice look to the movement
			// .
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, 
				ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left
			// .
			if (move > 0 && !m_FacingRight)
			{
				// Flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right
			// .
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
		}

		// Jump boolean will be true once it's trigger by player in the Player script
		// If the player is on the ground, player can jump once which will turn grounded
		// false. Once player jumped once and is currently in the air, he can jump again once
		// more and turn doubleJump to true to prevent anymore jumping.
		// .
		if (jump && (m_Grounded || !doubleJump))
		{
			m_Grounded = false;

			// ternary operator, if player hasn't doubleJumped, it will use the first value
			// for a normal jump.
			// .
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,
				!doubleJump ? m_JumpForce * Time.deltaTime : m_DoubleJumpForce * Time.deltaTime);

			doubleJump = true;
		}
	}


    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
