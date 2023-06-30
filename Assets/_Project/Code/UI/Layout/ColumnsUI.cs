using UnityEngine;
using UnityEngine.UI;

public class ColumnsUI : MonoBehaviour
{
    #region Editor
#if UNITY_EDITOR
    [ContextMenu("Set Refs")]
    private void setRefs()
    {
        _layoutGroup = GetComponentInChildren<VerticalLayoutGroup>();
    }
#endif
    #endregion

    [SerializeField]
    private VerticalLayoutGroup _layoutGroup;

}
