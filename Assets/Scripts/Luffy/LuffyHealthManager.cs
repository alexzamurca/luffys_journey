using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Luffy
{
    public class LuffyHealthManager : MonoBehaviour
    {
        public static float health = 100f;

        [SerializeField] private float decreaseHealthEveryXSeconds;
        private float currentDecreaseTimer;
        [Range(0f, 100f)][SerializeField] private float healthDecreaseAmount;
        [Range(0f, 100f)][SerializeField] private float attackDamage;
        [Range(0f, 100f)][SerializeField] private float healAmount;
        
        [SerializeField] private GameObject healthBarFilling;

        private bool _touchingIce = false;
        
        private void Start()
        {
            currentDecreaseTimer = decreaseHealthEveryXSeconds;
            health = 100f;
        }

        private void Update()
        {
            //Debug.Log("Health:" + (int)health);
            // If die
            if (health <= 0)
            {
                SceneManager.LoadScene(5);
            }
            // If time to decrease health
            if (currentDecreaseTimer <= 0)
            {
                DamageLuffy(true);
                // Reset timer
                currentDecreaseTimer = decreaseHealthEveryXSeconds;
            }
        }

        private void FixedUpdate()
        {
            // Decrease current decrease timer at regular intervals
            if (currentDecreaseTimer > 0)
            {
                currentDecreaseTimer -= Time.deltaTime;
            }
        }

        private void ChangeHealthBarFilling(float change)
        {
            float scaleChange = change * 0.00475f;
            float positionChange = change * 0.0027f;
            // Change scale
            healthBarFilling = ChangeScale(healthBarFilling, new Vector3(scaleChange, 0));
            // Move appropriately
            healthBarFilling.transform.position += new Vector3(positionChange, 0);
        }
        
        private GameObject ChangeScale(GameObject go, Vector3 change)
        {
            Transform parentTransform = go.transform.parent;
            go.transform.parent = null;
            Vector3 localScale = go.transform.localScale;
            localScale += change;
            go.transform.localScale = localScale;
            go.transform.parent = parentTransform;

            return go;
        }

        public void DamageLuffy(bool overtimeDamage)
        {
            float damage;
            if (overtimeDamage) damage = healthDecreaseAmount;
            else damage = attackDamage;
            
            // Decrease health
            health -= damage;
            // Reduce health filling UI
            ChangeHealthBarFilling(-damage);
        }
        
        public void HealLuffy()
        {
            if(health>=99) return;
            float amount = healAmount;
            if (healAmount + health > 100f)
            {
                amount = 100 - health;
            }
            // Increase health
            health += amount;
            // Reduce health filling UI
            ChangeHealthBarFilling(amount);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            GameObject collisionGameObject = other.gameObject;
            if (collisionGameObject.CompareTag("Meat"))
            {
                HealLuffy();
                Destroy(collisionGameObject);
            }
            else if (collisionGameObject.CompareTag("Frozen Cell"))
            {
                if (!_touchingIce)
                {
                    DamageLuffy(false);
                    _touchingIce = true;
                    
                    // Increase number of times luffy got hit
                    PlayerPrefsManager playerPrefsManager = new PlayerPrefsManager();
                    playerPrefsManager.IncrementDamageFromEnemyBosses();
                }
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            GameObject collisionGameObject = other.gameObject;
            if (collisionGameObject.CompareTag("Frozen Cell"))
            {
                if (_touchingIce)
                {
                    _touchingIce = false;
                }
            }
        }
    }
}