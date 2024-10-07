using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : Singleton<DataManager>
{
    private string filePath;
    public Dictionary<string, Monster> LoadedMonsterList { get; private set; }
    public Dictionary<string, CodeBlockData> LoadedCodeBlockList { get; private set; }
    public Dictionary<int, MoveBlock> LoadedMoveBlockList { get; private set; }
    public Dictionary<int, AttackBlock> LoadedAttackBlockList { get; private set; }
    public Dictionary<int, MonsterType> LoadedMonsterType { get; private set; }
    public Dictionary<int, StageMap> LoadedStageMap { get; private set; }
    public Dictionary<int, UIText> LoadedText { get; private set; }
    public Dictionary<string, TextType> LoadedTextType { get; private set; }
    public Dictionary<string, PlayerData> LoadedPlayerData { get; private set; }

    private readonly string _dataRootPath = Application.streamingAssetsPath;//"Application.StreamingAssetsPath";


    protected void Awake()
    {
        filePath = Application.persistentDataPath + "/playerData.json";
        ReadAllDataOnAwake();
    }

    #region 데이터테이블 로드
    private void ReadAllDataOnAwake()
    {
        LoadedMonsterList = LoadDataTable(nameof(Monster), ParseMonster, m => m.MonsterName);
        LoadedCodeBlockList = LoadDataTable(nameof(CodeBlockData), ParseCodeBlockData, cb => cb.BlockName);
        LoadedMonsterType = LoadDataTable(nameof(MonsterType), ParseMonsterType, mt => mt.TypeIndex);
        LoadedStageMap = LoadDataTable(nameof(StageMap), ParseStageMap, sm => sm.StageIndex);
        LoadedText = LoadDataTable(nameof(UIText), ParseUIText, ut => ut.TextIndex);
        LoadedTextType = LoadDataTable(nameof(TextType), ParseTextType, tt => tt.TypeName);
        LoadedPlayerData = LoadDataTable(nameof(PlayerData), ParsePlayerData, p => p.PlayerName);
    }
    void CopyFileToPersistentDataPath(string fileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string destinationPath = Path.Combine(Application.persistentDataPath, fileName);

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!File.Exists(destinationPath))
            {
                UnityWebRequest www = UnityWebRequest.Get(sourcePath);
                www.SendWebRequest();

                while (!www.isDone) { }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(destinationPath, www.downloadHandler.data);
                }
                else
                {
                    Debug.LogError("Failed to copy file from StreamingAssets: " + www.error);
                }
            }
        }
        else
        {
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }
            else
            {
                Debug.LogError("File not found in StreamingAssets: " + sourcePath);
            }
        }
    }

    private Dictionary<TKey, TValue> LoadDataTable<TKey, TValue>(string fileName, Func<XElement, TValue> parseElement, Func<TValue, TKey> getKey)
    {
        var dataTable = new Dictionary<TKey, TValue>();

        // 파일을 먼저 복사
        CopyFileToPersistentDataPath($"{fileName}.xml");

        // 데이터를 persistentDataPath에서 로드
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.xml");

        if (File.Exists(filePath))
        {
            XDocument doc = XDocument.Load(filePath);
            var dataElements = doc.Descendants("data");

            foreach (var data in dataElements)
            {
                TValue value = parseElement(data);
                TKey key = getKey(value);
                dataTable.Add(key, value);
            }
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }

        return dataTable;
    }
    #endregion

    #region 데이터 파싱
    private Monster ParseMonster(XElement data)
    {
        return new Monster
        {
            ID = int.Parse(data.Attribute(nameof(Monster.ID)).Value),
            MonsterName = data.Attribute(nameof(Monster.MonsterName)).Value,
            MonsterViewName = data.Attribute(nameof(Monster.MonsterViewName)).Value,
            Description = data.Attribute(nameof(Monster.Description)).Value,
            TypeIndex = int.Parse(data.Attribute(nameof(Monster.TypeIndex)).Value)
        };
    }

    private CodeBlockData ParseCodeBlockData(XElement data)
    {
        return new CodeBlockData
        {
            BlockIndex = int.Parse(data.Attribute(nameof(CodeBlockData.BlockIndex)).Value),
            BlockName = data.Attribute(nameof(CodeBlockData.BlockName)).Value,
            ViewName = data.Attribute(nameof(CodeBlockData.ViewName)).Value,
            Description = data.Attribute(nameof(CodeBlockData.Description)).Value
        };
    }

    private MoveBlock ParseMoveBlock(XElement data)
    {
        return new MoveBlock
        {
            BlockIndex = int.Parse(data.Attribute(nameof(MoveBlock.BlockIndex)).Value),
            MoveDirection = int.Parse(data.Attribute(nameof(MoveBlock.MoveDirection)).Value)
        };
    }

    private AttackBlock ParseAttackBlock(XElement data)
    {
        return new AttackBlock
        {
            BlockIndex = int.Parse(data.Attribute(nameof(AttackBlock.BlockIndex)).Value),
            AttackType = int.Parse(data.Attribute(nameof(AttackBlock.AttackType)).Value)
        };
    }

    private MonsterType ParseMonsterType(XElement data)
    {
        return new MonsterType
        {
            TypeIndex = int.Parse(data.Attribute(nameof(MonsterType.TypeIndex)).Value),
            TypeName = data.Attribute(nameof(MonsterType.TypeName)).Value,
            TypeViewName = data.Attribute(nameof(MonsterType.TypeViewName)).Value,
            Weakness = int.Parse(data.Attribute(nameof(MonsterType.Weakness)).Value)
        };
    }

    private StageMap ParseStageMap(XElement data)
    {
        var tempStageMap = new StageMap
        {
            StageIndex = int.Parse(data.Attribute(nameof(StageMap.StageIndex)).Value),
            StageSize = ParseVector2Int(data.Attribute("StageSize").Value),
            BlockContainerLength = int.Parse(data.Attribute(nameof(StageMap.BlockContainerLength)).Value),
            PlayerSpawnPos = ParseVector2Int(data.Attribute("PlayerSpawnPos").Value)
        };

        SetDataList(out tempStageMap.ArrayInfo, data, "ArrayInfo");
        SetDataList(out tempStageMap.BlockNameList, data, "BlockNameList");
        SetDataList(out tempStageMap.MonsterNameList, data, "MonsterNameList");
        SetDataList(out tempStageMap.MonsterSpawnPosList, data, "MonsterSpawnPosList", ParseVector2Int);
        SetDataList(out tempStageMap.BushMonsterNameList, data, "BushMonsterNameList");

        return tempStageMap;
    }

    private UIText ParseUIText(XElement data)
    {
        return new UIText
        {
            TextIndex = int.Parse(data.Attribute(nameof(UIText.TextIndex)).Value),
            TextTypeIndex = int.Parse(data.Attribute(nameof(UIText.TextTypeIndex)).Value),
            Description = data.Attribute(nameof(UIText.Description)).Value
        };
    }

    private TextType ParseTextType(XElement data)
    {
        return new TextType
        {
            TextTypeIndex = int.Parse(data.Attribute(nameof(TextType.TextTypeIndex)).Value),
            TypeName = data.Attribute(nameof(TextType.TypeName)).Value
        };
    }
    private PlayerData ParsePlayerData(XElement data)
    {
        var tempPlayerData = new PlayerData
        {
            PlayerName = data.Attribute(nameof(PlayerData.PlayerName)).Value
        };

        SetDataList(out tempPlayerData.StartMonsterNameList, data, "StartMonsterNameList");
        
        return tempPlayerData;
    }

    private Vector2Int ParseVector2Int(string value)

    {
        var values = value.Replace("(", "").Replace(")", "").Split('/');
        return new Vector2Int(int.Parse(values[0]), int.Parse(values[1]));
    }

    #endregion

    #region 데이터 세팅
    private void SetDataList<T>(out List<T> usingList, XElement data, string listName, Func<string, T> parseElement = null)
    {
        string ListStr = data.Attribute(listName)?.Value;
        if (!string.IsNullOrEmpty(ListStr))
        {
            ListStr = ListStr.Replace("{", "").Replace("}", "");

            var elements = ListStr.Split(',');

            var list = new List<T>();

            foreach (var element in elements)
            {
                T value = parseElement != null ? parseElement(element) : (T)Convert.ChangeType(element, typeof(T));
                list.Add(value);
            }
            usingList = list;
        }
        else
        {
            usingList = null;
        }
    }
    #endregion 

    #region 데이터 불러오기
    public Monster GetMonsterData(string dataName)
    {
        string name = RemoveTextAfterParenthesis(dataName);

        if (LoadedMonsterList.Count == 0 || !LoadedMonsterList.ContainsKey(name))
            return null;

        return LoadedMonsterList[name];
    }

    public CodeBlockData GetCodeBlockData(string dataClassName)
    {
        string name = RemoveTextAfterParenthesis(dataClassName);
        if (LoadedCodeBlockList.Count == 0 || !LoadedCodeBlockList.ContainsKey(name))
            return null;

        return LoadedCodeBlockList[name];
    }

    public MoveBlock GetMoveBlockData(int blockIndex)
    {
        if (LoadedMoveBlockList.Count == 0 || !LoadedMoveBlockList.ContainsKey(blockIndex))
            return null;

        return LoadedMoveBlockList[blockIndex];
    }

    public AttackBlock GetAttackBlockData(int blockIndex)
    {
        if (LoadedAttackBlockList.Count == 0 || !LoadedAttackBlockList.ContainsKey(blockIndex))
            return null;

        return LoadedAttackBlockList[blockIndex];
    }

    public StageMap GetStageMapData(int dataIndex)
    {
        if (LoadedStageMap.Count == 0 || !LoadedStageMap.ContainsKey(dataIndex))
            return null;

        return LoadedStageMap[dataIndex];
    }

    public MonsterType GetMonsterTypeData(int dataIndex)
    {
        if (LoadedMonsterType.Count == 0 || !LoadedMonsterType.ContainsKey(dataIndex))
            return null;

        return LoadedMonsterType[dataIndex];
    }

    public UIText GetTextData(int dataIndex)
    {
        if (LoadedText.Count == 0 || !LoadedText.ContainsKey(dataIndex))
            return null;

        return LoadedText[dataIndex];
    }

    public TextType GetTextTypeData(string dataClassName)
    {
        string name = RemoveTextAfterParenthesis(dataClassName);
        if (LoadedTextType.Count == 0 || !LoadedTextType.ContainsKey(name))
            return null;

        return LoadedTextType[name];
    }
    public PlayerData GetPlayerData(string dataClassName)
    {
        string name= RemoveTextAfterParenthesis(dataClassName);

        if (LoadedPlayerData.Count == 0 || !LoadedPlayerData.ContainsKey(name))
            return null;

        return LoadedPlayerData[name];
    }
    #endregion

    public string RemoveTextAfterParenthesis(string input)
    {
        int index = input.IndexOf('(');

        // 만약 '('가 문자열에 없다면, 원본 문자열을 그대로 반환
        if (index == -1)
        {
            return input;
        }

        // '(' 전까지의 문자열만 반환
        return input.Substring(0, index).Trim();
    }
}
