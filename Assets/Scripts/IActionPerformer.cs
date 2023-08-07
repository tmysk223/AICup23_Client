using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public interface IActionPerformer 
{
    void PerformUpdate(int[] owners, int[] count);

    void PerformAttack(LogUtility.Attack info);

    void PerformAdd(int node, int amount);

    void PerformFortify(LogUtility.Fortify info);
}
