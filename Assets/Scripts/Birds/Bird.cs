/*
Bird.cs

Description: The master class
To be refactored later.
Author: Yu Long
Created: Wednesday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reimirno 
{
    #region Bird Enums
    public enum BirdState
    {
        InQueue,            //In queue, doing nothing, waiting to be called.
        GettingReady,       //During the jump from the ground to sling, the bird is in this state.
        Interactable,       //The bird is on the sling and waiting for user to drag and release it.
        BeingPushedBySwing, //The user releases it and the spring joint is giving the bird recovery force. The joint has not broken.
        Free,               //The spring is broken and the bird is set to free. The user can click to activate its skill, if any.
        Skilled,            //The bird has performed its skill, if any.
        Hit                 //The bird has hit something and thus starts death countdown.
    }

    public enum BirdType
    {
        Red,
        Yellow,
        Blue
    }
    #endregion

    public class Bird: MonoBehaviour, IDamageDealer
    {
        #region Data
        [Header("The Only Control Needed")]
        [Tooltip("The Type of Bird. This controls all data of this bird gameObject.")]
        public BirdType birdType;
        //These fields would later be automatically filled in according to the birdType specified.
        protected int rawDamage;
        protected float lifeTimeAfterHit;
        protected bool hasSkill;
        protected AnimatorOverrideController controller;
        #endregion

        #region Run-time Parameters
        [Header("Read Only")]
        [Tooltip("How far away I can drag the bird from the slingshot anchor point.")]
        [ReadOnly, SerializeField] protected float maxDragDistance = 1.3f;
        [Tooltip("How long does the bird takes to jump to the sling, when it gets ready.")]
        [ReadOnly, SerializeField] protected float jumpUpTime = 1f;
        [Tooltip("How long to wait until feeding the next bird to sling, after the first bird it shot.")]
        [ReadOnly, SerializeField] protected float intervalBetweenBirds = 3f;
        //References
        protected Transform anchorPoint;
        protected Animator animator;
        protected Rigidbody2D body;
        protected SpringJoint2D springJoint;
        protected LineRenderer rightLineRenderer;
        protected Transform rightEndPoint;
        protected LineRenderer leftLineRenderer;
        protected Transform leftEndPoint;

        //Properties
        //When the bird is on the sling, update the status of clicking
        [ReadOnly, SerializeField] protected bool _isClicking = false;
        public bool IsClicking
        {
            get => _isClicking;
            protected set
            {
                if (value)
                {
                    
                    _isClicking = true;
                    body.isKinematic = true;
                }
                else
                {
                    //When it is set to false, let bird be influenced by physics
                    //because we have released the sling
                    _isClicking = false;
                    body.isKinematic = false;
                }
            }
        }
        private HashSet<string> animStates = new HashSet<string>();
        //Bird's state
        [ReadOnly, SerializeField] private BirdState _curState;
        public BirdState curState {
            get => _curState;
            set
            {
                _curState = value;

                //When shot from the sling, stop restricting rotation
                if (value == BirdState.Free) 
                    body.constraints = RigidbodyConstraints2D.None;

                //Update animation according to the bird state
                //The state/parameters have the same name as the bird state
                var key = Enum.GetName(typeof(BirdState), value);
                if (animStates.Contains(key))
                {
                    this.Log("anim set to", key);
                    animator.SetTrigger(key);
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Get references to components upon enabling.
        /// Also, do all sorts of initialization work here.
        /// </summary>
        protected void OnEnable()
        {
            //Get data from data hub according to the type designated
            var data = DataHub.Instance.GetBirdData(birdType);
            //Memorize these data
            rawDamage = data.rawDamage;
            lifeTimeAfterHit = data.lifeTimeAfterHit;
            hasSkill = data.hasSkill;
            controller = data.controller;

            ///Setting up refs and data
            //Set up spring joint and relevant refs
            springJoint = GetComponent<SpringJoint2D>();
            var slingObj = GameObject.FindGameObjectWithTag("Sling");
            springJoint.connectedBody = slingObj.GetComponent<Rigidbody2D>();
            var slingTrans = slingObj.transform;
            anchorPoint = slingTrans.Find("Anchor");
            rightEndPoint = slingTrans.Find("RightEnd");
            leftEndPoint = slingTrans.Find("LeftEnd");
            springJoint.enabled = false;
            //Set up rigidboay
            body = GetComponent<Rigidbody2D>();
            body.mass = DataHub.Instance.GetMassByheaviness(data.heaviness);
            body.isKinematic = true; //Doesn't want the bird to be affected by physics upon start

            //Animation-related set up
            //Stores all available animator parameters in a hash set first
            //I made it so that each BirdState could possibly correspond to an animator state
            //And each animator state is triggered by a parameter with the same name, if any
            //Hence, we need to check what states/parameters we have before we start animating
            animator = GetComponent<Animator>();
            foreach (var state in animator.parameters.Select(x => x.name))
            {
                animStates.Add(state);
            }
            animator.runtimeAnimatorController = controller;

            //Render the slingshot belt
            rightLineRenderer = rightEndPoint.GetComponent<LineRenderer>();
            leftLineRenderer = leftEndPoint.GetComponent<LineRenderer>();

            //Some parameters for the "hop up to sling" effect later
            originalPos = transform.position;
            jumpUpSpeed = Mathf.Abs(anchorPoint.position.x - originalPos.x) / jumpUpTime;

            curState = BirdState.InQueue;
        }

        //A simple state machine
        protected void Update()
        {
            switch (curState)
            {
                case BirdState.InQueue:
                    break;
                case BirdState.GettingReady:
                    JumpUpToSling();
                    break;
                case BirdState.Interactable:
                    if (IsClicking)
                    {
                        UpdateBirdPos();
                    }
                    DrawSlingBelt();
                    break;
                case BirdState.BeingPushedBySwing:
                    DrawSlingBelt();
                    break;
                case BirdState.Free:
                    DrawSlingBelt();
                    WaitForSkill();
                    break;
                case BirdState.Hit:
                    break;
            }
        }

        /// <summary>
        /// This is called by external GameManager to let a bird in queue to hop to sling.
        /// Once this is called, the state machine (Update method) would execute the next step.
        /// </summary>
        public void GetReady()
        {
            if ((int)curState >= (int)BirdState.GettingReady)
            {
                gameObject.LogError("You are trying to go back in birdState!");
            }
            curState = BirdState.GettingReady;
        }

        
        protected Vector3 originalPos;
        protected float jumpUpSpeed;
        protected float jumpArcHeight = 2;
        protected float jumpArrivaltolerance = 0.00001f;
        /// <summary>
        /// Method to jump to sling, following a parabola.
        /// This is called by the Update Method
        /// </summary>
        protected void JumpUpToSling()
        {
            float x0 = originalPos.x;
            float x1 = anchorPoint.position.x;
            float dist = x1 - x0;
            float nextX = Mathf.MoveTowards(transform.position.x, x1, jumpUpSpeed * Time.deltaTime);
            float baseY = Mathf.Lerp(originalPos.y, anchorPoint.position.y, (nextX - x0) / dist);
            float arc = jumpArcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            var nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
            transform.position = nextPos;

            // Do something when we reach the target
            if (Vector3.Distance(nextPos, anchorPoint.position) < jumpArrivaltolerance)
                OnArrivedAtSling();
        }
        /// <summary>
        /// Called when the bird arrives at the sling. 
        /// Now, the bird is ready to listen to user input (drag and shoot).
        /// </summary>
        protected void OnArrivedAtSling()
        {
            body.isKinematic = false;
            springJoint.enabled = true;
            curState = BirdState.Interactable;
        }

        /// <summary>
        /// Called when mouse is pressed down upon the collider of the bird.
        /// Used to achieve the "drag and shoot" feature by setting the IsClicking property.
        /// </summary>
        protected void OnMouseDown()
        {
            if (GameManager.Instance.curLevelStatus != LevelStatus.Ongoing
                && curState != BirdState.Interactable)
                return;
            IsClicking = true;
        }

        /// <summary>
        /// Function to update bird's rigidbody.position to follow mouse clicking point
        /// Constrained by maxDragDistance.
        /// This is called in Update method only when IsClicking is true.
        /// </summary>
        protected void UpdateBirdPos()
        {
            var mainCam = Camera.main;
            var rawInputVector = mainCam.ScreenToWorldPoint(Input.mousePosition)
                - anchorPoint.position //Difference between two positional vectors: anchor point and clicking point
                - Vector3.forward * mainCam.transform.position.z;//Keep position.z as 0 to be able to see it in camera
            body.position = anchorPoint.position
                + Mathf.Clamp(rawInputVector.magnitude, 0, maxDragDistance) //Clamp the distance
                * rawInputVector.normalized; //... and Multiply the distance by its direction unit vector
        }

        /// <summary>
        /// Draws sling belt.
        /// </summary>
        protected void DrawSlingBelt()
        {
            if (curState == BirdState.Interactable || curState == BirdState.BeingPushedBySwing)
            {
                rightLineRenderer.positionCount = 2;
                leftLineRenderer.positionCount = 2;
                rightLineRenderer.SetPosition(0, transform.position);
                rightLineRenderer.SetPosition(1, rightEndPoint.position);
                leftLineRenderer.SetPosition(0, transform.position);
                leftLineRenderer.SetPosition(1, leftEndPoint.position);
            }
            else
            {
                rightLineRenderer.positionCount = 0;
                leftLineRenderer.positionCount = 0;
            }
        }

        /// <summary>
        /// Called when mouse becomes no longer pressing down.
        /// This shoots out the bird.
        /// </summary>
        protected void OnMouseUp()
        {
            if (GameManager.Instance.curLevelStatus != LevelStatus.Ongoing
                && curState != BirdState.Interactable)
                return;
            IsClicking = false;
            curState = BirdState.BeingPushedBySwing;
            //springJoint.frequence defines the spring's frequency in the physics sense
            //Thus, 0.25f / springJoint.frequency calculates a quarter of the spring's period
            //We break the joint and after a quarter of the period
            //This is because we want to release the bird after it bounces back to the anchor point
            StartCoroutine(BreakJoint(0.25f / springJoint.frequency));
            //After a while the game starts to ask for the next bird to get ready
            StartCoroutine(AskForNextBird(intervalBetweenBirds));
        }
        protected IEnumerator BreakJoint(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            curState = BirdState.Free;
            springJoint.enabled = false;
        }
        protected IEnumerator AskForNextBird(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            GameManager.Instance.TryFeedNextBird();
        }

        /// <summary>
        /// This is called by Update method when the bird is free.
        /// </summary>
        protected void WaitForSkill()
        {
            if (Input.GetMouseButton(0)) // left mouse button
            {
                if (hasSkill)//if the bird has skill, then activates its skill
                {
                    DoSkill();
                    curState = BirdState.Skilled;
                }
            }
        }

        /// <summary>
        /// The actual skill execution.
        /// Perform different effects according to the bird type.
        /// </summary>
        protected virtual void DoSkill()
        {
            switch (birdType)
            {
                case BirdType.Red:
                    break;
                case BirdType.Yellow:
                    body.velocity *= 2f;
                    rawDamage *= 2;
                    break;
                case BirdType.Blue:
                    var clone1 = Instantiate(gameObject, transform.position + Vector3.up * 0.5f,
                        transform.rotation, transform.parent);
                    var clone2 = Instantiate(gameObject, transform.position - Vector3.up * 0.5f,
                        transform.rotation, transform.parent);
                    clone1.GetComponent<Rigidbody2D>().isKinematic = false;
                    clone2.GetComponent<Rigidbody2D>().isKinematic = false;
                    clone1.GetComponent<Rigidbody2D>().velocity = body.velocity.Rotate(20);
                    clone2.GetComponent<Rigidbody2D>().velocity = body.velocity.Rotate(-20);
                    clone1.GetComponent<Bird>().curState = BirdState.Skilled;
                    clone2.GetComponent<Bird>().curState = BirdState.Skilled;
                    break;
            }
        }

        /// <summary>
        /// Check whether the bird hits something.
        /// Once it hits something, it no longer can do skills and death countdown starts
        /// </summary>
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if(curState == BirdState.Free || curState == BirdState.Skilled)
            {
                curState = BirdState.Hit;
                //body.constraints = RigidbodyConstraints2D.None;
                StartCoroutine(CountDownDie(lifeTimeAfterHit));
            }
        }

        protected IEnumerator CountDownDie(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Die();
        }

        protected void Die()
        {
            //animator.SetTrigger("Dead");
            EffectHandler.Instance.ShowSmoke(transform.position, 
                SmokeEffectType.Small);
            GameManager.Instance.RemoveBird(this);//Remove this bird from GameManager's list
            GameManager.Instance.CheckLevelFinished();//Check whether level is finished
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Get the raw damage dealt by the bird. This value is then used in calculation of the actual damage.
        /// </summary>
        /// <returns>raw damage dealt by the bird</returns>
        public int GetRawDamage()
        {
            return rawDamage;
        }

    }
}
