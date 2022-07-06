using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LessonDetail
{
    public int lessonId; 
    public int organId;
    public string lessonTitle;
    public string lessonObjectives;  
    public string lessonThumbnail;
    public int isPublic;
    public int viewed;  
    public int modelId;
    public string size; 
    public string modelFile;
    public string audio;
    public string video;
    public int isMuteAll;
    public int isPresenterAll;
    public string authorName; 
    public int isActive;
    public int createdBy; 
    public string createdDate; 
    public ListLabelLesson[] listLabel;
}

[System.Serializable]
public class AllLessonDetails
{
    public int code; 
    public string message; 
    public LessonDetail[] data; 
}

[System.Serializable]
public class ListLabelLesson
{
    public int labelId;
    public string labelName;
    public Coordinate[] coordinates;
    public string labelIndex;
    public int level;
    public int parentId;
    public string videoLabel;
    public string audioLabel;
}

[System.Serializable]
public class Coordinate
{
    public int x;
    public int y; 
    public int z;
}
