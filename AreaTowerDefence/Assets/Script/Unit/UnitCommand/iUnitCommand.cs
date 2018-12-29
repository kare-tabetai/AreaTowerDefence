using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iUnitCommand
{
    /// <summary>
    /// 毎フレームのアップデート
    /// </summary>
    /// <param name="unitInfo"></param>
    void UpdateUnitInstruction(UnitInformation unitInfo);
    /// <summary>
    /// Queueに追加されてから中止したい場合これを呼び出してからDeQueue
    /// </summary>
    void Finalize(UnitInformation unitInfo);
}
