using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is a global singleton pattern that uses generic type. 
/// Game objects that want to be singleton needs to be inherite from this class
/// DontdestroyonLoad is also implemented -- then the gameobject would be persistence during the different scenes.
/// 
/// 
/// /// Note: The lazy instantiation occurs in the Instance property:
/// 1. First access checks if instance exists
/// 2. If null, searches for existing component
/// 3. If not found, creates new GameObject with component
/// 
/// 
/// <typeparam name="T"></typeparam>
public abstract class singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if(instance == null)
                {
                    SetUpInstance();
                }
            }
            return instance;
        
        }


    }
    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetUpInstance()
    {
        instance = (T)FindObjectOfType(typeof(T));
        if(instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;
            instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);


        }
    }

    private void RemoveDuplicates()
    {

        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    }






}
