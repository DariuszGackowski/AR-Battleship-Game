using UnityEngine;

[System.Serializable]
[System.Obsolete]
public class ActionDataBase : MonoBehaviour /*: NetworkBehaviour*/
{
    private int _attackerID;
    public int AttackerID
    {
        get => _attackerID;
        set => _attackerID = value;
    }
    private int _actionID;
    public int ActionID
    {
        get => _actionID;
        set => _actionID = value;
    }
    private int _defenderID;
    public int DefenderID
    {
        get => _defenderID;
        set => _defenderID = value;
    }
    public ActionDataBase(int attackerID, int actionID, int defenderID)
    {
        this.AttackerID = attackerID;
        this.ActionID = actionID;
        this.DefenderID = defenderID;
    }
    public static void RemoveData(ActionDataBase actionDataBase)
    {
        actionDataBase.AttackerID = actionDataBase.DefenderID = actionDataBase.ActionID = 0;
    }
}
