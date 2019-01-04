using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class iUnitCommand
{
    protected bool initialized;
    public bool Initialized { get { return initialized; } }

    /// <summary>
    /// 毎フレームのアップデート
    /// </summary>
    /// <param name="unitInfo"></param>
    public virtual void UpdateUnitInstruction(UnitInformation unitInfo) { }
    /// <summary>
    /// Queueに追加されてから中止したい場合これを呼び出してからDeQueue
    /// </summary>
    public virtual void Finalize(UnitInformation unitInfo) { }
}
