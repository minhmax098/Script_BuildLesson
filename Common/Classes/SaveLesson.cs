using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLesson : MonoBehaviour
{
    public static int lessonId; 
    public static void SaveLessonId (int _lessonId)
    {
        lessonId = _lessonId;
    }
}
