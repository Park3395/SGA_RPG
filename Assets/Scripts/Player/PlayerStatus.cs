using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    #region Action Status

    private int _hp = 100;
    public int hp { get { return _hp; } set { _hp = value; } }

    private int _maxhp = 100;
    public int maxHp { get { return _maxhp; } set { _maxhp = value; } }

    private bool[] _isWeaponed = new bool[6];
    public bool[] isWeaponed { get { return _isWeaponed; } set { _isWeaponed = value; } }

    #endregion



    #region General Status

    // �̵� �ӵ�
    private float _movespeed = 4.0f;
    public float movespeed { get { return _movespeed; } }
    // ���ӵ�
    private float _acceleration = 10.0f;
    public float acceleration { get { return _acceleration; } set { _acceleration = value; } }

    // ȸ�� ������
    private float _dodgeDelay = 1.0f;
    public float dodgeDelay { get { return _dodgeDelay; } }

    // ȸ�� �ӵ�
    private float _rotSmoothTime = 0.08f;
    public float rotSmoothTime { get { return _rotSmoothTime; } set { _rotSmoothTime = value; } }

    // ��������
    private float _jumpheight = 3.0f;
    public float jumpheight { get { return _jumpheight; } set { _jumpheight = value; } }
    
    // �ִ� ���� ���ӵ�
    private float _fallingVelocity = 53f;
    public float fallingVelocity { get { return _fallingVelocity; } }

    // �߷�
    private float _gravity = -15f;
    public float gravity { get { return _gravity; } set { _gravity = value; } }

    // ���� ���� ������
    private float _groundOffset = 0.86f;
    public float groundOffeset { get { return _groundOffset; } set { _groundOffset = value;} }

    // ���� ���� ����
    private float _groundRadius = 0.28f;
    public float groundRadius { get { return _groundRadius; } set { _groundRadius = value; } }

    // ī�޶� ���� ���� ����
    public float camTopClamp = 50.0f;
    // ī�޶� �Ʒ��� ���� ����
    public float camBottomClamp = -70.0f;
    // ī�޶� �ӵ�
    public float camSpeed = 2.0f;

    #endregion
}
