using FavorKim;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FruitTree : MonoBehaviour
{

    
    [SerializeField] private GameObject Fruit;  // 과일나무에 열리는 과일의 종류 ('과일' 이라는 클래스가 있다면 GameObject가 아니라 해당 클래스로 바꿀 것)
    
    private WaitForSeconds ripCoolWFS;      // 캡슐화 방지를 위한 WaitForSeconds 클래스
    private float ripCoolTime = 5.0f;       // 재채집 대기시간
    private bool ripCoolRunning = false;    // 채집 대기시간 여부

    private void Start()
    {
        ripCoolWFS = new WaitForSeconds(ripCoolTime);
    }
    private void OnEnable()
    {
        RegistOnStartStage();
    }
    private void OnDisable()
    {
        UnRegistOnStartStage();
    }

    public void RegistOnStartStage()
    {
        EventManager.Instance.AddListenerAfter("OnStartStage", OnStartStage_FruitTree, "SetTerrainData");
    }
    public void UnRegistOnStartStage()
    {
        EventManager.Instance.RemoveListener("OnStartStage", OnStartStage_FruitTree);
    }

    private void OnStartStage_FruitTree()
    {
        ResetFruit();
    }

    private void ResetFruit()
    {
        StopAllCoroutines();
        ripCoolRunning = false;
    }

    
    //  과일나무를 채집했을 때 호출할 콜백
    public void OnRipped()
    {
        // 쿨타임이 돌고있을 때는
        if (ripCoolRunning == true)
        {
            // 채집 불가시 호출할 콜백
            OnRipped_CanNotRip();
        }
        // 쿨타임이 안 돌고있을 때는
        else
        {
            // 채집 가능시 호출할 콜백
            OnRipped_CanRip();
        }
    }

    private void OnRipped_CanNotRip() 
    {
        // 채집할 수 없음을 알리는 콜백 (X 아이콘을 띄우는 등)
    }
    private void OnRipped_CanRip() 
    {
        // 쿨타임 시작
        StartCoroutine(CorRipCool());
    }

    IEnumerator CorRipCool()
    {
        ripCoolRunning = true;
        yield return ripCoolWFS;
        ripCoolRunning = false;
    }
}
