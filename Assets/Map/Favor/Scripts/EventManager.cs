using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace FavorKim
{
    public class EventManager : MonoBehaviour
    {
        #region ����
        private static EventManager instance;
        public static EventManager Instance;

        // �̺�Ʈ �ڵ鷯
        private Dictionary<string, LinkedList<UnityAction>> eventDict = new Dictionary<string, LinkedList<UnityAction>>();
        // �Լ��� �̸��� Key, �Լ��� Value�� ���� ��ųʸ�
        private Dictionary<string, UnityAction> actionsDict = new Dictionary<string, UnityAction>();

        #endregion

        #region ����Ƽ �̺�Ʈ
        private void Awake()
        {
            InitManager();
        }


        #endregion







        #region �̺�Ʈ ���/������
        /// <summary>
        /// �̺�Ʈ ��� �Լ��Դϴ�.
        /// </summary>
        /// <param name="handlerName">�̺�Ʈ �ڵ鷯�� �̸�</param>
        /// <param name="action">�̺�Ʈ �ڵ鷯�� ����� �޼���</param>
        public void AddListener(string handlerName, UnityAction action, out bool already)
        {

            // �̺�Ʈ �ڵ鷯�� ��ųʸ��� ��ϵǾ����� ������
            if (!eventDict.ContainsKey(handlerName))
            {
                // ���� ���� �� �̺�Ʈ ��ųʸ��� ���
                LinkedList<UnityAction> list = new LinkedList<UnityAction>();
                eventDict.Add(handlerName, list);
            }

            // �̺�Ʈ ��ũ�� ����Ʈ�� �ݹ��� �������� ������
            if (!eventDict[handlerName].Contains(action))
            {
                LinkedListNode<UnityAction> node = new LinkedListNode<UnityAction>(action);

                // �̺�Ʈ ��ũ�� ����Ʈ�� �߰�
                eventDict[handlerName].AddLast(action);
                already = false;
            }
            else
            {
                already = true;
            }

            // ó�� ��ϵ� �Լ��̸��̶��
            if (!actionsDict.ContainsKey(action.Method.Name))
            {
                // �Լ��� �Լ��� �̸����� �����صδ� ��ųʸ��� ���
                actionsDict.Add(action.Method.Name, action);
            }
        }

        /// <summary>
        /// �̺�Ʈ �ڵ鷯�� �ݹ��� Ư�� �ݹ� ���� ������ ��Ͻ�Ű�� �Լ��Դϴ�.
        /// </summary>
        /// <param name="handlerName">�Լ��� ����� �̺�Ʈ �ڵ鷯�� �̸�</param>
        /// <param name="action">����� �Լ��� �̸�</param>
        /// <param name="afterName">����� �Լ��� �� �̸��� ���� �Լ� ������ ��ϵ˴ϴ�.</param>
        public void AddListenerAfter(string handlerName, UnityAction action, string afterName)
        {

            
            bool already = false;
            // �̺�Ʈ �ڵ鷯�� ���
            AddListener(handlerName, action, out already);
            if (already)
            {
                Debug.LogWarning("�̹� ��ϵ� �ݹ��Դϴ�.");
                return;
            }

            LinkedListNode<UnityAction> node = eventDict[handlerName].Find(action);
            if (node != null)
            {
                UnityAction afterAction = null;
                // �̸��� ���� �Լ��� ã�ƿ�
                if (actionsDict.ContainsKey(afterName))
                {
                    afterAction = actionsDict[afterName];
                }
                else
                {
                    Debug.LogError($"{afterName}�̶�� �̸��� �޼ҵ带 ã�� �� �����ϴ�.");
                    // �� ��, �̸�-�Լ� ��ųʸ��� �޼ҵ尡 ��ϵ� �� �����Ƿ� �����ص�
                    StartCoroutine(CorInvoke(() => { AddListenerAfter(handlerName, action, afterName); }, 1.0f));
                }
                if (afterAction != null)
                {
                    LinkedListNode<UnityAction> afterNode = eventDict[handlerName].Find(afterAction);
                    if (afterNode != null)
                    {
                        // ����� �Լ��� after�Լ� ���Ŀ� ���
                        eventDict[handlerName].AddAfter(afterNode, node);
                    }
                    else
                    {
                        Debug.LogError("After Node�� ã�� �� �����ϴ�.");
                    }
                }
            }
            else
            {
                Debug.LogError("�����ϰ����ϴ� ��带 ã�� �� �����ϴ�.");
            }
        }

        /// <summary>
        /// �̺�Ʈ �ڵ鷯�� �ݹ��� Ư�� �ݹ� ���� ������ ��Ͻ�Ű�� �Լ��Դϴ�.
        /// </summary>
        /// <param name="handlerName">�̺�Ʈ �ڵ鷯 �̸�</param>
        /// <param name="action">����� �ݹ�</param>
        /// <param name="beforeName">����� �ݹ��� �� �Լ� ���� ������ ��ϵ˴ϴ�.</param>
        public void AddListenerBefore(string handlerName, UnityAction action, string beforeName)
        {
            bool already = false;
            // �̺�Ʈ �ڵ鷯�� ���
            AddListener(handlerName, action, out already);

            if (already)
            {
                Debug.LogWarning("�̹� ��ϵ� �ݹ��Դϴ�.");
                return;
            }

            LinkedListNode<UnityAction> node = eventDict[handlerName].Find(action);
            if (node != null)
            {
                // �̸��� ���� �Լ��� ã�ƿ�
                UnityAction beforeAction = actionsDict[beforeName];
                LinkedListNode<UnityAction> afterNode = eventDict[handlerName].Find(beforeAction);
                if (afterNode != null)
                {
                    // ����� �Լ��� before ������ ���
                    eventDict[handlerName].AddBefore(afterNode, node);
                }
                else
                {
                    Debug.LogError("Before Node�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogError("�����ϰ����ϴ� ��带 ã�� �� �����ϴ�.");
            }
        }

        /// <summary>
        /// �̺�Ʈ �ڵ鷯�� �Լ��� ��� �����ϴ� �Լ��Դϴ�.
        /// </summary>
        /// <param name="handlerName">�̺�Ʈ �ڵ鷯 �̸�</param>
        /// <param name="action">��� ������ �Լ�</param>
        public void RemoveListener(string handlerName, UnityAction action)
        {
            if (eventDict[handlerName] != null)
            {
                eventDict[handlerName].Remove(action);
            }
            else
            {
                Debug.LogError("�����Ϸ��� �̺�Ʈ�� �������� �ʽ��ϴ�.");
            }
            if (actionsDict.ContainsKey(nameof(action)))
            {
                actionsDict.Remove(nameof(action));
            }
        }

        #endregion

        #region �̺�Ʈ ȣ���

        /// <summary>
        /// �̺�Ʈ �ڵ鷯�� ȣ���ϴ� �Լ��Դϴ�.
        /// </summary>
        /// <param name="handlerName">�̺�Ʈ �ڵ鷯 �̸�</param>
        /// 
        public void InvokeEventHanler(string handlerName)
        {
            // �̺�Ʈ �ڵ鷯 ��ųʸ����� UnityAction�� ��� ��ũ�� ����Ʈ�� ����
            LinkedList<UnityAction> eventLL = eventDict[handlerName];

            // ������ ��ũ�� ����Ʈ�� ��ȸ�ϸ� �ݹ��� ȣ��
            foreach (UnityAction action in eventLL)
            {
                action.Invoke();
            }
        }
        #endregion


        #region Ŭ���� �Լ�
        /// <summary>
        /// EventManager�� �̱��� �����ϴ� �Լ��Դϴ�.
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
        /// ���� ȣ��(Invoke)�� �ڷ�ƾ �Դϴ�.
        /// </summary>
        /// <param name="action">���� ȣ���� �ݹ�</param>
        /// <param name="time">���� �ð�</param>
        /// <returns></returns>
        public static IEnumerator CorInvoke(UnityAction action, float time)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}
