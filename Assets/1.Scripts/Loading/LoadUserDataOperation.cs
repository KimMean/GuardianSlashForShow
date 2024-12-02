using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadUserDataOperation : ILoadingOperation
{
    public float Progress { get; private set; }
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

    public async Task Execute()
    {
        Progress = 0.0f;

        int totalTasks = LoadTasks.Count;
        int completedTasks = 0;

        foreach(Func<Task<bool>> taskFunc in LoadTasks)
        {
            Task<bool> task = taskFunc();
            // 병렬처리를 위해서는 불필요
            bool result = await task; // Task가 완료될 때까지 기다림
            if (result)
            {
                //Debug.Log($"Task 실행 중 : {task.ToString()}");
                completedTasks++;
                Progress = (float)completedTasks / totalTasks;
            }
            else
            {
                //Debug.LogError("Failed to load user data.");
                break; // 태스크 실패 시 루프 중단
            }
        }
    }

}
