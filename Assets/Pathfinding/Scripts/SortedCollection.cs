using System.Collections.Generic;

public class SortedCollection<T>
{
    public delegate bool Compare(T first, T second);

    private readonly Compare compareMethod;
    private readonly List<T> list;

    public SortedCollection(Compare compareMethod)
    {
        this.compareMethod = compareMethod;
        list = new List<T>();
    }

    public T GetFirst()
    {
        T element = list[0];
        list.RemoveAt(0);
        return element;
    }

    public void Add(T element)
    {
        int inserIndex = 0;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (compareMethod(element, list[i]))
            {
                inserIndex = i + 1;
                break;
            }
        }
        list.Insert(inserIndex, element);
    }

    public int Count => list.Count;
    public bool Contains(T element) => list.Contains(element);
}
