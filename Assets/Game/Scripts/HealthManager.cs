using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth;
    [SerializeField]
    private int _currentHealth;
    private Character _character;

    private void Awake() {
        _character = GetComponent<Character>();
        _currentHealth = _maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
        CheckHealth();
    }

    public void AddHealth(int health)
    {
        _currentHealth += health;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_currentHealth <= 0)
        {
            _character.SwitchStateTo(Character.CharacterState.Dead);
        } else if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public bool CanHealing()
    {
        return _currentHealth < _maxHealth;
    }

    public float CurrentHealthPercent()
    {
        return (float)_currentHealth/(float)_maxHealth;
    }
}
