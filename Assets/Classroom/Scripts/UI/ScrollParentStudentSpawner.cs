using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class ScrollParentStudentSpawner : MonoBehaviour
{
    #region Public Fields

    public GameObject studentPrefabToSpawn;
    public GridObjectCollection gridObject;
    public ScrollingObjectCollection scrollObject;
    public GameObject metaDataPanel;

    public TextMeshProUGUI metaDataNameText;
    public TextMeshProUGUI metaDataRegionText;
    public TextMeshProUGUI metaDataEmailText;

    public static ScrollParentStudentSpawner Instance;

    public bool pressableButtonsClicked = false;
    public GameObject pressableButtons;
    public ItemCardStudent selectedStudent;

    public List<GameObject> studentList = new List<GameObject>();

    public Interactable muteButton;

    #endregion

    #region Private Fields

    private Transform pressableButtonsLocation;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        Instance = this;

        pressableButtonsClicked = false;
        pressableButtons.SetActive(pressableButtonsClicked);
        metaDataPanel.SetActive(false);
    }

    #endregion

    #region Public Methods

    public void PlacePressableButtonsAtLocation(Transform _Location, ItemCardStudent _student)
    {
        // Nothing selected or Same Student Selected
        if(selectedStudent == null || selectedStudent == _student)
        {
            pressableButtonsClicked = !pressableButtonsClicked;
        }
        else
        {
            // New Student Selected
            pressableButtonsClicked = true;
            selectedStudent.DeselctToggleButton();            
        }

        pressableButtons.SetActive(pressableButtonsClicked);
        pressableButtonsLocation = _Location;
        selectedStudent = _student;

        if (selectedStudent.studentUser.studentMuted)
        {
            muteButton.IsToggled = true;
            ToggleMuteIcon(true);
        }
        else
        {
            muteButton.IsToggled = false;
            ToggleMuteIcon(false);
        }

        pressableButtons.transform.position = pressableButtonsLocation.position;

        scrollObject.CanScroll = !pressableButtonsClicked;

        PopulateMetaData();
    }

    public void PopulateMetaData()
    {
        metaDataNameText.text = selectedStudent.userName;
        metaDataRegionText.text = selectedStudent.userRegion;
        metaDataEmailText.text = selectedStudent.userEmail;
    }

    public void ToggleMetaData()
    {
        metaDataPanel.SetActive(!metaDataPanel.activeSelf);

        if(metaDataPanel.activeSelf)
        {
            PopulateMetaData();
        }
    }

    public void ToggleMuteIcon(bool mute)
    {
        if (selectedStudent != null)
        {
            selectedStudent.ToggleMuteIcon(mute);
        }
    }

    public void UpdateStudentScrollObject()
    {
        CleanStudentList();

        for (int i = 0; i < ClassroomManager.Instance.connectedStudentsList.Count; i++)
        {
            GameObject spawnedStudentUI;
            ItemCardStudent studentItemCard;
            spawnedStudentUI = Instantiate(studentPrefabToSpawn, studentPrefabToSpawn.transform.parent);
            spawnedStudentUI.SetActive(true);
            studentList.Add(spawnedStudentUI);

            studentItemCard = spawnedStudentUI.GetComponent<ItemCardStudent>();
            if (studentItemCard == null)
            {
                Debug.LogError("COULDN'T FIND SPAWNED STUDENT CARD COMPONENT");
                break;
            }

            if (ClassroomManager.Instance.connectedStudentsList[i] != null)
            {
                studentItemCard.studentUser = ClassroomManager.Instance.connectedStudentsList[i].GetComponent<ClassroomUser>();
                studentItemCard.UpdateNameText(ClassroomManager.Instance.connectedStudentsList[i].GetComponent<ClassroomUser>().userName);
                studentItemCard.UpdateStudentColor(ClassroomManager.Instance.connectedStudentsList[i].GetComponent<ClassroomUser>().userColor);
                studentItemCard.UpdateRegionText(ClassroomManager.Instance.connectedStudentsList[i].GetComponent<ClassroomUser>().userRegion);
                studentItemCard.UpdateEmailText(ClassroomManager.Instance.connectedStudentsList[i].GetComponent<ClassroomUser>().userEmail);
                studentItemCard.ToggleMuteIcon(studentItemCard.studentUser.studentMuted);
            }
            else
            {
                studentItemCard.UpdateNameText("No Name");
                studentItemCard.UpdateStudentColor(Color.red);
            }
        }

        gridObject.UpdateCollection();
        scrollObject.UpdateContent();
        StartCoroutine(DelayResetList());
    }

    public void CloseStudentSpawnerMenu()
    {
        SelectStudentsUI.Instance.DisableStudentsScrollUI();
        pressableButtonsClicked = false;
        pressableButtons.SetActive(false);
        metaDataPanel.gameObject.SetActive(false);
    }

    #endregion

    #region Coroutines

    IEnumerator DelayResetList()
    {
        yield return null;
        gridObject.UpdateCollection();
        scrollObject.UpdateContent();
    }

    #endregion

    #region Private Methods

    private void CleanStudentList()
    {
        if(studentList != null && studentList.Count > 0)
        {
            foreach(GameObject student in studentList)
            {
                Destroy(student);
            }

            //studentList.Clear();
            studentList = new List<GameObject>();
        }
    }

    #endregion

}
