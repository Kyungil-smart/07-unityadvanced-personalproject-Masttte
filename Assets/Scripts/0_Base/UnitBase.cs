using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[Flags]
public enum Buff : byte
{
    None = 0,
    AtkUp = 1 << 0,
    DefUp = 1 << 1,
}

[Flags]
public enum Debuff : byte
{
    None = 0,
    AtkDown = 1 << 0,
    DefDown = 1 << 1,
}
public abstract class UnitBase : MonoBehaviour, IDamagable, IPointerDownHandler
{
    [Header("정보")]
    public Nexus nexus;
    public UnitSO data;

    [Header("Stats")]
    [HideInInspector] public int maxHp;
    [HideInInspector] public int maxLucidity;
    public int _hp;  // virtual 프로퍼티를 위한 public
    public virtual int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hp <= 0)
            {
                _hp = 0;
                Die().Cancel();
            }
            ui?.UpdateHp(_hp);
        }
    }
    public int _lucidity;
    public virtual int Lucidity
    {
        get => _lucidity;
        set
        {
            _lucidity = value;
            if (_lucidity <= 0)
            {
                _lucidity = 0;
            }
            ui?.UpdateLucidity(_lucidity);
        }
    }
    public int atk;
    public int def;

    [Header("위치")]
    public int root;
    public int[] offsets = { 0 }; // 유닛이 점유하는 위치 배열. 0부터 시작 (연속된 두칸은 0, 1 / 특수한 형태 (0, 2, 4)가능)
    public int size => offsets.Max(); // 유닛이 점유하는 길이 (0-base)
    public bool isLookLeft => sr.flipX;

    [Header("상태")]
    [HideInInspector] public bool isActing;
    [HideInInspector] public bool isDead;
    public bool isRange;
    public Buff buff;
    public Debuff debuff;

    [Header("행동")]
    public Behavior[] behaviors;

    // 참조
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator ani;

    // UI
    [HideInInspector] public UnitUI ui;
#if UNITY_EDITOR
    private void Reset()
    {
        //같은 이름의 데이터 할당
        string _name = name.Split('_')[0];

        data = Resources.Load<UnitSO>($"1_Lucid/{_name}/{_name}");
        if (data == null)
        {
            data = Resources.Load<UnitSO>($"2_Nightmare/{_name}/{_name}");
            if (data == null)
            {
                data = Resources.Load<UnitSO>($"3_Nature/{_name}/{_name}");
                if (data == null)
                {
                    Debug.LogWarning($"'{name}' SO를 찾을 수 없습니다");
                }
            }
        }
        behaviors = Resources.LoadAll<Behavior>($"1_Lucid/{_name}");
        if (behaviors.Length == 0)
        {
            behaviors = Resources.LoadAll<Behavior>($"2_Nightmare/{_name}");
            if (behaviors.Length == 0)
            {
                behaviors = Resources.LoadAll<Behavior>($"3_Nature/{_name}");
                if (behaviors.Length == 0)
                {
                    Debug.LogWarning($"'{name}' Behavior를 찾을 수 없습니다");
                }
            }
        }

        nexus = data.nexus;
        maxHp = data.hp; Hp = data.hp;
        maxLucidity = data.lucidity; Lucidity = data.lucidity;
        atk = data.atk;
        def = data.def;
    }
