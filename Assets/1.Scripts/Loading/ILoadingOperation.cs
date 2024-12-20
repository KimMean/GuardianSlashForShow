using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ILoadingOperation
{
    public float progress { get; }
    Task Execute();
}
