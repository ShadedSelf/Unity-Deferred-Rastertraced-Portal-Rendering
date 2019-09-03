using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class w : MonoBehaviour
{
    void OnEnable()
	{
        StartCoroutine(GetText());
    }
 
    IEnumerator GetText()
	{
        UnityWebRequest www = UnityWebRequest.Get("http://www.google.com");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError) 
		{
            Debug.Log(www.error);
        }
        else 
		{
            Debug.Log(www.downloadHandler.text);
            // byte[] results = www.downloadHandler.data;
        }
    }
}
