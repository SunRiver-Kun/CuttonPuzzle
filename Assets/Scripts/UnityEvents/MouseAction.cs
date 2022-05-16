using UnityEngine;
using UnityEngine.Events;

public class MouseAction : MonoBehaviour
{
    public enum ActionType
    {
        Down, Up, Enter, Exit, Over, Drag, UpAsButton
    }
    public UnityEvent downAction = new UnityEvent(); 
    public UnityEvent upAction = new UnityEvent(); 
    public UnityEvent enterAction = new UnityEvent(); 
    public UnityEvent exitAction = new UnityEvent(); 
    public UnityEvent overAction = new UnityEvent(); 
    public UnityEvent dragAction = new UnityEvent(); 
    public UnityEvent upAsButtnAction = new UnityEvent();
    public static event System.Action<GameObject, ActionType> totalAction; //manager，任何物体的任何事件都触发

    private void OnMouseDown()
    {
        downAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Down); }
    }
    private void OnMouseDrag()
    {
        dragAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Drag); }
    }
    private void OnMouseEnter()
    {
        enterAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Enter); }
    }
    private void OnMouseExit()
    {
        exitAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Exit); }
    }
    private void OnMouseOver()
    {
        overAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Over); }
    }
    private void OnMouseUp()
    {
        upAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.Up); }
    }
    //仅当释放鼠标时，鼠标还在物体上才调用OnMouseUpAsButton
    private void OnMouseUpAsButton()
    {
        upAsButtnAction.Invoke();
        if (totalAction != null) { totalAction(gameObject, ActionType.UpAsButton); }
    }
}
