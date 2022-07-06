using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class APIUrlConfig
{
    // API public 
    public static string SignIn = "https://api.xrcommunity.org/v1/xap/user/login";
    public static string SignUp = "https://api.xrcommunity.org/v1/xap/user/signup";
    public static string ForgotPass = "https://api.xrcommunity.org/v1/xap/user/forgotPassword";
    public static string ResetPass = "https://api.xrcommunity.org/v1/xap/user/resetPassword";
    public static string GetListLessonByCategory = "https://api.xrcommunity.org/api/lessons/listByCategoryId?categoryId={0}";
    public static string LoadLesson = "https://api.xrcommunity.org/v1/xap/{0}"; 
    public static string GetCategoryWithLesson = "https://api.xrcommunity.org/v1/xap/organs/getListXRLibrary"; 
    public static string GetListLessons = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId=&searchValue={0}&offset={1}&limit={2}"; 
    public static string GetListLessonsByOrgan = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId={0}&searchValue={1}&offset={2}&limit={3}"; 
    public static string GetLessonsByID = "https://api.xrcommunity.org/v1/xap/lessons/getLessonDetail/{0}"; 

    public static string GetListMyLesson = "https://api.xrcommunity.org/v1/xap/lessons/getListMyLesson?limit={0}&offset={1}&searchValue={2}"; 
    public static string GetListOrgans = "https://api.xrcommunity.org/v1/xap/organs/getListOrgans";
    public static string GetList3DModel = "https://api.xrcommunity.org/v1/xap/models/getList3DModel?type={0}&searchValue={1}&offset={2}&limit={3}";

    public static string UpdateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/updateLessonInfo/{0}";
    public static string CreateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/createLessonInfo"; 
    public static string Get3DModelDetail = "https://api.xrcommunity.org/v1/xap/models/get3DModelDetail/{0}";

    public static string Upload3DModel = "https://api.xrcommunity.org/v1/xap/stores/upload3DModel";
    public static string Import3DModel = "https://api.xrcommunity.org/v1/xap/models/import3DModel";
    public static string CreateModelLabel = "https://api.xrcommunity.org/v1/xap/labels/createModelLabel";

    public static string AddAudioLesson = "https://api.xrcommunity.org/v1/xap//lessons/addAudioLesson";
    public static string AddVideoLesson = "https://api.xrcommunity.org/v1/xap/lessons/addVideoLesson";
    public static string AddAudioLabel = "https://api.xrcommunity.org/v1/xap/lessons/addAudioLabel";
    public static string AddVideoLabel = "https://api.xrcommunity.org/v1/xap/lessons/addVideoLabel";

    public static string GetLinkVideo = "https://www.youtube.com/oembed?url={0}&format=json";
    public static string GetLinkAPIVideo = "https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=statistics";
    public static string DeleteAudioLesson = "https://api.xrcommunity.org/v1/xap/lessons/deleteAudioLesson/{0}";
    public static string DeleteVideoLesson = "https://api.xrcommunity.org/v1/xap/lessons/deleteVideoLesson/{0}";
    public static string DeleteLabel = "https://api.xrcommunity.org/v1/xap/labels/deleteLabel/{0}";

    public static string BASE_URL = "https://api.xrcommunity.org/v1/xap/";

    // API private 
    // public static string SignIn = "http://10.60.156.166:8556/v1/xap/user/login";
    // public static string SignUp = "http://10.60.156.166:8556/v1/xap/user/signup";
    // public static string ForgotPass = "http://10.60.156.166:8556/v1/xap/user/forgotPassword";
    // public static string ResetPass = "http://10.60.156.166:8556/v1/xap/user/resetPassword";
    // public static string GetListLessonByCategory = "http://10.60.156.166:8556/v1/xap/lessons/listByCategoryId?categoryId={0}";
    // public static string LoadLesson = "http://10.60.156.166:8556/v1/xap/{0}"; 
    // public static string GetCategoryWithLesson = "http://10.60.156.166:8556/v1/xap/organs/getListXRLibrary"; 
    // public static string GetListLessons = "http://10.60.156.166:8556/v1/xap/organs/getListLessonByOrgan?organId=&searchValue={0}&offset={1}&limit={2}"; 
    // public static string GetListLessonsByOrgan = "http://10.60.156.166:8556/v1/xap/organs/getListLessonByOrgan?organId={0}&searchValue={1}&offset={2}&limit={3}"; 
    // public static string GetLessonsByID = "http://10.60.156.166:8556/v1/xap/lessons/getLessonDetail/{0}"; 
    // public static string GetListMyLesson = "http://10.60.156.166:8556/v1/xap/lessons/getListMyLesson?limit={0}&offset={1}&searchValue={2}"; 
    // public static string GetListOrgans = "http://10.60.156.166:8556/v1/xap/organs/getListOrgans";
    // public static string GetList3DModel = "http://10.60.156.166:8556/v1/xap/models/getList3DModel?type={0}&searchValue={1}&offset={2}&limit={3}";
    // public static string UpdateLessonInfo = "http://10.60.156.166:8556/v1/xap/lessons/updateLessonInfo/{0}";
    // public static string CreateLessonInfo = "http://10.60.156.166:8556/v1/xap/lessons/createLessonInfo"; 
    // public static string Get3DModelDetail = "http://10.60.156.166:8556/v1/xap/models/get3DModelDetail/{0}";
    // public static string Upload3DModel = "http://10.60.156.166:8556/v1/xap/stores/upload3DModel";
    // public static string Import3DModel = "http://10.60.156.166:8556/v1/xap/models/import3DModel";
    // public static string CreateModelLabel = "http://10.60.156.166:8556/v1/xap/labels/createModelLabel";
    // public static string AddAudioLesson = "http://10.60.156.166:8556/v1/xap/lessons/addAudioLesson";
    // public static string AddVideoLesson = "http://10.60.156.166:8556/v1/xap/lessons/addVideoLesson";
    // public static string AddAudioLabel = "http://10.60.156.166:8556/v1/xap/lessons/addAudioLabel";
    // public static string AddVideoLabel = "http://10.60.156.166:8556/v1/xap/lessons/addVideoLabel";
    // public static string GetLinkVideo = "https://www.youtube.com/oembed?url={0}&format=json";
    // public static string GetLinkAPIVideo = "https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=statistics";
    // public static string DeleteAudioLesson = "http://10.60.156.166:8556/v1/xap/lessons/deleteAudioLesson/{0}";
    // public static string DeleteVideoLesson = "http://10.60.156.166:8556/v1/xap/lessons/deleteVideoLesson/{0}";
    // public static string DeleteLabel = "http://10.60.156.166:8556/v1/xap/labels/deleteLabel/{0}";
}
