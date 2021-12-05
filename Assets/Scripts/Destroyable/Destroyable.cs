/*
Destroyable.cs

Description: The general class for pigs and destructible tiles on the map.
Author: Yu Long
Created: Wednesday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reimirno 
{
    
    public enum DestroyableType //Related to HP threshold and death effect
    {
        Pig_Weak = 0,
        Pig_Standard,
        Pig_Strong,
        Pig_VeryStrong,
        Pig_Ultimate,

        Glass = 100,
        Wood,
        Stone,
    }
    public enum Mass //Related to damage, score (determined by size)
    {
        VeryLight,
        Light,
        Medium,
        Heavy,
        VeryHeavy
    }

    public class Destroyable : MonoBehaviour, IDamageDealer
    {
        #region Data
        [Header("Data")]
        [Tooltip("This is related to HP threshold and death effect.")]
        public DestroyableType type;
        [Tooltip("This is related to damage and score afforded upon death.")]
        public Mass heaviness;

        [Header("Visual")]
        [Tooltip("Pigs will use animator.")]
        public AnimatorOverrideController controller;
        [Tooltip("Tiles don't have animation. For them, state change will just be a simple sprite swap. Put four sprites here for four states")]
        public Sprite[] swapsSprites;
        [Tooltip("Forces tiles to use animator.")]
        public bool forceToUseAnimator = false;

        [Header("Read Only")]
        [ReadOnly] public bool isPig;
        [ReadOnly,SerializeField] protected int rawDamage;
        [ReadOnly, SerializeField] protected List<int> DamageThreshold;
        [ReadOnly, SerializeField] protected int Score;
        [ReadOnly, SerializeField] protected float minimalHurtTriggerVelocity = 1f;
        #endregion

        #region Run-time Parameters
        //Refs
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D body;

        //Properties
        [ReadOnly, SerializeField] protected int _curHP;
        public int CurHP
        {
            get => _curHP;
            set => _curHP = value;
        }
        #endregion

        /// <summary>
        /// Initialization work.
        /// </summary>
        protected virtual void OnEnable()
        {
            DamageThreshold = DataHub.Instance.GetThresholdsByMaterial(type);
            isPig = (int)type < 100;
            Score = DataHub.Instance.GetScoreByheavinessAndType(type, heaviness);
            CurHP = DamageThreshold.Last() + 20;
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            body.mass = DataHub.Instance.GetMassByheaviness(heaviness);
            rawDamage = DataHub.Instance.GetDamageByheaviness(heaviness);
        }
        
        /// <summary>
        /// Keep checking whether something hits it and possibly deal damage.
        /// </summary>
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (ShouldDealDamage(collision))
            {
                GetHurt(collision);
            }
        }

        /// <summary>
        /// Check whether this collision is a valid damage
        /// </summary>
        protected virtual bool ShouldDealDamage(Collision2D collision)
        {
            return collision.collider.GetComponent<IDamageDealer>() != null &&
                collision.relativeVelocity.magnitude > minimalHurtTriggerVelocity;
        }

        /// <summary>
        /// Calculate damage dealt and subtract that from HP.
        /// Formula = Floor(relative_collision_speed * raw_damage_of_the_collider)
        /// </summary>
        protected virtual void GetHurt(Collision2D collision)
        {
            CurHP -= Mathf.FloorToInt(collision.relativeVelocity.magnitude *
                collision.collider.GetComponent<IDamageDealer>().GetRawDamage());
            CheckStateAfterHurt();
        }

        /// <summary>
        /// Circumvent all checks and simply deal damage.
        /// Could be used by some special effects.
        /// </summary>
        public void DealDirectDamage(int damage)
        {
            CurHP -= damage;
            CheckStateAfterHurt();
        }

        /// <summary>
        /// After getting hurt, check whether HP < 0
        /// </summary>
        protected virtual void CheckStateAfterHurt()
        {
            this.Log("Current HP:", CurHP);
            //If HP < 0, then die.
            if (CurHP < 0)
            {
                Die();
                return;
            }

            //If not dead, but hurt... we need to determine its hurt level.
            //For example, if there are 3 elements in DamageThreshold list
            //The maximum levelofHurt is 3.
            int levelOfHurt = DamageThreshold.Count;
            foreach (var threshold in DamageThreshold)
            {
                if (CurHP < threshold)
                {
                    break;
                }
                levelOfHurt--;
            }
            if (levelOfHurt > 0)
            {
                HandleEffect("Hurt" + levelOfHurt); //Hurt1, Hurt2. Hurt3
            }
        }

        protected virtual void Die()
        {
            HandleEffect("Dead");
            GameManager.Instance.AddScore(Score);
            RemoveFromGame();
        }

        /// <summary>
        /// Handle some visual/audio effects of hurt/death
        /// </summary>
        protected virtual void HandleEffect(string state)
        {
            //For pigs, update animator. The parameter trigger has the same name as our state string
            //Hurt1, Hurt2, Hurt3, Dead
            if (isPig || forceToUseAnimator)
            {
                animator.SetTrigger(state);
            }
            else
            {
                //For tiles, do a sprite swap.
                //Take note of the index
                if (state == "Dead")
                    spriteRenderer.sprite = swapsSprites[3];
                else
                    spriteRenderer.sprite = swapsSprites[int.Parse(
                        state.Substring(state.Length - 1, 1))];
            }
            
            if(state == "Dead")
            {
                EffectHandler.Instance.ShowDieEffect(transform.position, type);
                EffectHandler.Instance.ShowScore(transform.position + 0.85f * Vector3.up, Score);
            }
        }

        protected virtual void RemoveFromGame()
        {
            if (isPig)
            {
                GameManager.Instance.RemovePig(this);
                GameManager.Instance.CheckLevelFinished();
            }
            
            gameObject.SetActive(false);
        }

        /// <summary>
        /// A destroyable is itself a damage dealer.
        /// </summary>
        public int GetRawDamage()
        {
            return rawDamage;
        }

    }
}
