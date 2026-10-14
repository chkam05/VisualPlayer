using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace VisualPlayer.Utils;

public class ShuffleCycle
{
    private readonly Dictionary<int, int> _nextIndices = new();
    private readonly Random _random;


    #region CONSTRUCTORS

    public ShuffleCycle(int count, Random? random = null)
    {
        _random = random ?? Random.Shared;
        Build(count, null);
    }

    #endregion CONSTRUCTORS


    public int GetNextIndex(int currentIndex)
    {
        return _nextIndices.GetValueOrDefault(currentIndex, -1);
    }

    public int GetPreviousIndex(int currentIndex)
    {
        foreach (var pair in _nextIndices)
        {
            if (pair.Value == currentIndex)
                return pair.Key;
        }

        return -1;
    }

    public void Rebuild(int count, int? firstIndex = null)
    {
        Build(count, firstIndex);
    }

    public int Reindex(NotifyCollectionChangedEventArgs change, int currentIndex, int count)
    {
        switch (change.Action)
        {
            case NotifyCollectionChangedAction.Add when change.NewItems is not null:
                for (var i = 0; i < change.NewItems.Count; i++)
                    currentIndex = Insert(change.NewStartingIndex + i, currentIndex);
                break;
            case NotifyCollectionChangedAction.Remove when change.OldItems is not null:
                for (var i = change.OldItems.Count - 1; i >= 0; i--)
                    currentIndex = Remove(change.OldStartingIndex + i, currentIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                currentIndex = Move(change.OldStartingIndex, change.NewStartingIndex, currentIndex);
                break;
            case NotifyCollectionChangedAction.Replace when change.NewItems is not null:
                for (var i = change.NewItems.Count - 1; i >= 0; i--)
                    currentIndex = Remove(change.NewStartingIndex + i, currentIndex);
                for (var i = 0; i < change.NewItems.Count; i++)
                    currentIndex = Insert(change.NewStartingIndex + i, currentIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                Build(count, null);
                return -1;
        }

        return currentIndex;
    }

    #region UTILITIES

    private void Build(int count, int? firstIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (firstIndex is int index && (index < 0 || index >= count))
            throw new ArgumentOutOfRangeException(nameof(firstIndex));

        _nextIndices.Clear();

        if (count == 0)
            return;

        var indices = new int[count];

        for (var i = 0; i < count; i++)
            indices[i] = i;

        for (var i = count - 1; i > 0; i--)
        {
            var other = _random.Next(i + 1);
            (indices[i], indices[other]) = (indices[other], indices[i]);
        }

        if (firstIndex is int selectedIndex)
        {
            var position = Array.IndexOf(indices, selectedIndex);
            (indices[0], indices[position]) = (indices[position], indices[0]);
        }

        _nextIndices[-1] = indices[0];

        for (var i = 0; i < count - 1; i++)
            _nextIndices[indices[i]] = indices[i + 1];

        _nextIndices[indices[^1]] = -1;
    }

    private int Insert(int index, int currentIndex)
    {
        Remap(value => value >= index ? value + 1 : value);

        if (currentIndex >= index)
            currentIndex++;

        var parents = new List<int>();
        var parent = currentIndex;

        do
        {
            parents.Add(parent);
            parent = GetNextIndex(parent);
        }
        while (parent >= 0);

        parent = parents[_random.Next(parents.Count)];
        _nextIndices[index] = GetNextIndex(parent);
        _nextIndices[parent] = index;

        return currentIndex;
    }

    private int Move(int oldIndex, int newIndex, int currentIndex)
    {
        int Map(int value)
        {
            if (value == oldIndex)
                return newIndex;
            if (oldIndex < newIndex && value > oldIndex && value <= newIndex)
                return value - 1;
            if (oldIndex > newIndex && value >= newIndex && value < oldIndex)
                return value + 1;
            return value;
        }

        Remap(Map);
        return currentIndex < 0 ? -1 : Map(currentIndex);
    }

    private void Remap(Func<int, int> map)
    {
        var updated = new Dictionary<int, int>(_nextIndices.Count);

        foreach (var pair in _nextIndices)
        {
            var key = pair.Key < 0 ? -1 : map(pair.Key);
            var value = pair.Value < 0 ? -1 : map(pair.Value);
            updated[key] = value;
        }

        _nextIndices.Clear();

        foreach (var pair in updated)
            _nextIndices.Add(pair.Key, pair.Value);
    }

    private int Remove(int index, int currentIndex)
    {
        var parent = GetPreviousIndex(index);
        _nextIndices[parent] = GetNextIndex(index);
        _nextIndices.Remove(index);

        if (currentIndex == index)
            currentIndex = parent;

        Remap(value => value > index ? value - 1 : value);

        if (currentIndex > index)
            currentIndex--;

        return currentIndex;
    }

    #endregion UTILITIES
}
