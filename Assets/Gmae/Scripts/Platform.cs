using UnityEngine;

public class Platform : MonoBehaviour
{
    public float _movespeed = 2f;
    public float _boundY = 6f;

    public bool _movingplatform, _isbreakable, _isspikes, _isplatform;

    private Animator _anim;

    private void Awake()
    {
        if (_isbreakable)
        {
            _anim = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 temp = transform.position;
        temp.y += _movespeed * Time.deltaTime;
        transform.position = temp;

        if (temp.y >= _boundY)
        {
            gameObject.SetActive(false);
        }
    }

}
