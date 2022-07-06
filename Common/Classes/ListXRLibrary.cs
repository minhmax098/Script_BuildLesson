using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListXRLibrary
{
   public int code; 
   public string message; 
   public OrganForHome[] data; 
}

[System.Serializable]
public class OrganForHome
{
    public int organsId;
    public string organsName; 
    public LessonForHome[] listLesson; 
}

[System.Serializable]
public class LessonForHome
{
    public int lessonId;
    public string lessonTitle;
    public string lessonThumbnail; 
    public int isActive; 
    public int isPublic; 
}
