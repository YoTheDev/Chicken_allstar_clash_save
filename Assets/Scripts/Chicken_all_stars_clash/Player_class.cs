using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PatternSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Player_class : MonoBehaviour {
    
    [SerializeField] private GameObject playerPivot;
    [SerializeField] private GameObject playerDamage;
    [SerializeField] private Animator animator;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float doubleJumpHeight;
    [SerializeField] public float airattackjumpHeight;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackForceUp;
    [SerializeField] private float maxHealth;
    [SerializeField] private float repeatAttackTime;
    [SerializeField] private float repeatAttackNumber;
    [SerializeField] private Camera_manager camera_script;

    private bool _isJumpPressed;
    private bool _playOneShot;
    private bool _tookDamage;
    private bool _TouchBossAttack;
    private float _damage;
    private float _shieldTimer;
    private int _attackCount;
    private GameObject _boss;
    private GameObject _bossAttack;
    private GameObject _camera;
    public Player_management player_management;
    private Slider _slider01;
    private Slider _slider02;
    private Material[] _skinnedMaterial;
    private Vector3 knockbackDirection;
    private Vector3 ShieldScale;
    private Color LerpedColor = Color.black;
    private ParticleSystem.MainModule smokeParticle;
    private PlayerInput thisPlayerInput;
    private Gamepad _currentController;

    [HideInInspector] public WeaponData _currentWeapon;
    [HideInInspector] public bool _canAirAttack;
    [HideInInspector] public bool _saveAxisXpositive;
    [HideInInspector] public bool _doubleJump;
    [HideInInspector] public float _axisX;
    [HideInInspector] public float _saveSpeed;
    [HideInInspector] public float nearGroundedRange;
    [HideInInspector] public float reloadTimer;
    [HideInInspector] public int weaponIndex;
    [HideInInspector] public int currentPlayerInputIndex;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isNearGrounded;
    [HideInInspector] public bool _isGrounded;
    [HideInInspector] public bool _attack;
    [HideInInspector] public bool _airAttack;
    [HideInInspector] public bool block;
    [HideInInspector] public Rigidbody _rigidbody;
    [HideInInspector] public Collider attackBoxCollider;

    public float playerSpeed;
    public float propulsion;
    public float shieldRemain;
    public float shieldRecharge;
    public GameObject attackBox;
    public GameObject attack2Box;
    public GameObject Shield;
    public GameObject projectile;
    public GameObject deathBalloon;
    public GameObject explosionSmoke;
    public GameObject balloonCharge;
    public GameObject Ground_slash;
    public GameObject Air_slash;
    public List<WeaponData> weapon;
    public Game_management Game_management;
    public TextMeshPro PlayerIndicator;
    public List<string> playerLifeUIstring;

    void Start() {
        thisPlayerInput = GetComponent<PlayerInput>();
        _skinnedMaterial = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        for (int i = 0; i < _skinnedMaterial.Length; i++) {
            _skinnedMaterial[i].EnableKeyword("_EMISSION");
        }
        smokeParticle = GetComponentInChildren<ParticleSystem>().main;
        ShieldScale = Shield.transform.localScale;
        _saveAxisXpositive = true;
        _camera = GameObject.Find("Main Camera");
        camera_script = _camera.GetComponent<Camera_manager>();
        deathBalloon.SetActive(false);
        animator = GetComponentInChildren<Animator>();
        player_management = GameObject.Find("Player_manager").GetComponent<Player_management>();
        currentPlayerInputIndex = GetComponent<PlayerInput>().playerIndex;
        Game_management.playerAlive[currentPlayerInputIndex] = true;
        _slider01 = GameObject.Find(playerLifeUIstring[currentPlayerInputIndex]+"/Health_bar_01").GetComponent<Slider>();
        _slider02 = GameObject.Find(playerLifeUIstring[currentPlayerInputIndex]+"/Health_bar_02").GetComponent<Slider>();
        PlayerIndicator.text = "Player " + (currentPlayerInputIndex + 1);
        attackBox.SetActive(false); attack2Box.SetActive(false);
        _boss = GameObject.FindWithTag("Boss");
        _rigidbody = GetComponent<Rigidbody>();
        _saveSpeed = playerSpeed;
        _slider01.maxValue = maxHealth; _slider02.maxValue = maxHealth;
        _slider01.value = maxHealth; _slider02.value = maxHealth;
        if (weapon.Count == 0) {
            Debug.LogWarning("List for " + gameObject.name + " is set to 0");
            return;
        }
        if (_currentWeapon == null) _currentWeapon = weapon[1];
    }
    
    private void FixedUpdate() {
        isNearGrounded = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), nearGroundedRange);
        if (_axisX > 0 && !_attack && _isGrounded || _axisX < 0 && !_attack && _isGrounded) {
            animator.SetFloat("speed",1);
            CancelInvoke(nameof(WalkOver));
        }
        else { Invoke(nameof(WalkOver),0.2f); }
        if(_slider01.value <= maxHealth / 3) {
            animator.SetBool("low", true);
            LerpedColor = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
            for (int i = 0; i < _skinnedMaterial.Length; i++) {
                _skinnedMaterial[i].SetColor("_EmissionColor",LerpedColor);
            }
        }
        
        if(_slider02.value <= 0) {
            animator.SetBool("almostdead", true);
        }
        
        if (block) {
            _shieldTimer += shieldRemain;
            Shield.transform.localScale += new Vector3(0.1f, .1f, .1f) * (-7f * Time.deltaTime);
            Debug.Log(_shieldTimer);
            if (_shieldTimer >= 100) {
                Shield.transform.localScale = ShieldScale;
                block = false;
                _shieldTimer = 0;
                Shield.SetActive(false);
                gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
                animator.SetInteger("speed",0);
                playerSpeed = 0;
                _slider01.value -= 15;
                player_management.ActivateInput = false;
                Instantiate(explosionSmoke, transform.position, transform.rotation);
                Invoke(nameof(ShieldBroke),5);
            }
        }
        else {
            if (_shieldTimer > 0) {
                Shield.transform.localScale += new Vector3(0.1f, .1f, .1f) * (2.34f * Time.deltaTime);
                _shieldTimer -= shieldRecharge;
                Debug.Log(_shieldTimer);
                if (_shieldTimer <= 0) {
                    Shield.transform.localScale = ShieldScale;
                }
            }
        }

        if (_shieldTimer < 50) smokeParticle.maxParticles = 0;
        if (_shieldTimer >= 50 && _shieldTimer < 60) smokeParticle.maxParticles = 1;
        if (_shieldTimer >= 60 && _shieldTimer < 70) smokeParticle.maxParticles = 5;
        if (_shieldTimer >= 70 && _shieldTimer < 80) smokeParticle.maxParticles = 10;
        if (_shieldTimer >= 80 && _shieldTimer < 90) smokeParticle.maxParticles = 15;
        if (_shieldTimer >= 90 && _shieldTimer < 100) smokeParticle.maxParticles = 20;
        
        if (!isNearGrounded && !isDead) {
            _rigidbody.drag = 3;
            playerSpeed = 100;
        }
        _rigidbody.AddForce(Vector3.left * _axisX * playerSpeed,ForceMode.Force);
        if (_isJumpPressed && _isGrounded) {
            _rigidbody.AddForce(Vector3.up * jumpHeight,ForceMode.Impulse);
            _isGrounded = false;
            _isJumpPressed = false;
            _canAirAttack = true;
            _doubleJump = true;
        }
        if (Game_management.victory && !_playOneShot) {
            player_management.scoreEarned += _slider02.value;
            if (_slider02.value >= maxHealth) {
                player_management.scoreEarned += 1000;
                Debug.Log("No damage bonus");
            }
            _playOneShot = true;
        }
        if (_slider01.value < _slider02.value) _slider02.value -= 0.05f;
        if (isDead) return;
        if (!(_slider02.value <= 0)) return;
        Shield.SetActive(false);
        block = false;
        isDead = true;
        playerSpeed = 10;
        CancelInvoke(nameof(InvulnerabilityEnd));
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
        Game_management._aliveIndex = currentPlayerInputIndex;
        Invoke(nameof(PlayerBigger),2);
    }

    void WalkOver() { animator.SetFloat("speed",0); }

    void PlayerBigger() {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.AddForce(Vector3.up * 100,ForceMode.Impulse);
        Game_management.PlayerDead();
        _rigidbody.mass = 0.02f;
        _currentWeapon = weapon.First();
        playerSpeed = 2;
        _rigidbody.drag = 0.5f;
        playerPivot.SetActive(false);
        deathBalloon.SetActive(true);
        deathBalloon.layer = LayerMask.NameToLayer("IgnoreCollision");
        if (!Game_management.gameOver && !Game_management.victory)
            Invoke(nameof(BalloonCollisionActive), 1);
    }
    void BalloonCollisionActive() { deathBalloon.layer = LayerMask.NameToLayer("Default"); }

    public void OnMove(InputValue Moving) {
        if (!player_management.ActivateInput || player_management.pauseUI.activeSelf) return;
        var rotation = transform.rotation;
        _axisX = Moving.Get<float>();
        if (_axisX < 0) {
            if (!_attack && !_airAttack && !_tookDamage) {
                playerPivot.transform.rotation = Quaternion.Euler(rotation.x,180, rotation.z);
            }
            _saveAxisXpositive = false;
        }
        else if (_axisX > 0) {
            if (!_attack && !_airAttack && !_tookDamage) {
                playerPivot.transform.rotation = Quaternion.Euler(rotation.x,0,rotation.z);
            }
            _saveAxisXpositive = true;
        }
    }

    public void OnJump() {
        if (!player_management.ActivateInput || Game_management.victory || isDead || block || player_management.pauseUI.activeSelf) return;
        if (isNearGrounded && !_attack) {
            animator.SetBool("jump",true);
            animator.SetBool("grounded",false);
            _isJumpPressed = true;
        }
        if (!_isGrounded && _doubleJump) {
            animator.SetBool("double_jump",true);
            animator.SetBool("jump",false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * doubleJumpHeight,ForceMode.Impulse);
            if (_axisX != 0) {
                if (!_saveAxisXpositive) {
                    _rigidbody.AddForce(Vector3.right * propulsion,ForceMode.Impulse);
                }
                else {
                    _rigidbody.AddForce(Vector3.left * propulsion,ForceMode.Impulse);
                }
            }
            _doubleJump = false;
            _isJumpPressed = false;
            _canAirAttack = true;
        }
    }

    void OnBlock() {
        if (!player_management.ActivateInput || block || isDead || player_management.pauseUI.activeSelf) return;
        block = true;
        _currentWeapon.DoBlock(this);
        _currentWeapon.Interrupt(this);
        playerSpeed = _saveSpeed / 2;
    }

    void OnUnBlock() {
        if (!player_management.ActivateInput || isDead || player_management.pauseUI.activeSelf) return;
        playerSpeed = _saveSpeed;
        block = false;
        _currentWeapon.DoUnBlock(this);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
            _currentWeapon.currentAirProjectile = 0;
            _isGrounded = true;
            _tookDamage = false;
            animator.SetBool("jump",false);
            animator.SetBool("double_jump",false);
            animator.SetBool("grounded",true);
            animator.SetBool("attack",false);
            animator.SetBool("damage",false);
            if(!isDead) {
                _rigidbody.drag = 10;
                if (!block) playerSpeed = _saveSpeed;
                AttackCooldown();
            }
        }
        if (other.gameObject.CompareTag("Boss")) {
            _TouchBossAttack = false;
            CollisionBoss();
        }
        if (isDead) {
            _currentWeapon.Interrupt(this);
            balloonCharge.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (isDead) return;
        if (other.gameObject.CompareTag("AttackBoss") && !block) {
            _bossAttack = other.gameObject;
            _TouchBossAttack = true;
            CollisionBoss();
        }
    }

    void CollisionBoss() {
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
        deathBalloon.layer = LayerMask.NameToLayer("IgnoreCollision");
        playerDamage.SetActive(true);
        Invoke(nameof(InvulnerabilityEnd), 1);
        if(isDead) return;
        animator.SetBool("damage",true);
        _rigidbody.velocity = new Vector3(0, 0, 0);
        if(_TouchBossAttack) {
            knockbackDirection = new Vector3(transform.position.x - _bossAttack.transform.position.x, 0);
            _rigidbody.AddForce(knockbackDirection * (knockbackForce + 40),ForceMode.Impulse);
        }
        else {
            knockbackDirection = new Vector3(transform.position.x - _boss.transform.position.x, 0);
            _rigidbody.AddForce(knockbackDirection * knockbackForce,ForceMode.Impulse);
        }
        _rigidbody.AddForce(knockbackDirection * knockbackForce,ForceMode.Impulse);
        _rigidbody.AddForce(Vector3.up * knockbackForceUp,ForceMode.Impulse);
        _currentController = thisPlayerInput.GetDevice<Gamepad>();
        _currentController.SetMotorSpeeds(0,2);
        Invoke(nameof(VibrationDamageEnd),0.1f);
        AttackCooldown();
        Damage();
    }

    void VibrationDamageEnd() { _currentController.SetMotorSpeeds(0,0); }

    void Damage() {
        camera_script.shakeStart = true;
        camera_script.ShakeTime = 0;
        camera_script.ShakeDuration = 0.2f;
        camera_script.ShakeDistance = 1;
        _tookDamage = true;
        if (_TouchBossAttack) _damage = FindObjectOfType<Enemy>().attackDamageCoast;
        else _damage = FindObjectOfType<Enemy>().damageCoast;
        if (_slider01.value > 0) _slider01.value -= _damage;
        else _slider02.value -= _damage;
    }

    void InvulnerabilityEnd() {
        if(block) return;
        gameObject.layer = LayerMask.NameToLayer("Player_one");
        deathBalloon.layer = LayerMask.NameToLayer("Default");
    }

    void ShieldBroke() {
        player_management.ActivateInput = true;
        playerSpeed = _saveSpeed;
        gameObject.layer = LayerMask.NameToLayer("Player_one");
    }

    public void OnAttack() {
        if (_attack || !player_management.ActivateInput || block || player_management.pauseUI.activeSelf) return;
        if (isNearGrounded && !_attack && !_airAttack) {
            animator.SetBool("attack",true);
            attackBoxCollider = attackBox.GetComponent<Collider>();
            _currentWeapon.DoSimple(this);
            if (_currentWeapon.SimpleMultipleDamage) {
                attackBox.SetActive(true);
                InvokeRepeating(nameof(DoSimpleAttack), repeatAttackTime, repeatAttackNumber);
            }
            if (_attackCount <= 0) {
                _attackCount = 1;
                animator.SetInteger("attack_count",1);
            }
            else {
                _attackCount = 0;
                animator.SetInteger("attack_count",0);
            }
            Invoke(nameof(AttackCooldown),reloadTimer);
        }
        else if (_canAirAttack) {
            animator.SetBool("attack",true);
            animator.SetBool("damage",false);
            if(!isDead) _rigidbody.drag = 3;
            _currentWeapon.DoAirSimple(this);
            Invoke(nameof(AttackCooldown),reloadTimer);
        }
    }
    
    void DoSimpleAttack() { _currentWeapon.DoSimple(this); }

    public void AttackCooldown() {
        CancelInvoke(nameof(DoSimpleAttack));
        animator.SetBool("attack",false);
        var rotation = transform.rotation;
        if (!isDead) {
            if (_saveAxisXpositive) playerPivot.transform.rotation = Quaternion.Euler(rotation.x,0,rotation.z);
            else playerPivot.transform.rotation = Quaternion.Euler(rotation.x,180,rotation.z);
            if (!block) playerSpeed = _saveSpeed;
        }
        if(_attack) {
            _attack = false;
            attackBox.SetActive(false);
            Ground_slash.SetActive(false);
        }
        if(_airAttack) {
            _airAttack = false;
            attack2Box.SetActive(false);
            Air_slash.SetActive(false);
        }
    }

    void OnStart()
    {
        if (!player_management.pauseUI.activeSelf && player_management.ActivateInput && !Game_management.gameOver && !Game_management.victory) {
            player_management.pauseUI.SetActive(true);
            player_management.PauseResumeButton.Select();
            Time.timeScale = 0;
        }
    }
}
