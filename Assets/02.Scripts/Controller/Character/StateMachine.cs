/*using System;
using System.Collections.Generic;
using System.Linq;

public class StateMachine<T>
    where T : Enum
{

    public T currentState;
    public T previousState;
    protected Dictionary<T, IState<T>> states;
    private bool _isDirty;

    public void Init(IDictionary<T, IState<T>> copy)
    {
        states = new Dictionary<T, IState<T>>(copy);
        currentStateID = states.First().Key;
        states[currentStateID].OnStateEnter();
    }

    public void UpdateState()
    {
        // ���� ���¸� ������Ʈ�ϰ� �� ���°� �Ѱ��� ���� ���·�
        T nextID = states[currentStateID].OnStateUpdate();
        // �ٲ۴�
        ChangeState(nextID);
    }

    public void FixedUpdateState()
    {
        states[currentStateID].OnStateFixedUpdate();
    }

    public void LateUpdateState()
    {
        _isDirty = false;
    }

    public bool ChangeState(T newStateID)
    {
        if (_isDirty)
            return false;

        // �ٲٷ��� ���°� ���� ���¿� �����ϸ� �ٲ�������
        if (Comparer<T>.Default.Compare(newStateID, currentStateID) == 0)
            return false;

        // �ٲٷ��� ���°� ���డ������ �ʴٸ� �ٲ��� ����
        if (states[newStateID].canExecute == false)
            return false;

        _isDirty = true;
        states[currentStateID].OnStateExit(); // ���� ���¿��� Ż��
        previousStateID = currentStateID;
        currentStateID = newStateID; // ���� ����
        states[currentStateID].OnStateEnter(); // ���ο� ���·� ����
        return true;
    }

    public void ChangeStateForcely(T newStateID)
    {
        if (Comparer<T>.Default.Compare(currentStateID, default) != 0)
            states[currentStateID].OnStateExit();

        previousStateID = currentStateID;
        currentStateID = newStateID;
        states[currentStateID].OnStateEnter();
    }
}
}
*/