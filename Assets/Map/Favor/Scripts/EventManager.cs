using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace FavorKim
{
    public class EventManager : MonoBehaviour
    {
        #region 변수
        private static EventManager instance;
        public static EventManager Instance;

        // 이벤트 핸들러
        private Dictionary<string, LinkedList<UnityAction>> eventDict = new Dictionary<string, LinkedList<UnityAction>>();
        // 함수의 이름을 Key, 함수를 Value로 갖는 딕셔너리
        private Dictionary<string, UnityAction> actionsDict = new Dictionary<string, UnityAction>();

        #endregion

        #region 유니티 이벤트
        private void Awake()
        {
            InitManager();
        }


        #endregion







        #region 이벤트 등록/해제부
        /// <summary>
        /// 이벤트 등록 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러의 이름</param>
        /// <param name="action">이벤트 핸들러에 등록할 메서드</param>
        public void AddListener(string handlerName, UnityAction action, out bool already)
        {

            // 이벤트 핸들러가 딕셔너리에 등록되어있지 않으면
            if (!eventDict.ContainsKey(handlerName))
            {
                // 새로 생성 후 이벤트 딕셔너리에 등록
                LinkedList<UnityAction> list = new LinkedList<UnityAction>();
                eventDict.Add(handlerName, list);
            }

            // 이벤트 링크드 리스트에 콜백이 존재하지 않으면
            if (!eventDict[handlerName].Contains(action))
            {
                LinkedListNode<UnityAction> node = new LinkedListNode<UnityAction>(action);

                // 이벤트 링크드 리스트에 추가
                eventDict[handlerName].AddLast(action);
                already = false;
            }
            else
            {
                already = true;
            }

            // 처음 등록된 함수이름이라면
            if (!actionsDict.ContainsKey(action.Method.Name))
            {
                // 함수를 함수의 이름으로 저장해두는 딕셔너리에 등록
                actionsDict.Add(action.Method.Name, action);
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 콜백을 특정 콜백 이전 순서에 등록시키는 함수입니다.
        /// </summary>
        /// <param name="handlerName">함수를 등록할 이벤트 핸들러의 이름</param>
        /// <param name="action">등록할 함수의 이름</param>
        /// <param name="afterName">등록할 함수가 이 이름을 가진 함수 이전에 등록됩니다.</param>
        public void AddListenerAfter(string handlerName, UnityAction action, string afterName)
        {

            
            bool already = false;
            // 이벤트 핸들러에 등록
            AddListener(handlerName, action, out already);
            if (already)
            {
                Debug.LogWarning("이미 등록된 콜백입니다.");
                return;
            }

            LinkedListNode<UnityAction> node = eventDict[handlerName].Find(action);
            if (node != null)
            {
                UnityAction afterAction = null;
                // 이름을 통해 함수를 찾아옴
                if (actionsDict.ContainsKey(afterName))
                {
                    afterAction = actionsDict[afterName];
                }
                else
                {
                    Debug.LogError($"{afterName}이라는 이름의 메소드를 찾을 수 없습니다.");
                    // 이 후, 이름-함수 딕셔너리에 메소드가 등록될 수 있으므로 예약해둠
                    StartCoroutine(CorInvoke(() => { AddListenerAfter(handlerName, action, afterName); }, 1.0f));
                }
                if (afterAction != null)
                {
                    LinkedListNode<UnityAction> afterNode = eventDict[handlerName].Find(afterAction);
                    if (afterNode != null)
                    {
                        // 등록할 함수를 after함수 이후에 등록
                        eventDict[handlerName].AddAfter(afterNode, node);
                    }
                    else
                    {
                        Debug.LogError("After Node를 찾을 수 없습니다.");
                    }
                }
            }
            else
            {
                Debug.LogError("삽입하고자하는 노드를 찾을 수 없습니다.");
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 콜백을 특정 콜백 이전 순서에 등록시키는 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러 이름</param>
        /// <param name="action">등록할 콜백</param>
        /// <param name="beforeName">등록할 콜백이 이 함수 이전 순서에 등록됩니다.</param>
        public void AddListenerBefore(string handlerName, UnityAction action, string beforeName)
        {
            bool already = false;
            // 이벤트 핸들러에 등록
            AddListener(handlerName, action, out already);

            if (already)
            {
                Debug.LogWarning("이미 등록된 콜백입니다.");
                return;
            }

            LinkedListNode<UnityAction> node = eventDict[handlerName].Find(action);
            if (node != null)
            {
                // 이름을 통해 함수를 찾아옴
                UnityAction beforeAction = actionsDict[beforeName];
                LinkedListNode<UnityAction> afterNode = eventDict[handlerName].Find(beforeAction);
                if (afterNode != null)
                {
                    // 등록할 함수를 before 이전에 등록
                    eventDict[handlerName].AddBefore(afterNode, node);
                }
                else
                {
                    Debug.LogError("Before Node를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError("삽입하고자하는 노드를 찾을 수 없습니다.");
            }
        }

        /// <summary>
        /// 이벤트 핸들러에 함수를 등록 해제하는 함수입니다.
        /// </summary>
        /// <param name="handlerName">이벤트 핸들러 이름</param>
        /// <param name="action">등록 해제할 함수</param>
        public void RemoveListener(string handlerName, UnityAction action)
        {
            if (eventDict[handlerName] != null)
            {
                eventDict[handlerName].Remove(action);
            }
            else
            {
                Debug.LogError("삭제하려는 이벤트가 존재하지 않습니다.");
            }
            if (actionsDict.ContainsKey(nameof(action)))
            {
                actionsDict.Remove(nameof(action));
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
            LinkedList<UnityAction> eventLL = eventDict[handlerName];

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
        public void InitManager()
        {
            if (instance != null && instance.gameObject != this.gameObject)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;
            Instance = instance;
        }
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