#endif

    #region 초기화
    public void Init()
    {
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        // 위치
        root = Mathf.RoundToInt(transform.position.x - (size / 2f));

        // inject (Instantiate로 복제하여 유닛마다 독립 인스턴스를 가짐)
        for (int i = 0; i < behaviors.Length; i++)
        {
            behaviors[i] = Instantiate(behaviors[i]);
            behaviors[i].owner = this;

            for (int j = 0; j < behaviors[i].conditions.Length; j++)
            {
                behaviors[i].conditions[j] = Instantiate(behaviors[i].conditions[j]);
                behaviors[i].conditions[j].behavior = behaviors[i];
            }

            for (int j = 0; j < behaviors[i].actions.Length; j++)
            {
                behaviors[i].actions[j] = Instantiate(behaviors[i].actions[j]);
            }
        }

        maxHp = data.hp; Hp = data.hp;
        maxLucidity = data.lucidity; Lucidity = data.lucidity;
        atk = data.atk;
        def = data.def;

        ui = GetComponent<UnitUI>();
        ui?.Init(maxHp, Hp, maxLucidity, Lucidity);
    }


    protected virtual void OnEnable()
    {
        Map.Instance.Put(this);
        Map.Instance.AddNexus(this);
    }
    protected virtual void OnDisable()
    {
        // todo : 사망 보험금 처리
    }
    #endregion

    #region 위치
    /// <summary>
    /// 현재 root와 오프셋을 조합하여 점유 중인 모든 그리드 번호를 반환합니다.
    /// </summary>
    public IEnumerable<int> GetOccupied()
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            yield return root + offsets[i];
        }
    }
    public IEnumerable<int> GetReverseOccupied()
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            yield return (root + size) - offsets[i];
        }
    }
    #endregion

    #region 상태
    public virtual void TakeDamage(int amount)
    {
        ui?.ShowDamageText(amount - def);
        Hp -= (amount - def);
    }
    #endregion

    #region 행동
    /// <summary>
    /// 이동 가능 여부를 체크합니다. 작은 유닛이 큰 유닛 방향으로 이동하려 할 때 불가능합니다.
    /// </summary>
    public bool CanMove(int dir)
    {
        int pushDir = dir > 0 ? 1 : -1;
        int checkStart = pushDir > 0 ? root + size + 1 : root - 1;
        int checkEnd   = pushDir > 0 ? root + size + dir : root + dir;

        for (int pos = checkStart; pushDir > 0 ? pos <= checkEnd : pos >= checkEnd; pos += pushDir)
        {
            UnitBase target = Map.Instance.GetUnitAt(pos);
            if (target != null && target.size > this.size)
            {
                // 작은 유닛이 큰 유닛 방향으로 이동 불가
                return false;
            }
        }
        return true;
    }

    public virtual void Move(int dir, float time = 0.15f)
    {
        if (!CanMove(dir)) return;

        Map.Instance.Remove(this);
        int oldRoot = root;
        this.root += dir;
        int pushDir = dir > 0 ? 1 : -1;

        // 이동 후 이 유닛이 점유할 모든 위치를 계산
        HashSet<int> newOccupied = new HashSet<int>();
        foreach (int pos in GetOccupied())
        {
            newOccupied.Add(pos);
        }

        // 밀려날 유닛들을 수집 (중복 제거)
        HashSet<UnitBase> unitsToPush = new HashSet<UnitBase>();
        foreach (int pos in newOccupied)
        {
            UnitBase unit = Map.Instance.GetUnitAt(pos);
            if (unit != null && unit != this)
            {
                unitsToPush.Add(unit);
            }
        }

        // 밀려나는 유닛들을 이동 방향 반대쪽으로 밀어냄
        // 이동하는 유닛의 size + 1 만큼 밀어내야 큰 유닛 전체를 피할 수 있음
        int pushAmount = size + 1;

        foreach (UnitBase unit in unitsToPush)
        {
            Map.Instance.Remove(unit);
            unit.root -= pushDir * pushAmount;
            unit.transform.DOLocalMoveX(unit.transform.localPosition.x - pushDir * pushAmount, time).SetEase(Ease.OutCubic);
            Map.Instance.Put(unit);
        }

        transform.DOLocalMoveX(transform.localPosition.x + dir, time).SetEase(Ease.OutCubic);
        Map.Instance.Put(this);
    }
    public virtual void Flip()
    {
        sr.flipX = !sr.flipX;
    }

    public virtual async Awaitable Die()
    {
        if (isDead) return;
        isDead = true;

        Map.Instance.Remove(this);
        Map.Instance.RemoveNexus(this);

        ani.SetTrigger("die");
        await Awaitable.EndOfFrameAsync(); // SetTrigger 대기
        float clipLength = ani.GetNextAnimatorStateInfo(0).length;
        await Awaitable.WaitForSecondsAsync(clipLength);

        gameObject.SetActive(false);
    }

    #endregion

    #region UI
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) GameManager.Instance.OnUnitClick(this);
    }
    #endregion
}
