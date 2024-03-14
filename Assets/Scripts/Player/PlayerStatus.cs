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

    // 이동 속도
    private float _movespeed = 4.0f;
    public float movespeed { get { return _movespeed; } }
    // 가속도
    private float _acceleration = 10.0f;
    public float acceleration { get { return _acceleration; } set { _acceleration = value; } }

    // 회피 딜레이
    private float _dodgeDelay = 1.0f;
    public float dodgeDelay { get { return _dodgeDelay; } }

    // 회전 속도
    private float _rotSmoothTime = 0.08f;
    public float rotSmoothTime { get { return _rotSmoothTime; } set { _rotSmoothTime = value; } }

    // 점프높이
    private float _jumpheight = 3.0f;
    public float jumpheight { get { return _jumpheight; } set { _jumpheight = value; } }
    
    // 최대 낙하 가속도
    private float _fallingVelocity = 53f;
    public float fallingVelocity { get { return _fallingVelocity; } }

    // 중력
    private float _gravity = -15f;
    public float gravity { get { return _gravity; } set { _gravity = value; } }

    // 지면 감지 오프셋
    private float _groundOffset = 0.86f;
    public float groundOffeset { get { return _groundOffset; } set { _groundOffset = value;} }

    // 지면 감지 범위
    private float _groundRadius = 0.28f;
    public float groundRadius { get { return _groundRadius; } set { _groundRadius = value; } }

    // 카메라 위쪽 각도 제한
    public float camTopClamp = 50.0f;
    // 카메라 아래쪽 각도 제한
    public float camBottomClamp = -70.0f;
    // 카메라 속도
    public float camSpeed = 2.0f;

    #endregion
}
