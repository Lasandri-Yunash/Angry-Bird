using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryBird : MonoBehaviour
{
    [SerializeField] private AudioClip _hitClip;

    // References to the Rigidbody2D and CircleCollider2D components

    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;

    private bool _hasBeenLaunched;
    private bool _shouldFaceVelDirction;

    private AudioSource _audioSource;

    private void Awake()
    {
        // Get the Rigidbody2D and CircleCollider2D components

        _rb = GetComponent<Rigidbody2D>();
        _circleCollider=GetComponent<CircleCollider2D>();

        _audioSource = GetComponent<AudioSource>();
       
    }

    private void Start()
    {

        // Set the Rigidbody2D to kinematic and disable the collider initially

        _rb.isKinematic = true;
        _circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_hasBeenLaunched && _shouldFaceVelDirction)
        {

            transform.right = _rb.velocity;

        }
        
    }
    public void LaunchBird(Vector2 direction ,float force)
    {

        _rb.isKinematic = false;
        _circleCollider.enabled = true;

        _rb.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
        _shouldFaceVelDirction = true;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        _shouldFaceVelDirction = false;
        SoubdManager.instance.PlayClip(_hitClip, _audioSource);

        Destroy(this);
    }
}
