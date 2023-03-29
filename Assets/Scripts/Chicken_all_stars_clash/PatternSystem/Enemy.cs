using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace PatternSystem {
    public class Enemy : MonoBehaviour {

        public bool RandomizePattern;
        public List<PatternAction> Pattern;
        public Game_management gameManagement;
        public Camera_manager camera_script;
        public List<GameObject> target = new List<GameObject>();
        public float turnSpeed = .01f;
        public float maxHealth;
        public GameObject Turn_wave;

        [HideInInspector] public Rigidbody Rigidbody;
        [HideInInspector] public Animator animator;
        [HideInInspector] public bool Knockback;
        [HideInInspector] public bool Turn;
        [HideInInspector] public bool Jump;
        [HideInInspector] public int _currentPatternIndex;
        [HideInInspector] public GameObject player;
        [HideInInspector] public GameObject Wall;
        [HideInInspector] public float damageCoast;
        [HideInInspector] public float attackDamageCoast;

        [SerializeField] private float deathKnockback;
        [SerializeField] private float deathKnockbackUp;
        [SerializeField] private float deathKnockbackBack;
        [SerializeField] private float shakeDistance;
        [SerializeField] private float shakeHealthBarDistance;
        [SerializeField] private GameObject graphics;
        [SerializeField] private GameObject graphicsPivot;
        [SerializeField] private GameObject HideBarImage;
        [SerializeField] private GameObject HealthBar;
        [SerializeField] private GameObject damageFeedback;
        [SerializeField] private GameObject deathFeedback;
        [SerializeField] private GameObject Shock_wave_prefab;
        [SerializeField] private GameObject Shock_wave_01;
        [SerializeField] private GameObject Shock_wave_02;
        [SerializeField] private Slider Slider;
        [SerializeField] private TextMeshProUGUI Health_text;
        [SerializeField] private TextMeshProUGUI Health_text_02;
        
        public PatternAction _currentPatternAction;
        private int[] _PatternIndex = {0,4};
        private List<int> _RandomTarget = new List<int> {0,1,2,3};
        public int _afterAction;
        public int _rngPlayer;
        private float _currentHealth;
        private float _patternTimer;
        private float fixedDeltaTime;
        public float ShakeTimer = 1;
        private bool _isDead;
        public bool _enemyReady;
        private Quaternion _rotGoal;
        private Vector3 _direction;
        private Vector3 startPos;
        private Vector3 healthBarStartPos;

        private void Start() {
            animator = GetComponentInChildren<Animator>();
            _currentHealth = maxHealth;
            Slider.maxValue = maxHealth;
            Slider.value = maxHealth;
            Health_text.text = _currentHealth.ToString();
            Health_text_02.text = maxHealth.ToString();
            HideBarImage.SetActive(false);
            Rigidbody = GetComponent<Rigidbody>();
            Physics.gravity = new Vector3(0, -180f, 0);
            fixedDeltaTime = Time.fixedDeltaTime;
            healthBarStartPos = HealthBar.transform.position;
            startPos = graphicsPivot.transform.position;
        }

        public void EnemyStart() {
            if (!_enemyReady) {
                for (int i = 0; i < gameManagement.playerAlive.Count; i++) {
                    if(!gameManagement.playerAlive[i]) continue;
                    target.Add(GameObject.FindWithTag("Player"));
                }
                _enemyReady = true;
            }
        }

        private void Update() {
            if(ShakeTimer < 1) ShakeTimer += Time.deltaTime;
            if (ShakeTimer < 0.2) {
                startPos = graphicsPivot.transform.position;
                graphics.transform.position = startPos + Random.insideUnitSphere * shakeDistance;
                HealthBar.transform.position = healthBarStartPos + Random.insideUnitSphere * shakeHealthBarDistance;
            }
            else {
                HealthBar.transform.position = healthBarStartPos;
            }
            if(!_enemyReady) return;
            if (Pattern.Count == 0) { Debug.LogWarning("List for " + gameObject.name + " is set to 0"); return; }
            if (!_isDead && !gameManagement.gameOver) {
                if (_currentPatternAction == null || _currentPatternAction.IsFinished(this) &&
                    _patternTimer >= _currentPatternAction.PatternDuration) {
                    _currentPatternAction = RandomizePattern ? GetRandomPatternAction() : GetNextPatternAction();
                    _currentPatternAction.Do(this);
                    _patternTimer = 0;
                    damageCoast = _currentPatternAction.PatternDamage;
                }
                _patternTimer += Time.deltaTime;
            }
            if (Turn) {
                Vector3 posTarget = target[_rngPlayer].transform.position;
                Vector3 posOrigin = transform.position;
                Quaternion rotOrigin = transform.rotation;
                _direction = (posTarget - posOrigin).normalized;
                _direction.y = 0; _direction.z = 0;
                _rotGoal = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.Slerp(rotOrigin,_rotGoal,turnSpeed * Time.deltaTime);
            }
            if (!gameManagement.playerAlive[gameManagement._aliveIndex] && !target[gameManagement._aliveIndex]) {
                target[gameManagement._aliveIndex] = null;
                _RandomTarget.Remove(_RandomTarget[gameManagement._aliveIndex]);
            }
            if (_currentHealth <= maxHealth / 3) {
                HideBarImage.SetActive(true);
            }
        }

        public void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Player") && !Knockback) {
                player = other.gameObject;
                _currentPatternAction.isCollided(this);
            }
            else if (other.gameObject.CompareTag("Wall")) {
                Wall = other.gameObject;
                _currentPatternAction.isCollidedWall(this);
            }
            if (other.gameObject.CompareTag("Ground")) {
                animator.SetBool("grounded",true);
                animator.SetInteger("attack",0);
                Turn_wave.SetActive(false);
                if (Knockback) {
                    Rigidbody.velocity = Vector3.zero;
                    _patternTimer = _currentPatternAction.PatternDuration;
                    Knockback = false;
                    animator.SetBool("collided",false);
                }
                if (Turn) {
                    transform.rotation = Quaternion.LookRotation(_direction);
                }
                int randomNumber = Random.Range(0, target.Count);
                _rngPlayer = _RandomTarget[randomNumber];
                Rigidbody.velocity = Vector3.zero;
                Turn = false;
                camera_script.shakeStart = true;
                camera_script.ShakeTime = 0;
                if (Jump) {
                    Instantiate(Shock_wave_prefab, new Vector3(transform.position.x,transform.position.y - 2,transform.position.z), transform.rotation);
                    Shock_wave_01 = GameObject.Find("Shock_wave_01");
                        Shock_wave_02 = GameObject.Find("Shock_wave_02");
                    Shock_wave_01.SetActive(true);
                        Shock_wave_02.SetActive(true);
                    Rigidbody rbShockWave_01 = Shock_wave_01.GetComponent<Rigidbody>();
                        Rigidbody rbShockWave_02 = Shock_wave_02.GetComponent<Rigidbody>();
                    rbShockWave_01.AddForce(Vector3.left * 20, ForceMode.Impulse);
                        rbShockWave_02.AddForce(Vector3.right * 20, ForceMode.Impulse);
                    Shock_wave_01.name = "Shock_wave_03";
                        Shock_wave_02.name = "Shock_wave_04";
                    Shock_wave_01.transform.parent = null;
                        Shock_wave_02.transform.parent = null;
                    GameObject Shock_wave_Instantiate = GameObject.Find("Shock_Wave(Clone)");
                    Destroy(Shock_wave_Instantiate);
                    Shock_wave_01 = null;
                        Shock_wave_02 = null;
                        Jump = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Attack") || other.gameObject.CompareTag("DeathBalloon") || other.gameObject.CompareTag("Shield")) {
                ShakeTimer = 0;
                float damage = other.GetComponentInParent<Player_class>()._currentWeapon.DamageData;
                float score = other.GetComponentInParent<Player_class>()._currentWeapon.ScoreData;
                Player_management playerManagement =
                    GameObject.Find("Player_manager").GetComponent<Player_management>();
                playerManagement.scoreEarned += score;
                Instantiate(damageFeedback, transform.position, transform.rotation);
                _currentHealth -= damage;
                Slider.value -= damage;
                Debug.Log(_currentHealth);
                if (_currentHealth > maxHealth / 3) Health_text.text = _currentHealth.ToString();
                else Health_text.text = "???";
            }
            if (other.gameObject.CompareTag("Projectile")) {
                ShakeTimer = 0;
                float damage = other.GetComponentInParent<projectile>().damage;
                Instantiate(damageFeedback, transform.position, transform.rotation);
                _currentHealth -= damage;
                Slider.value -= damage;
                Debug.Log(_currentHealth);
                if (_currentHealth > maxHealth / 3) Health_text.text = _currentHealth.ToString();
                else Health_text.text = "???";
            }
            if (_currentHealth <= 0) {
                animator.SetBool("Dead",true);
                Instantiate(deathFeedback,transform.position,transform.rotation);
                _isDead = true;
                player = other.gameObject;
                Vector3 posTarget = player.transform.position;
                Vector3 posOrigin = transform.position;
                _direction = (posTarget - posOrigin).normalized;
                _direction.y = 0; _direction.z = 0;
                transform.rotation = Quaternion.LookRotation(_direction);
                Rigidbody.velocity = Vector3.zero;
                Vector3 knockbackDirection = new Vector3(posOrigin.x - posTarget.x, 0);
                Rigidbody.AddForce(knockbackDirection * deathKnockback,ForceMode.Impulse);
                Rigidbody.AddForce(Vector3.up * deathKnockbackUp,ForceMode.Impulse);
                Rigidbody.AddForce(Vector3.back * deathKnockbackBack,ForceMode.Impulse);
                gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
                Time.timeScale = 0.05f;
                Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
                Invoke(nameof(NormalizeTime),0.05f);
            }
        }

        void NormalizeTime() {
            Time.timeScale = 1;
            Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
            gameManagement.Victory();
        }

        private PatternAction GetRandomPatternAction() {
            switch (_afterAction) {
                case 0:
                    int randomNumber = Random.Range(0, _PatternIndex.Length);
                    _currentPatternIndex = _PatternIndex[randomNumber];
                    _afterAction++;
                    break;
                case 1:
                    _currentPatternIndex++;
                    _afterAction++;
                    break;
                case 2:
                    _currentPatternIndex = Pattern.Count - 1;
                    _afterAction = 0;
                    break;
                case 3:
                    _currentPatternIndex = 2;
                    _afterAction++;
                    break;
                case 4:
                    _currentPatternIndex = 3;
                    _afterAction = 0;
                    break;
            }
            return Pattern[_currentPatternIndex];
        }

        private PatternAction GetNextPatternAction() {
            _currentPatternIndex++;
            if (_currentPatternIndex >= Pattern.Count) _currentPatternIndex = 0;
            return Pattern[_currentPatternIndex];
        }
    }
}
