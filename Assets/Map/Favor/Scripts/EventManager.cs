using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace FavorKim
{
    public class EventManager : MonoBehaviour
    {
        #region 변수
        private static EventManager instance;
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<EventManager>();
                    if(instance == null)
                    {
                        instance = new GameObject("EventManager").AddComponent<EventManager>();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 이벤트 핸들러 딕셔너리 입니다.
        /// 이벤트 핸들러의 이름(string)을 Key로,
        /// 이벤트 핸들러에 함수들을 등록시킬 연결 리스트를 Value로 갖습니다.
        /// </summary>
        private Dictionary<string, LinkedList<UnityAction>> eventHandlerDict = new Dictionary<string, LinkedList<UnityAction>>();

        /// <summary>
        /// 함수의 이름을 Key, 콜백을 Value로 갖는 딕셔너리입니다.
        /// 이를 통해 이벤트 매니저는 이벤트 매니저를 통해 등록한 콜백과 콜백들의 이름을 모두 알게되므로,
        /// 이후, 다른 객체의 콜백들의 정보가 필요할 때 해당 딕셔너리를 통해 해당 콜백에 접근이 가능합니다.
        /// </summary>
        private Dictionary<string, UnityAction> callBackDict = new Dictionary<string, UnityAction>();

        #endregion

        #region 유니티 이벤트
        
        #endregion







        #region 이벤트 구독/해제부
        /// <summary>
        /// 이벤트 구독 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러의 이름</param>
        /// <param name="action">이벤트 핸들러에 구독할 메서드</param>
        /// <param name="already">해당 콜백이 이미 이벤트 핸들러에 구독되어있는지에 대한 여부</param>
        public void AddListener(string handlerName, UnityAction action, out bool already)
        {

            // 이벤트 핸들러가 이벤트 핸들러 딕셔너리에 구독되어있지 않으면
            if (!eventHandlerDict.ContainsKey(handlerName))
            {
                // 새로 생성 후 이벤트 핸들러 딕셔너리에 구독
                LinkedList<UnityAction> list = new LinkedList<UnityAction>();
                eventHandlerDict.Add(handlerName, list);
            }

            // 이벤트 핸들러에 콜백이 구독되어있지 않다면
            if (!eventHandlerDict[handlerName].Contains(action))
            {
                LinkedListNode<UnityAction> node = new LinkedListNode<UnityAction>(action);

                // 이벤트 핸들러에 구독
                eventHandlerDict[handlerName].AddLast(action);

                // 이벤트 핸들러에 구독되어있지 않은 함수였으므로 already변수는 false
                already = false;
            }
            // 이벤트 핸들러에 콜백이 이미 구독되어있다면
            else
            {
                already = true;
            }

            // 구독한 콜백이 처음으로 등장한 함수라면
            if (!callBackDict.ContainsKey(action.Method.Name))
            {
                // 함수를 함수의 이름으로 저장해두는 딕셔너리에 구독
                callBackDict.Add(action.Method.Name, action);
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 콜백을 특정 콜백 뒤 순서에 구독시키는 함수입니다.
        /// </summary>
        /// <param name="handlerName">함수를 등록할 이벤트 핸들러의 이름</param>
        /// <param name="action">등록할 함수의 이름</param>
        /// <param name="beforeName">등록할 함수가 이 이름을 가진 함수 이후에 등록됩니다.</param>
        public void AddListenerAfter(string handlerName, UnityAction action, string beforeName)
        {
            bool already = false;
            // 이벤트 핸들러에 등록
            AddListener(handlerName, action, out already);

            // 구독하려는 이벤트 핸들러에 구독하려는 콜백이 이미 구독되어있을 경우
            if (already)
            {
                // 이미 구독된 콜백을 노드로 치환
                LinkedListNode<UnityAction> alreadyNode = eventHandlerDict[handlerName].Find(action);

                // 치환했을 경우
                if (alreadyNode != null)
                {
                    UnityAction beforeAction = callBackDict[beforeName];
                    // 콜백이 콜백 딕셔너리에 등록이 되어있을 경우
                    if (beforeAction != null)
                    {
                        // 이전 순서 함수를 노드로 치환
                        LinkedListNode<UnityAction> beforeNode = eventHandlerDict[handlerName].Find(beforeAction);
                        if (beforeNode != null)
                        {
                            // 기존에 등록되어있던 노드를 제거하고
                            eventHandlerDict[handlerName].Remove(alreadyNode);
                            // 새롭게 이벤트 핸들러에 이전 순서 노드 이후에 구독
                            eventHandlerDict[handlerName].AddAfter(beforeNode, alreadyNode);
                        }
                        else
                        {
                            Debug.LogError("이전 순서 함수를 노드로 치환할 수 없습니다.");
                        }
                    }
                    // 콜백이 콜백 딕셔너리에 등록이 되어있지 않을 경우
                    else
                    {
                        Debug.LogError($"{beforeName}이라는 함수를 찾을 수 없습니다.");
                        // 이 후, 콜백 딕셔너리에 메소드가 등록될 수 있으므로 재귀 예약해둠
                        StartCoroutine(CorInvoke(() => { AddListenerAfter(handlerName, action, beforeName); }, 1.0f));
                    }
                }
                else
                {
                    Debug.LogError("구독할 함수를 노드에서 찾을 수 없습니다.");
                }
            }
            // 구독하려는 이벤트 핸들러에 구독하려는 콜백이 구독되어있지 않을 경우
            else
            {
                // 방금 구독한 콜백을 이벤트 핸들러에서 찾아내 노드로 치환
                LinkedListNode<UnityAction> node = eventHandlerDict[handlerName].Find(action);

                // 구독한 콜백을 찾아 노드로 치환했을 경우
                if (node != null)
                {
                    UnityAction beforeAction = null;
                    // 콜백 딕셔너리에 이전 순서 콜백이 존재할 경우,
                    if (callBackDict.ContainsKey(beforeName))
                    {
                        // 구독하려는 콜백 이전 순서에 있는 콜백을 콜백 딕셔너리를 통해 얻어옴
                        beforeAction = callBackDict[beforeName];
                    }
                    // 콜백 딕셔너리에 이전 순서에 해당하는 콜백이 없을 경우
                    else
                    {
                        Debug.LogError($"{beforeName}이라는 이름의 메소드를 찾을 수 없습니다.");

                        // 이 후, 콜백 딕셔너리에 메소드가 등록될 수 있으므로 재귀 예약해둠
                        StartCoroutine(CorInvoke(() => { AddListenerAfter(handlerName, action, beforeName); }, 1.0f));
                        return;
                    }

                    // 이전 순서 콜백을 성공적으로 얻어올 경우,
                    if (beforeAction != null)
                    {
                        // 이전 순서 콜백을 노드로 치환
                        LinkedListNode<UnityAction> beforeNode = eventHandlerDict[handlerName].Find(beforeAction);

                        // 치환에 성공할 경우,
                        if (beforeNode != null)
                        {
                            // 구독할 콜백을 before함수 이후에 구독
                            eventHandlerDict[handlerName].AddAfter(beforeNode, node);
                        }
                        // 치환에 실패할 경우,
                        else
                        {
                            Debug.LogError("Before Node를 찾을 수 없습니다.");
                        }
                    }
                }
                // 구독한 콜백을 찾아 노드로 치환실패했을 경우,
                else
                {
                    Debug.LogError("삽입하고자하는 노드를 찾을 수 없습니다.");
                }
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 콜백을 특정 콜백 이전 순서에 구독시키는 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러 이름</param>
        /// <param name="action">등록할 콜백</param>
        /// <param name="afterName">등록할 콜백이 이 함수 이전 순서에 구독됩니다.</param>
        public void AddListenerBefore(string handlerName, UnityAction action, string afterName)
        {
            bool already = false;
            // 이벤트 핸들러에 등록
            AddListener(handlerName, action, out already);

            // 구독하려는 이벤트 핸들러에 이미 구독하려는 콜백이 구독되어있을 경우
            if (already)
            {
                // 이미 구독된 콜백을 노드로 치환
                LinkedListNode<UnityAction> alreadyNode = eventHandlerDict[handlerName].Find(action);

                // 치환했을 경우
                if (alreadyNode != null)
                {
                    // 뒤 순서 함수를 딕셔너리에서 얻어옴
                    UnityAction afterAction = callBackDict[afterName];

                    // 딕셔너리에 뒤 순서 함수가 등록되어있을 경우
                    if (afterAction != null)
                    {
                        // 뒤 순서 함수를 노드로 치환
                        LinkedListNode<UnityAction> afterNode = eventHandlerDict[handlerName].Find(afterAction);
                        // 기존에 등록되어있던 노드를 제거하고
                        eventHandlerDict[handlerName].Remove(alreadyNode);
                        // 새롭게 이벤트 핸들러에 뒤 순서 노드 이전에 구독
                        eventHandlerDict[handlerName].AddBefore(afterNode, alreadyNode);
                        return;
                    }
                    // 딕셔너리에 뒤 순서 함수가 등록되어있지 않을 경우
                    else
                    {
                        Debug.LogError($"{afterName}이라는 이름의 메소드를 찾을 수 없습니다.");

                        // 이 후, 콜백 딕셔너리에 메소드가 등록될 수 있으므로 재귀 예약해둠
                        StartCoroutine(CorInvoke(() => { AddListenerBefore(handlerName, action, afterName); }, 1.0f));
                    }
                }
                else
                {
                    Debug.LogError("구독할 함수를 노드에서 찾을 수 없습니다.");
                    return;
                }
            }
            else
            {
                // 방금 구독한 콜백을 이벤트 핸들러에서 찾아내 노드로 치환
                LinkedListNode<UnityAction> nodeToRegist = eventHandlerDict[handlerName].Find(action);

                // 구독한 콜백을 찾아 노드로 치환했을 경우
                if (nodeToRegist != null)
                {
                    UnityAction afterAction = null;
                    // 콜백 딕셔너리에 뒤 순서 콜백이 존재할 경우,
                    if (callBackDict.ContainsKey(afterName))
                    {
                        // 구독하려는 콜백 뒤 순서에 있는 콜백을 콜백 딕셔너리를 통해 얻어옴
                        afterAction = callBackDict[afterName];
                    }
                    // 콜백 딕셔너리에 뒤 순서에 해당하는 콜백이 없을 경우
                    else
                    {
                        Debug.LogError($"{afterName}이라는 이름의 메소드를 찾을 수 없습니다.");

                        // 이 후, 콜백 딕셔너리에 메소드가 등록될 수 있으므로 재귀 예약해둠
                        StartCoroutine(CorInvoke(() => { AddListenerBefore(handlerName, action, afterName); }, 1.0f));
                    }

                    // 뒤 순서 콜백을 성공적으로 얻어올 경우,
                    if (afterAction != null)
                    {
                        // 뒤 순서 콜백을 노드로 치환
                        LinkedListNode<UnityAction> afterNode = eventHandlerDict[handlerName].Find(afterAction);

                        // 치환에 성공할 경우,
                        if (afterNode != null)
                        {
                            // 구독할 콜백을 after함수 이전에 구독
                            eventHandlerDict[handlerName].AddBefore(afterNode, nodeToRegist);
                        }
                        // 치환에 실패할 경우,
                        else
                        {
                            Debug.LogError("After Node를 찾을 수 없습니다.");
                        }
                    }
                }
                // 구독한 콜백을 찾아 노드로 치환실패했을 경우,
                else
                {
                    Debug.LogError("삽입하고자하는 노드를 찾을 수 없습니다.");
                }
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 함수를 등록 해제하는 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러 이름</param>
        /// <param name="action">등록 해제할 함수</param>
        public void RemoveListener(string handlerName, UnityAction action)
        {
            // 이벤트 핸들러 딕셔너리에 해당 이름을 갖는 이벤트 핸들러가 존재한다면
            if (eventHandlerDict[handlerName] != null)
            {
                // 해당 이벤트 핸들러에서 해당 action을 제거
                eventHandlerDict[handlerName].Remove(action);
            }
            else
            {
                Debug.LogError("삭제하려는 이벤트가 존재하지 않습니다.");
            }
            // 해당 콜백이 함수 딕셔너리에 존재할 경우
            if (callBackDict.ContainsKey(action.Method.Name))
            {
                // 해당 콜백을 함수 딕셔너리에서 제거
                callBackDict.Remove(action.Method.Name);
            }
        }

        #endregion

        #region 이벤트 호출부

        /// <summary>
        /// 이벤트 핸들러를 호출하는 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러 이름</param>
        /// 
        public void InvokeEventHanler(string handlerName)
        {
            // 이벤트 핸들러 딕셔너리에서 UnityAction이 담긴 링크드 리스트를 추출
            LinkedList<UnityAction> eventLL = eventHandlerDict[handlerName];

            // 추출한 링크드 리스트를 순회하며 콜백을 호출
            foreach (UnityAction action in eventLL)
            {
                action.Invoke();
            }
        }
        #endregion


        #region 클래스 함수
        /// <summary>
        /// EventManager를 싱글톤 선언하는 함수입니다.
        /// </summary>
       

        #endregion


        /// <summary>
        /// 예약 호출(Invoke)용 코루틴 입니다.
        /// </summary>
        /// <param name="action">예약 호출할 콜백</param>
        /// <param name="time">예약 시간</param>
        /// <returns></returns>
        public static IEnumerator CorInvoke(UnityAction action, float time)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}
