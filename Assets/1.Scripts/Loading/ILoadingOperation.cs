using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ILoadingOperation
{
    public float Progress { get; }
    Task Execute();
}
