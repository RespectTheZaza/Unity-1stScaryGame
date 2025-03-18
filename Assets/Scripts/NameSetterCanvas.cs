using UnityEngine;
using TMPro;

public class NameSetterCanvas : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _input;

    private void Awake()
    {
        _input.onEndEdit.AddListener(_input_OnSubmit);
    }

    private void _input_OnSubmit(string text)
    {
        PlayerNameTracker.SetName(text);
    }
}
