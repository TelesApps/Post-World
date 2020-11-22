using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskProgressPanel : MonoBehaviour
{
    [SerializeField] RawImage ColonistImg;
    [SerializeField] TextMeshProUGUI ColonistNameTxt;
    [SerializeField] TextMeshProUGUI TaskDescriptionTxt;
    [SerializeField] Slider TaskProgess;
    [SerializeField] TextMeshProUGUI SliderProgressTxt;

    BuildingControlsUI parentControls;
    Colonist colonist;
    WorkSlot workSlot;

    public void OnInit(Colonist colonist, WorkSlot workSlot)
    {
        this.colonist = colonist;
        this.workSlot = workSlot;
        this.ColonistImg.texture = colonist.getRenderTexture();
        this.ColonistNameTxt.text = $"{colonist.getColonistData().FirstName} {colonist.getColonistData().LastName}";
    }

    void Start()
    {
        parentControls = GetComponentInParent<BuildingControlsUI>();
        this.TaskDescriptionTxt.text = colonist.getColonistData().colStatusDesc;
    }

    private void FixedUpdate()
    {
        float progressValue = workSlot.OutputRPCReceived / workSlot.OutputRPCRequired;
        // MathfLerp makes the bar move more smoothly.
        TaskProgess.value = Mathf.Lerp(TaskProgess.value, progressValue, 8 * Time.deltaTime);
        SliderProgressTxt.text = $"Task: {System.Math.Round(progressValue * 100, 2)}%";
    }

    public void onCancelTask()
    {
        parentControls.onCancelColonistTask(workSlot.SlotNumber, this.transform);
    }
}
