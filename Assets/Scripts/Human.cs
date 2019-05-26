using Battlerock;
using System.Collections;
using UnityEngine;

public enum Gender
{
    Male,
    Female,
    Other
}

public enum HumanState
{
    Idle,
    Walk,
    Flail
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SetRandomColor))]
public class Human : Character
{
    #region Public Variables

    public Color skinColor;
    public Renderer[] bodyPartRenderers;
    public GameObject[] hairStyles;
    public GameObject[] bodyTypes;
    public Gender gender;

    #endregion

    #region Private Variables

    [SerializeField]
    private GameObject m_target;
    private Vector3 m_velocity;
    private Animator m_animator;
    private IEnumerator m_coroutine;
    private HumanState m_state;

    private const float MIN_DISTANCE_TO_TARGET = 1.5f;

    #endregion

    #region Public Properties

    public HumanState State
    {
        get
        {
            return m_state;
        }

        set
        {
            m_animator.SetInteger("State", (int)value);
            m_state = value;
        }
    }

    #endregion

    #region Unity Methods

    // Use this for initialization
    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        var heading = transform.position - (m_target.transform.position);
        var distance = heading.magnitude;
        var direction = heading / distance;

        if (distance < MIN_DISTANCE_TO_TARGET)
        {
            State = HumanState.Idle;
        }

        var modifier = 1.0f;
        var flip = false;

        if (State == HumanState.Idle)
        {
            m_velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }
        else if (State == HumanState.Walk)
        {
            modifier = 1.0f;
            m_velocity = -direction * (modifier * stats.speed * Time.deltaTime);
        }
        else if (State == HumanState.Flail)
        {
            flip = true;
            modifier = 2.0f;
            m_velocity = direction * (modifier * stats.speed * Time.deltaTime);
        }

        //set the rotation vector direction to face
        var moveDi = flip == false
            ? new Vector3(-direction.x, 0, -direction.z)
            : new Vector3(direction.x, 0, direction.z);
        
        //find the wanted rotation angle based on the rotation vector
        Quaternion wantedRotation = Quaternion.LookRotation(moveDi, Vector3.up);

        //set the player's rotation to rotate towards the last inputted rotation vector direction
        transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation,
            stats.rotateSpeed * Time.deltaTime);

        m_velocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = m_velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && State != HumanState.Flail)
        {
            State = HumanState.Flail;
        }


        if (other.gameObject.tag == "Waypoint" && State != HumanState.Idle)
        {
            State = HumanState.Idle;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (State != HumanState.Flail)
            {
                m_target = other.gameObject;
                State = HumanState.Flail;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && State != HumanState.Idle)
        {
            State = HumanState.Idle;
        }
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        m_animator = GetComponent<Animator>();

        if (m_target == null)
        {
            m_target = gameObject;
        }

        SetSkinTone();
        SetBodyType();
        SetHairStyles();
    }

    private void SetBodyType()
    {
        if (bodyTypes == null || bodyTypes.Length <= 0)
        {
            Debug.LogErrorFormat("{0}: NO BODY TYPES SET.", name);
            return;
        }

        var randomBodyIndex = Random.Range(0, bodyTypes.Length);
        bodyTypes[randomBodyIndex].SetActive(true);
    }

    private void SetHairStyles()
    {
        if (hairStyles == null || hairStyles.Length <= 0)
        {
            Debug.LogErrorFormat("{0}: NO HAIR STYLES SET.", name);
            return;
        }

        var randomHairIndex = Random.Range(0, hairStyles.Length + 1);
        if (randomHairIndex < hairStyles.Length)
        {
            hairStyles[randomHairIndex].SetActive(true);
        }
    }

    private void SetSkinTone()
    {
        skinColor = GetComponent<SetRandomColor>().Color;

        if (bodyPartRenderers == null || bodyPartRenderers.Length <= 0)
        {
            Debug.LogErrorFormat("{0}: NO BODY PARTS SET.", name);
            return;
        }

        for (int i = 0; i < bodyPartRenderers.Length; i++)
        {
            bodyPartRenderers[i].material.color = skinColor;
        }
    }

    #endregion
}
