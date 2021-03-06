﻿using UnityEngine;

namespace UnitySampleAssets._2D
{

    public class PlatformerCharacter2D : MonoBehaviour
    {
        private bool facingRight = true; // For determining which way the player is currently facing.

        [SerializeField] private float maxSpeed = 10f; // The fastest the player can travel in the x axis.
        [SerializeField] private float jumpForce = 800f; // Amount of force added when the player jumps.	

        [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;
                                                     // Amount of maxSpeed applied to crouching movement. 1 = 100%

        [SerializeField] private bool airControl = false; // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask whatIsGround; // A mask determining what is ground to the character

        private Transform groundCheck; // A position marking where to check if the player is grounded.
        private float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool grounded = false; // Whether or not the player is grounded.
        private Transform ceilingCheck; // A position marking where to check for ceilings
        private float ceilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator anim; // Reference to the player's animator component.
        private AudioClip coffee;
        AudioSource audio;

        bool doubleJump = false;
        bool sprinting = false;
        float sprintTime;
        float gameTime;

//        private float difficulty2 = 20f;
//        private float difficulty3 = 30f;
//        private float difficulty4 = 40f;

        private void Awake()
        {
            gameTime = Time.time;
            // Setting up references.
            groundCheck = transform.Find("GroundCheck");
            ceilingCheck = transform.Find("CeilingCheck");
            anim = GetComponent<Animator>();
            audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Time.time > gameTime+ 30f)
            { maxSpeed += 3f;
                gameTime = Time.time;
                audio.Play();
            }

            /*
            //difficulty implementation
            if (Time.time > gameTime + 130f)
                maxSpeed = difficulty4;
            else if (Time.time > (90f + gameTime) && Time.time < (gameTime + 130f))
                maxSpeed = difficulty3;
            else if (Time.time > (45f + gameTime) && Time.time < (gameTime + 90f))
                maxSpeed = difficulty2;
            */
        }
        private void FixedUpdate()
        {
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
            anim.SetBool("Running", grounded);

            // Set the vertical animation
//            anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);

            if (grounded)
			{
                doubleJump = false;
//				anim.SetBool("Jumping", false);
			}
            // sprint time is now 0.2 seconds long
            if(sprinting && (Time.time > sprintTime + 0.2))
            {
                maxSpeed -= 50f;
                sprinting = false;
            }
        }


        public void Move(float move, bool crouch, bool jump)
        {

            if (Input.GetKeyDown(KeyCode.LeftShift) && !sprinting)
            {
                sprinting = true;
                sprintTime = Time.time;
                maxSpeed += 50f;
            }


/*			// If crouching, check to see if the character can stand up - we don't care about crouching
            if (!crouch && anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
                    crouch = true;
            }
*/
            // Set whether or not the character is crouching in the animator
//            anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (grounded || airControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*crouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
//                anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                GetComponent<Rigidbody2D>().velocity = new Vector2(move*maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

/*               // If the input is moving the player right and the player is facing left...
                if (move > 0 && !facingRight)
                    // ... flip the player.
                    Flip();
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && facingRight)
                    // ... flip the player.
                    Flip(); 
*/
            }
            // If the player should jump...
            if ((grounded || !doubleJump) && jump)// && anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                grounded = false;

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);

                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

                if (!grounded)
                    doubleJump = true;
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

		public void setSpeed(float newSpeed)
		{
			maxSpeed = newSpeed;
		}

		public double getMaxSpeed()
		{
            return maxSpeed;
		}

		void OnGui()
		{
			GUI.Label(new Rect(10, 10, 300, 30), "Speed: " + maxSpeed);
		}
    }
}
