using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteData;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();

        InitSceneRouteDict();
    }

    private void InitSceneRouteDict()
    {
        if (sceneRouteData.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteData.sceneRouteList)
            {
                var key = route.fromSceneName + route.toSceneName;
                if (sceneRouteDict.ContainsKey(key))
                    continue;
                else
                    sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// ��ȡ�����������·��
    /// </summary>
    /// <param name="fromSceneName">��ʼ����</param>
    /// <param name="toSceneName">Ŀ�곡��</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        return sceneRouteDict[fromSceneName + toSceneName];
    }
}
