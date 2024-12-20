using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadUserDataOperation : ILoadingOperation
{
    public float progress { get; private set; }
    private List<Func<Task<bool>>> LoadTasks;


    public LoadUserDataOperation()
    {
        LoadTasks = new List<Func<Task<bool>>>
        {
            () => NetworkManager.Instance.GetUserClearStage(),
            () => NetworkManager.Instance.GetUserCurrency(),
            () => NetworkManager.Instance.GetWeaponData(),
            () => NetworkManager.Instance.GetUserWeaponData(),
            () => NetworkManager.Instance.GetNecklaceData(),
            () => NetworkManager.Instance.GetUserNecklaceData(),
            () => NetworkManager.Instance.GetRingData(),
            () => NetworkManager.Instance.GetUserRingData(),
            () => NetworkManager.Instance.GetUserEquipmentData(),
            () => NetworkManager.Instance.GetProductData()
        };
    }
    /// <summary>
    /// �� �ε� �� �ʿ��� �����͸� ���ʷ� ��û�մϴ�.
    /// </summary>
    public async Task Execute()
    {
        progress = 0.0f;

        int totalTasks = LoadTasks.Count;
        int completedTasks = 0;

        foreach(Func<Task<bool>> taskFunc in LoadTasks)
        {
            Task<bool> task = taskFunc();
            bool result = await task; // Task�� �Ϸ�� ������ ��ٸ�
            if (result)
            {
                //Debug.Log($"Task ���� �� : {task.ToString()}");
                completedTasks++;
                progress = (float)completedTasks / totalTasks;
            }
            else
            {
                //Debug.LogError("Failed to load user data.");
                break; // �½�ũ ���� �� ���� �ߴ�
            }
        }
    }

}
