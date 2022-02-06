using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary> 
/// �T���v���G1�̍s���ݒ�R���|�[�l���g
/// </summary>
public class BattleOperatorForSample1 : BattleOperatorForEnemy
{
    // Start is called before the first frame update
    //protected override void Start()
    //{
        
    //}

    //// Update is called once per frame
    //protected override void Update()
    //{
        
    //}

    /// <summary> �s���ݒ� </summary>
    protected override void OperateCommand()
    {
        if (_RunningCommand == null)
        {
            switch (_HowToSelect)
            {
                case TargetSelectRule.AtRandom:
                    //�ǂ��炩�������_���ōU��
                    _RunningCommand = StartCoroutine(BodyAttack(ActivePlayers[Random.Range(0, 1)]));
                    break;
            }
        }
    }

    /// <summary> �̓�����U�� </summary>
    /// <param name="target"> ��l�̕W�I </param>
    IEnumerator BodyAttack(BattleOperator target)
    {
        _IsAttacking = true;
        float waitTime = 0.5f;//Random.Range(0.1f, 1f);
        bool isGaveCounter = false;

        //�^�[�Q�b�g���ʂɈړ����Ĉ�莞�ԑҋ@
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(target.BasePosition + (target.transform.forward * 5f), 0.5f).SetEase(Ease.Linear));
        sequence.AppendInterval(waitTime);
        sequence.Play();
        yield return sequence.WaitForCompletion();

        //�̓�����U��
        _IsCounterattacked = false;
        _AttackRatio = 0.4f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(target.BasePosition - (target.transform.forward * 5f), 0.8f).SetEase(Ease.Linear));
        sequence.Play().OnUpdate(() =>
        {
            //�J�E���^�[���󂯂���ADOTween��؂�
            if (TriggerCounterattacked)
            {
                isGaveCounter = true;
                transform.DOKill();

            }
        });
        yield return sequence.WaitForCompletion();

        //�J�E���^�[���󂯂����ۂ��őΉ��������@�Ō��_�ċN
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(_BasePosition - (transform.forward * 5f), 0.05f).SetEase(Ease.INTERNAL_Zero));
        sequence.Append(transform.DOMove(_BasePosition, 0.2f).SetEase(Ease.Linear));
        sequence.Play();

        yield return sequence.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        _IsAttacking = false;
        _Status.IsMyTurn = false;
    }
}