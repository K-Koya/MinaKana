using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーとなるキャラクターの能力値
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerStatus : CharacterStatus , ICSVDataConverter
{
    #region 定数
    /// <summary> パラメーター名:SP </summary>
    public const string PARAMETER_NAME_SP = "SP";
    #endregion

    /// <summary> キャラクターのRigidbody </summary>
    Rigidbody _RB = default;

    [SerializeField, Tooltip("ジャンプ力")]
    float _JumpPower = 15.0f;

    [SerializeField, Tooltip("true : 地面についている")]
    bool _IsGrounded = false;

    [SerializeField, Tooltip("最大SP(スペシャルアタックの消費ポイント)")]
    short _SPInitial = 100;

    [SerializeField, Tooltip("現在SP(スペシャルアタックの消費ポイント)")]
    short _SPCurrent = 100;

    #region プロパティ
    /// <summary> 最大SP </summary>
    public override short SPInitial { get => _SPInitial; set => _SPInitial = value; }
    /// <summary> 現在SP </summary>
    public override short SPCurrent { get => _SPCurrent; set => _SPCurrent = value; }
    /// <summary> true : 地面についている </summary>
    public bool IsGrounded { get => _IsGrounded; }

    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CalculateParameters();
        _HPCurrent = _HPInitial;
        _SPCurrent = _SPInitial;
        _RB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //重力落下
        if (!_RB.isKinematic)
        {
            Vector3 gravityAccelarate = -transform.up * _JumpPower * 2.5f;
            _RB.AddForce(gravityAccelarate, ForceMode.Acceleration);
        }
    }

    void CalculateParameters()
    {
        //データテーブルよりステータス一式を取得
        List<string> data = DataTableCharacter.I.GetDataUsingName(_Name);

        //ID番号取得
        _CharacterNumber = byte.Parse(data[1]);

        //計算したうえで格納
        _HPInitial = short.Parse(data[2]);
        _SPInitial = short.Parse(data[3]);
        _Attack = short.Parse(data[4]);
        _Defense = short.Parse(data[5]);
        _Rapid = short.Parse(data[6]);
        _Technique = short.Parse(data[7]);
    }

    /// <summary>
    /// ジャンプさせる
    /// </summary>
    /// <param name="powerRatio">ジャンプ力倍率</param>
    /// <param name="canAirial">true : 空中でもジャンプが可能</param>
    public void DoJump(float powerRatio, bool canAirial = false)
    {
        if (_IsGrounded || canAirial)
        {
            //落下速度を殺す
            Vector3 velocity = Vector3.ProjectOnPlane(_RB.velocity, transform.up);
            _RB.velocity = velocity;

            //ジャンプ力を計算して加算
            Vector3 jumpAccelarate = transform.up * _JumpPower * powerRatio;
            _RB.AddForce(jumpAccelarate, ForceMode.VelocityChange);
        }
    }

    /// <summary> 主に地面にいる判定に利用 </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        //地形レイヤとの接触
        if (collision.gameObject.CompareTag("Ground"))
        {
            //うち、足元付近での接触
            Vector3[] points = collision.contacts.Select(c => c.point).ToArray();
            if(points.Where(p => (p - (transform.position + transform.up * 0.1f)).y < 0).ToArray().Length > 0)
            {
                _IsGrounded = true;
            }
        }
    }

    /// <summary> 主に地面にいる判定に利用 </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        //地形レイヤとの接触
        if (collision.gameObject.CompareTag("Ground"))
        {
            _IsGrounded = false;
        }
    }

    void ICSVDataConverter.CSVToMembers(List<string> csv)
    {
        throw new System.NotImplementedException();
    }

    List<string> ICSVDataConverter.MembersToCSV()
    {
        throw new System.NotImplementedException();
    }
}

