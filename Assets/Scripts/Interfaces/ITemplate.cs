using System.Collections.Generic;



public interface ITemplate<T>
{
    T TemplateInstance { get; }
    List<T> ObserversList{ get;}
    T AddObserver();
    void RemoveObserver(int index);
    void RemoveObserver(T observer);
    void ClearObservers();
}
