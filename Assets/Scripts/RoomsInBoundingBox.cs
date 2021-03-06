﻿using Assets.Scripts;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RoomsInBoundingBox : MonoBehaviour
{
    public GameObject plane;

    // objects
    public GameObject workingSpaceA;
    public GameObject workingSpaceB;
    public GameObject workingSpaceC;
    public GameObject Kitchen;
    public GameObject Bathrooms;
    public GameObject Relaxation;
    public GameObject Common;
    public GameObject Keynote;
    public GameObject Core;
    public GameObject Mentors;
    public GameObject Hardware;

    // ui
    public GameObject feedbackSlider;

    public GameObject textPrefab;
    public string[] scoreBarText = new string[] {
    "Hackers\nx\nMentors",
    "Hackers\nx\nBathrooms",
    "Quiet\nx\nKitchen",
    "Hackers\nx\nWindows",
    "Commons\nx\nKeynote",
    "Commons\nx\nBathroom",
    "Hardware\nx\nMentors",
    "Kitchen\nx\nCommons"
    };



    List<GameObject> rooms = new List<GameObject>();

    List<GameObject> instantiated = new List<GameObject>();
    Population pop;

    Vector3 position;
    float width;
    float depth;

    GameObject _wsA, _wsB, _wsC,
        _mentors, _kitchen, _relax,
        _bathroom, _common, _core, _keynote,
        _hardware;


    public float[] scores;
    public GameObject[] sliders;
    public GameObject[] scoreTexts;


    float scaleMax = 0.5f;

    float targetScaleMin = 0.2f;
    float targetScaleMax = 0f;

    public List<Timestamp> timeLine;
    GameObject[] objects;
    GameObject[] objectPositions;
    GameObject[] objectPositions2;

    bool positionChanged = true;
    public bool DisplayHistory = false;
    public float historyStep= 0f;
    bool runningTimestamp = false;
    bool isShowingBarGraph = true;

    // Start is called before the first frame update
    void Start()
    {
        scores = new float[8];
        timeLine = new List<Timestamp>();
        objects = new GameObject[11];
        objectPositions = new GameObject[11];
        objectPositions2 = new GameObject[11];

        // general
        position = this.transform.position;
        width = this.transform.localScale.x;
        depth = this.transform.localScale.z;

        // rooms
        _wsA = UnityEngine.Object.Instantiate(workingSpaceA);
        _wsB = UnityEngine.Object.Instantiate(workingSpaceB);
        _wsC = UnityEngine.Object.Instantiate(workingSpaceC);

        _kitchen = UnityEngine.Object.Instantiate(Kitchen);
        _bathroom = UnityEngine.Object.Instantiate(Bathrooms);
        _relax = UnityEngine.Object.Instantiate(Relaxation);
        _mentors = UnityEngine.Object.Instantiate(Mentors);
        _common = UnityEngine.Object.Instantiate(Common);
        _keynote = UnityEngine.Object.Instantiate(Keynote);
        _core = UnityEngine.Object.Instantiate(Core);
        _hardware = UnityEngine.Object.Instantiate(Hardware);

        // ui
        GetScores();
        sliders = DisplaySliders(scores, feedbackSlider);
        scoreTexts = DisplayScoreText(scoreBarText, textPrefab);

        //text


        AddObjectsToArray();
        AddObjectositionsToArray();
        AddObjectositionsToArray2();

        var stamp = new Timestamp(objectPositions, scores);
        timeLine.Add(stamp);
        Debug.Log("timestamp added");



    }

    // Update is called once per frame
    void Update()
    {
        

        if (DisplayHistory)
        {

            runningTimestamp = true;

            int index = (int)Math.Round(historyStep * timeLine.Count);

            for (int i = 0; i < objects.Length; i++)
            {
                objectPositions[i].transform.position = timeLine[index].positions[i];
                objectPositions[i].transform.localScale = timeLine[index].scale[i];
            }
        }

        else
        {
            //historyStep = 1f;
            if (runningTimestamp)
            {

                for (int i = 0; i < objects.Length; i++)
                {
                    objectPositions[i].transform.position = timeLine[timeLine.Count-1].positions[i];
                    objectPositions[i].transform.localScale = timeLine[timeLine.Count - 1].scale[i];
                }

                runningTimestamp = false;
            }

            GetScores();
            UpdateSliders(scores, .8f);

            positionChanged = CheckPositionChanged();

            if (positionChanged)
            {
                var stamp = new Timestamp(objectPositions, scores);

                timeLine.Add(stamp);
                Debug.Log("timestamp added");
            }
        }



    }


    bool CheckPositionChanged()
    {
        var lastStamp = timeLine[timeLine.Count - 1];

        for (int i = 0; i < objects.Length; i++)
        {
            if (objectPositions[i].transform.position.x != lastStamp.positions[i].x
                || objectPositions[i].transform.position.y != lastStamp.positions[i].y
                || objectPositions[i].transform.position.z != lastStamp.positions[i].z
                || objectPositions[i].transform.localScale.x != lastStamp.scale[i].x
                || objectPositions[i].transform.localScale.y != lastStamp.scale[i].y
                || objectPositions[i].transform.localScale.z != lastStamp.scale[i].z
                )
            {
                return true;
            }
        }

        return false;
    }

    void GetScores()
    {
        scores[0] = GetScore1(_wsA.GetComponent<getXYZ>().myNewEmpty, _wsB.GetComponent<getXYZ>().myNewEmpty, _wsC.GetComponent<getXYZ>().myNewEmpty, _mentors.GetComponent<getXYZ>().myNewEmpty);
        scores[1] = GetScore1(_wsA.GetComponent<getXYZ>().myNewEmpty, _wsB.GetComponent<getXYZ>().myNewEmpty, _wsC.GetComponent<getXYZ>().myNewEmpty, _mentors.GetComponent<getXYZ>().myNewEmpty);
        scores[2] = GetScore1(_wsA.GetComponent<getXYZ>().myNewEmpty, _wsB.GetComponent<getXYZ>().myNewEmpty, _wsC.GetComponent<getXYZ>().myNewEmpty, plane);
        scores[3] = GetScore5(_kitchen.GetComponent<getXYZ>().myNewEmpty, _relax.GetComponent<getXYZ>().myNewEmpty);
        scores[4] = GetScore5(_common.GetComponent<getXYZ>().myNewEmpty, _keynote.GetComponent<getXYZ>().myNewEmpty);
        scores[5] = GetScore5(_common.GetComponent<getXYZ>().myNewEmpty, _bathroom.GetComponent<getXYZ>().myNewEmpty);
        scores[6] = GetScore5(_hardware.GetComponent<getXYZ>().myNewEmpty, _mentors.GetComponent<getXYZ>().myNewEmpty);
        scores[7] = GetScore5(_kitchen.GetComponent<getXYZ>().myNewEmpty, _common.GetComponent<getXYZ>().myNewEmpty);

        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = Remap(scores[i], 0, scaleMax, targetScaleMin, targetScaleMax);
        }

        scores[2] = targetScaleMin - scores[2];

    }

    GameObject[] DisplaySliders(float[] scores, GameObject slider)
    {
        GameObject[] sliders = new GameObject[scores.Length];
    
        for (int i = 0; i < scores.Length; i++)
        {

            sliders[i] = UnityEngine.Object.Instantiate(slider, new Vector3((0.05f * i)-0.175f, 1.25f, 1.17f), new Quaternion());
        }

        return sliders;
    }

    GameObject[] DisplayScoreText(string[] barText, GameObject textPrefab)
    {
        GameObject[] texts = new GameObject[barText.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i] = UnityEngine.Object.Instantiate(textPrefab, new Vector3((0.05f * i) - 0.175f, 0.02f, 1.17f), new Quaternion());
            GameObject textObject = texts[i].transform.GetChild(1).gameObject;
            textObject.GetComponent<TextMeshProUGUI>().text = barText[i];
            //grandChild = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        }

        return texts;
    }

    void AddObjectsToArray()
    {
        objects[0] = _wsA;
        objects[1] = _wsB;
        objects[2] = _wsC;
        objects[3] = _kitchen;
        objects[4] = _bathroom;
        objects[5] = _relax;
        objects[6] = _mentors;
        objects[7] = _common;
        objects[8] = _keynote;
        objects[9] = _core;
        objects[10] = _hardware;
    }

    void AddObjectositionsToArray()
    {
        objectPositions[0] = _wsA.GetComponent<getXYZ>().myChildCube;
        objectPositions[1] = _wsB.GetComponent<getXYZ>().myChildCube;
        objectPositions[2] = _wsC.GetComponent<getXYZ>().myChildCube;
        objectPositions[3] = _kitchen.GetComponent<getXYZ>().myChildCube;
        objectPositions[4] = _bathroom.GetComponent<getXYZ>().myChildCube;
        objectPositions[5] = _relax.GetComponent<getXYZ>().myChildCube;
        objectPositions[6] = _mentors.GetComponent<getXYZ>().myChildCube;
        objectPositions[7] = _common.GetComponent<getXYZ>().myChildCube;
        objectPositions[8] = _keynote.GetComponent<getXYZ>().myChildCube;
        objectPositions[9] = _core.GetComponent<getXYZ>().myChildCube;
        objectPositions[10] = _hardware.GetComponent<getXYZ>().myChildCube;
    }

    void AddObjectositionsToArray2()
    {
        objectPositions[0] = _wsA.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[1] = _wsB.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[2] = _wsC.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[3] = _kitchen.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[4] = _bathroom.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[5] = _relax.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[6] = _mentors.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[7] = _common.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[8] = _keynote.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[9] = _core.GetComponent<getXYZ>().myNewEmpty;
        objectPositions[10] = _hardware.GetComponent<getXYZ>().myNewEmpty;
    }


    void UpdateSliders(float[] scores, float barScaler)
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            //sliders[i] = Object.Instantiate(slider, new Vector3(10 * i, 40, 40), new Quaternion());

            //sliders[i].transform.localScale = new Vector3(1, scores[i], 1);
            float posX = sliders[i].transform.position.x;
            float posZ = sliders[i].transform.position.z;
            float posY = sliders[i].transform.position.y;

            float scaleX = sliders[i].transform.localScale.x;
            float scaleZ = sliders[i].transform.localScale.z;

            float multiplier = Math.Abs(scores[i] * barScaler);

            sliders[i].transform.localScale = new Vector3(scaleX, multiplier, scaleZ);
            sliders[i].transform.position = new Vector3(posX, (multiplier / 2) + 0.05f, posZ);
        }
    }


    void InstantiateNotOverlappingRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Vector3 location;
            //var location = GenerateRoomLocation(rooms[i]);
            if (instantiated.Count == 0)
            {
                location = Utilities.GenerateRoomLocation(rooms[i], width, depth);
            }
            else
            {
                location = Utilities.GenerateRoomLocationBasedOnAnotherRoom(rooms[i], instantiated[i - 1], instantiated);
            }

            instantiated.Add(UnityEngine.Object.Instantiate(rooms[i], location, new Quaternion()));
        }
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // distance from working spaces to mentors
    float GetScore1(GameObject wsA, GameObject wsB, GameObject wsC, GameObject mentors)
    {
        float score = 0;
        var vectorA = wsA.transform.position - mentors.transform.position;
        var vectorB = wsB.transform.position - mentors.transform.position;
        var vectorC = wsC.transform.position - mentors.transform.position;

        score = vectorA.magnitude + vectorA.magnitude + vectorC.magnitude;


        return score;
    }




    float GetScore5(GameObject common, GameObject keynote)
    {
        float score = 0;
        var vectorA = common.transform.position - keynote.transform.position;

        score = vectorA.magnitude;
        return score;
    }

    public void ToggleBarGraph()
    {
        isShowingBarGraph = !isShowingBarGraph;

        for(int i = 0; i < sliders.Length; i++)
        {
            sliders[i].SetActive(isShowingBarGraph);
        }
    }

    public void ToggleHistory()
    {
        DisplayHistory = !DisplayHistory;
        //HistorySlider.SetActive(DisplayHistory);
    }

    public void HistorySliderChanging(SliderEventData data)
    {
        Debug.Log("Value: " + data.NewValue.ToString());
        historyStep = data.NewValue;
    }

    public void OnSliderUpdated(SliderEventData data)
    {
        Debug.Log("Test value: " + data.NewValue.ToString());
    }

}
