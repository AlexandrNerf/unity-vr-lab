using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockButton : MonoBehaviour
{
    [SerializeField] int blockID;       // ID блока этой кнопки
    Builder builder;                     // ссылка на Builder

    // Start вызывается перед первым кадром
    void Start()
    {
        // Находим объект с тегом "Builder" и получаем компонент Builder
        builder = GameObject.FindGameObjectWithTag("Builder").GetComponent<Builder>();
    }

    // Метод вызывается при нажатии на кнопку UI
    public void Click()
    {
        builder.SpawnBlock(blockID);    // спавним блок с этим ID
    }
}