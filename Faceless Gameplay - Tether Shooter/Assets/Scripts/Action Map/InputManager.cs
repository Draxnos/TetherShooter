using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static PlayerControls pcs;

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (pcs == null)
        {
            pcs = new PlayerControls();
        }
    }

    public static void StartRebind(string name_, int index, GameObject rebindActive, bool excludeMouse)
    {
        InputAction action = pcs.asset.FindAction(name_);

        if (action == null || action.bindings.Count <= index)
        {
            return;
        }

        Rebind(action, index, rebindActive, excludeMouse);
    }

    private static void Rebind(InputAction toRebind, int index, GameObject rebindActive, bool excludeMouse)
    {
        if (toRebind == null || index < 0)
        {
            return;
        }

        rebindActive.SetActive(true);

        toRebind.Disable();

        var rebind = toRebind.PerformInteractiveRebinding(index);

        rebind.OnComplete(operation =>
        {
            toRebind.Enable();
            operation.Dispose();

            SaveBindingOverride(toRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            toRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        rebindStarted?.Invoke(toRebind, index);
        rebind.Start();
    }

    public static string GetBindingName(string name_, int index)
    {
        if (pcs == null)
        {
            pcs = new PlayerControls();
        }

        if (name_ == null)
        {
            return null;
        }

        InputAction action = pcs.asset.FindAction(name_);
        return action.GetBindingDisplayString(index);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int x = 0; x < action.bindings.Count; x++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + x, action.bindings[x].overridePath);
        }
    }

    public static void LoadBindingOverride(string name_)
    {
        if (pcs == null)
        {
            pcs = new PlayerControls();
        }

        InputAction action = pcs.asset.FindAction(name_);

        for (int x = 0; x < action.bindings.Count; x++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + x)))
            {
                action.ApplyBindingOverride(x, PlayerPrefs.GetString(action.actionMap + action.name + x));
            }
        }
    }

    public static void ResetBinding(string name_, int index)
    {
        InputAction action = pcs.asset.FindAction(name_);

        if (action == null || action.bindings.Count <= index)
        {
            return;
        }

        action.RemoveBindingOverride(index);
        SaveBindingOverride(action);
    }
}
