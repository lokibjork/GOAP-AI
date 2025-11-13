using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubGoal
{
    public Dictionary<string, int> sGoals;
    // Se o objetivo deve ser removido ao ser alcançado
    public bool remove;
    public SubGoal(string sName, int iCost, bool remove)
    {
        sGoals = new Dictionary<string, int>();
        sGoals.Add(sName, iCost);
        this.remove = remove;
    }
}
public class GoapAgent : MonoBehaviour
{
    public List<GoapAction> actions = new List<GoapAction>();               // Lista de ações
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>(); // Lista de objetivos
    public GoapAction currentAction;                                        // Ação atual
    public Queue<GoapAction> actionQueue;                                   // Fila de ações
    SubGoal currentGoal;                                                    // Objetivo atual
    GoapPlanner planner;                                                    // Planejador
    bool invoked = false;                                                   // Se a ação foi invocada    
    public WorldStates personalStates = new WorldStates();                  // Estados pessoais
    public GoapInventory inventory = new GoapInventory();                   // Inventário do agente

    protected virtual void Start()
    {
        GoapAction[] goapActions = GetComponents<GoapAction>();
        foreach (GoapAction act in goapActions)
        {
            actions.Add(act);
        }
    }

    void CompleteAction()
    {
        currentAction.performingAction = false;
        currentAction.PostPerform();
        invoked = false;
    }

    void LateUpdate()
    {
        if (currentAction != null && currentAction.performingAction)
        {
            // O agente possui um path e chegou no destino?
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, transform.position);
            Debug.Log(distanceToTarget);
            if (currentAction.agent.hasPath && distanceToTarget <= 2.0f)
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            // Se o agente não estiver no destino setado inicialmente, setamos o destino
            else if (currentAction.agent.destination != currentAction.target.transform.position)
            {
                currentAction.agent.SetDestination(currentAction.target.transform.position);
            }
            return;
        }
        if (planner == null || actionQueue == null)
        {
            planner = new GoapPlanner();
            // Usamos o linq para sortir a lista por grau de prioridade
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionQueue = planner.ActionPlan(actions, sg.Key.sGoals, personalStates);
                if (actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }
        // Se a fila de ações não for nula e não tiver ações
        if (actionQueue != null && actionQueue.Count == 0)
        {
            // Se a meta atual for removida
            if (currentGoal.remove)
            {
                // Removemos a meta
                goals.Remove(currentGoal);
            }
            // Zeramos as variáveis
            planner = null;
        }
        if (actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }
                if (currentAction.target != null)
                {
                    currentAction.performingAction = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            }
            else { actionQueue = null; }
        }
    }
}