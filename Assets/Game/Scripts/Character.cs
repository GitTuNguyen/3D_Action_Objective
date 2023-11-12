using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    private PlayerInput _playerInput;
    private Vector3 _movementValocity;
    private Quaternion _rotateGoal;

    private float _verticalVelocity;
    public float rotataSpeed = 0.1f;
    public float MoveSpeed = 5f;
    public float Gravity = -9.8f;

    public float offSetTimeAirBorne = 0.2f;
    private float _timeAirBorne;
    private HealthManager _healthManage;

    //Attack Slide
    public float TimeStartAttack;
    public float TimeAttackDuration = 0.6f;
    public float AttackSlideSpeed = 0.06f;
    public Vector3 beingHitImpact;

    //Slide
    public float SlideSpeed = 9f;

    //Invisible
    private bool _isInvisible;
    private float _invisibleDuration = 2f;

    //Damage caster
    private DamageCaster _damageCaster;

    //Enemy
    public bool isPlayer = true;
    private NavMeshAgent _navMeshAgent;
    private Transform _targetPlayer;
    
    
    //Material Animation
    MaterialPropertyBlock _materialPropertyBlock;
    SkinnedMeshRenderer _skinnedMeshRenderer;

    //Item drop
    public GameObject itemDrop;
    public int coin = 0;

    //VFX
    private PlayerVFXManager _playerVFXManager;
    private float _attackingAnimationDuration;
    
    //Spawn State
    public float spawnDuration = 2f;
    public float _currentSpawnTime;
    private EnemySpawner _spawner;

    //State Machine simple
    public enum CharacterState
    {
        Normal,
        Slide,
        Attacking,
        BeingHit,
        Dead,
        Spawn
    }
    public CharacterState CurrentState;

    private void Awake() {        
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _healthManage = GetComponent<HealthManager>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _playerVFXManager = GetComponent<PlayerVFXManager>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        _timeAirBorne = 0;
        _currentSpawnTime = 0;
        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            SwitchStateTo(CharacterState.Spawn);
        } else {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
    
    private void CalculatePlayerMovement()
    {
        if (_playerInput.IsLeftMouseDown && _characterController.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        } else if (_playerInput.IsSpaceKeyDown  && _characterController.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        _movementValocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.VerticalInput);
        _movementValocity.Normalize();
        _movementValocity = Quaternion.Euler(0f, -45f, 0) * _movementValocity;
        _animator.SetFloat("Speed", _movementValocity.magnitude);
        _movementValocity *= MoveSpeed * Time.deltaTime;
        if (_characterController.isGrounded)
        {
            _verticalVelocity = Gravity * 0.3f;
        } else {
            _verticalVelocity = Gravity;
        }
        _movementValocity += _verticalVelocity * Vector3.up * Time.deltaTime;
        
        if (_timeAirBorne > offSetTimeAirBorne)
        {
            _animator.SetBool("AirBorne", !_characterController.isGrounded);
        }
        
    }

    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        } else {
            SwitchStateTo(CharacterState.Attacking);
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0.0f);
        }
    }

    private void FixedUpdate() {
        switch(CurrentState)
        {
            case CharacterState.Spawn:
                _currentSpawnTime += Time.deltaTime;
                if (_currentSpawnTime >= spawnDuration)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                    
                } else {
                    CalculateEnemyMovement();            
                }
                break;
            case CharacterState.Slide:
                _movementValocity = transform.forward * SlideSpeed * Time.deltaTime;
                print(_movementValocity);
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.time < TimeStartAttack + TimeAttackDuration)
                    {
                        float timePassed = Time.time - TimeStartAttack;
                        float lerpTime = timePassed / TimeAttackDuration;
                        _movementValocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    if (_playerInput.IsLeftMouseDown)// && _characterController.isGrounded)
                    {
                        string currennAnimtionName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        _attackingAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        print(currennAnimtionName);
                        print (_attackingAnimationDuration);
                        if (currennAnimtionName != "LittleAdventurerAndie_ATTACK_03" && _attackingAnimationDuration > 0.5f && _attackingAnimationDuration < 0.7f)
                        {
                            
                            print("play combo");
                            _playerInput.IsLeftMouseDown = false;
                            //CalculatePlayerMovement();
                            SwitchStateTo(CharacterState.Attacking);
                            
                        }
                    }
                } else {
                    transform.rotation = Quaternion.Slerp(transform.rotation, _rotateGoal, rotataSpeed);
                }
                
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:                
                break;
        }

        if (beingHitImpact.magnitude > 0.2f)
        {
            _movementValocity = beingHitImpact * Time.deltaTime;
        }
        beingHitImpact = Vector3.Lerp(beingHitImpact, Vector3.zero, Time.deltaTime * 5f);

        if (isPlayer)
        {
            if ( _movementValocity.x != 0f &&  _movementValocity.z != 0f  && CurrentState != CharacterState.BeingHit)
            {
                _rotateGoal = Quaternion.LookRotation(new Vector3(_movementValocity.x, 0f, _movementValocity.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, _rotateGoal, rotataSpeed);
            }
            if (!_characterController.isGrounded)
            {
                _timeAirBorne += Time.deltaTime;
            } else {
                _timeAirBorne = 0f;
            }
            if (_timeAirBorne >= offSetTimeAirBorne && _characterController.isGrounded)
            {
                _timeAirBorne = 0f;
            }
            _characterController.Move(_movementValocity);
            _movementValocity = Vector3.zero;
        } else {
            if (CurrentState != CharacterState.Normal)
            {
                _characterController.Move(_movementValocity);
                _movementValocity = Vector3.zero;
            }
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        //Clear input cache
        if (isPlayer)
        {
            _playerInput.ClearCache();
        }      
        
        //ExitState
        switch (CurrentState)
        {
            case CharacterState.Spawn:
                _isInvisible = false;
                break;
            case CharacterState.Normal:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Attacking:
                if(_damageCaster != null)           
                {
                    _damageCaster.DisableDamageCaster();
                }
                if (isPlayer)
                {                    
                    _playerVFXManager.StopAllBlade();
                }
                break;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Dead:
                return;
        }
        
        //EnterState
        switch (newState)
        {
            case CharacterState.Spawn:
                _isInvisible = true;
                StartCoroutine(MaterialApear());
                break;
            case CharacterState.Normal:
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    _rotateGoal = Quaternion.LookRotation(_targetPlayer.position - transform.position);                    
                }
                _animator.SetTrigger("Attack");
                if (isPlayer)
                {
                    TimeStartAttack = Time.time;
                }
                break;
            case CharacterState.BeingHit:                
                _animator.SetTrigger("BeingHit");
                if(isPlayer)
                {
                    _isInvisible = true;
                    StartCoroutine(DelayCancelInvisible());
                }
                
                break;
            case CharacterState.Dead:
                _characterController.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
        }
        CurrentState = newState;
    }

    public void AttackAnimationFinished()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    public void BeingHitAnimationFinished()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void SlideAnimationEnd()
    {
        print("SwitchStateTo(CharacterState.Normal);");
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {
        if (_isInvisible) //can't take dame
        {
            print("Invisible");
            return;
        }

        if (_healthManage != null)
        {
            _healthManage.ApplyDamage(damage);
        }
        if(!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHit(attackerPos);
        }
        StartCoroutine(MaterialBlink());
        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);
        } else 
        {
            AddImpact(attackerPos, 5f);
        }
    }

    IEnumerator DelayCancelInvisible()
    {
        yield return new WaitForSeconds(_invisibleDuration);
        _isInvisible = false;
    }

    private void AddImpact(Vector3 attackPos, float forece)
    {
        Vector3 attackDir = transform.position - attackPos;
        attackDir.Normalize();
        attackDir.y = 0;
        beingHitImpact = attackDir * forece;

    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.5f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialApear()
    {
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHight_Start = -10f;
        float dissolveHight_Target = 20f;
        float dissolveHight_Current;
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight_Current = Mathf.Lerp(dissolveHight_Start, dissolveHight_Target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight_Current);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(1);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHight_Start = 20f;
        float dissolveHight_Target = -10f;
        float dissolveHight_Current;
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight_Current = Mathf.Lerp(dissolveHight_Start, dissolveHight_Target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight_Current);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        DropItem();
        if (_spawner != null)
        {
            _spawner.spawnedEnemyList.Remove(gameObject);
        }
        Destroy(gameObject);
    }

    public void DropItem()
    {
        if (itemDrop != null)
        {
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUp(PickUpItem item)
    {
        switch(item.type)
        {
            case PickUpItem.ItemType.Health:
                if (_healthManage.CanHealing())
                {
                    AddHealth(item.value);
                    Destroy(item.gameObject);
                }
                break;
            case PickUpItem.ItemType.Coin:
                AddCoin(item.value);
                break;
        }
        if (item.type != PickUpItem.ItemType.Health)
        {
            Destroy(item.gameObject);
        }
    }

    private void AddHealth(int value)
    {
        _healthManage.AddHealth(value);
        _playerVFXManager.PlayHealVFX();
    }
    private void AddCoin(int value)
    {
        coin += value;
    }

    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(_targetPlayer, Vector3.up);
        }
    }
    
    public void SetSpawner(EnemySpawner enemySpawner)
    {
        _spawner = enemySpawner;
    }
}
