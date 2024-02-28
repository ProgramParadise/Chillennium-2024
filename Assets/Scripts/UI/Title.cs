using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour
{
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        Invoke("IdleAnimation", 2);
    }

    // Can probably use CrossFadeAlpha instead of this
    void IdleAnimation()
    {
        _animator.SetTrigger("Idle");
    }
}
