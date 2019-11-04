using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectListController : MonoBehaviour
{

    public SubjectEditController subjectEditController;

    public void CreateSubject()
    {
        gameObject.SetActive(false);
        subjectEditController.gameObject.SetActive(true);
        subjectEditController.InitializeForCreate();
    }
}
