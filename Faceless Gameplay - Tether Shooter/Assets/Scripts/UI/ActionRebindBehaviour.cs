using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ActionRebindBehaviour : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inpRef;

    [SerializeField]
    private bool excludeMouse = false;
    [SerializeField]
    private int selectedBind;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    [SerializeField]
    private string actionName;

    [SerializeField]
    private Text bindText;
    [SerializeField]
    private GameObject bindActive;
    [SerializeField]
    private Button bindButton;
    [SerializeField]
    private Button resetButton;
    

    private void OnEnable()
    {
        bindButton.onClick.AddListener(() => Rebind());

        if (inpRef != null)
        {
            InputManager.LoadBindingOverride(actionName);

            GetBindInfo();
            UpdateUI();
        }

        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        InputManager.rebindComplete -= UpdateUI;
        InputManager.rebindCanceled -= UpdateUI;
    }

    private void OnValidate()
    {
        if (!inpRef)
        {
            return;
        }

        GetBindInfo();
        UpdateUI();
    }

    private void GetBindInfo()
    {
        if (inpRef.action != null)
        {
            actionName = inpRef.action.name;
        }

        if (inpRef.action.bindings.Count > selectedBind)
        {
            inputBinding = inpRef.action.bindings[selectedBind];
            bindingIndex = selectedBind;
        }
    }

    private void UpdateUI()
    {
        if (bindActive && bindActive.activeInHierarchy)
        {
            bindActive.SetActive(false);
        }

        if (bindText != null)
        {
            if (Application.isPlaying)
            {
                bindText.text = InputManager.GetBindingName(actionName, selectedBind);
            }
            else
            {
                bindText.text = inpRef.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }

    public void Rebind()
    {
        InputManager.StartRebind(actionName, selectedBind, bindActive, excludeMouse);
    }

    private void ResetBind()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}
