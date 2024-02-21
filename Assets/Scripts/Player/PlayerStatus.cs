using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    #region Action Status

    private int _hp;
    public int hp { get { return _hp; } set { _hp = value; } }

    private int _maxhp;
    public int maxHp { get { return _maxhp; } set { _maxhp = value; } }

    #endregion



    #region General Status

    // 이동 속도
    private float _movespeed = 2.0f;
    public float movespeed { get { return _movespeed; } }
    // 가속도
    private float _acceleration = 10.0f;
    public float acceleration { get { return _acceleration; } set { _acceleration = value; } }


    // 회전 속도
    private float _rotSmoothTime = 0.12f;
    public float rotSmoothTime { get { return _rotSmoothTime; } set { _rotSmoothTime = value; } }

    // 점프높이
    private float _jumpheight = 1.2f;
    public float jumpheight { get { return _jumpheight; } set { _jumpheight = value; } }
    
    // 최대 낙하 가속도
    private float _fallingVelocity = 53f;
    public float fallingVelocity { get { return _fallingVelocity; } }

    // 중력
    private float _gravity = -15f;
    public float gravity { get { return _gravity; } set { _gravity = value; } }

    // 지면 감지 오프셋
    private float _groundOffset;
    public float groundOffeset { get { return _groundOffset; } set { _groundOffset = value;} }
    
    // 지면 감지 범위
    private float _groundRadius;
    public float groundRadius { get { return _groundRadius; } set { _groundRadius = value; } }


    #endregion
}
