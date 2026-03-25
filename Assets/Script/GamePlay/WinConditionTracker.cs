using System;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionTracker : MonoBehaviour
{
    public event Action OnAllResolved;

    private readonly HashSet<PixelObjectRoot> activeFragments = new();

    private int scheduledObjects;
    private bool spawnFinished;

    public int ActiveFragmentCount => activeFragments.Count;
    public int ScheduledObjects => scheduledObjects;
    public bool SpawnFinished => spawnFinished;

    private void Start()
    {
        scheduledObjects = 0;
        spawnFinished = false;
    }

    private void OnDisable()
    {
        foreach (var fragment in activeFragments)
        {
            fragment.OnFragmentResolved -= HandleFragmentResolved;
            fragment.OnFragmentSplitCreated -= HandleFragmentSplitCreated;
        }
    }

    public void RegisterScheduledObject()
    {
        scheduledObjects++;
    }

    public void RegisterActiveFragment(PixelObjectRoot root)
    {
        if (root == null) return;
        if (activeFragments.Contains(root)) return;

        activeFragments.Add(root);
        root.OnFragmentResolved += HandleFragmentResolved;
        root.OnFragmentSplitCreated += HandleFragmentSplitCreated;
    }

    public void MarkSpawnFinished()
    {
        spawnFinished = true;
        CheckComplete();
    }

    private void HandleFragmentResolved(PixelObjectRoot root)
    {
        if (root == null) return;

        root.OnFragmentResolved -= HandleFragmentResolved;
        root.OnFragmentSplitCreated -= HandleFragmentSplitCreated;

        activeFragments.Remove(root);
        CheckComplete();
    }

    private void HandleFragmentSplitCreated(PixelObjectRoot newFragment)
    {
        RegisterActiveFragment(newFragment);
    }

    private void CheckComplete()
    {
        if (spawnFinished && activeFragments.Count == 0 && scheduledObjects > 0)
        {
            spawnFinished = false;
            scheduledObjects = 0;
            OnAllResolved?.Invoke();
        }
    }
}