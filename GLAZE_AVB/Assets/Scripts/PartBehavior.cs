﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartBehavior : MonoBehaviour
{
    private Collider wallcollider;
    [SerializeField] AudioClip Repairclip;
    private bool repairing = false;
    [SerializeField] private float repairTimer = 2f;
    private bool building = false;
    [SerializeField] private float buildTimer = 4f;
    private float _timer = 0;

    [SerializeField] private int buildCost = 3;
    [SerializeField] private int repairCost = 1;

    private int state = 3;

    [SerializeField] private Sprite sprTrenchFull;
    [SerializeField] private Sprite sprTrenchBroken;
    [SerializeField] private SpriteRenderer sprRenderer;
    [SerializeField] private List<GameObject> antList;

    private GameManager gmc;

    // Start is called before the first frame update
    void Start()
    {
        wallcollider = gameObject.GetComponent<Collider>();
        sprRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        UpdateState();
        gmc = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(repairing)
        {
            Building(repairTimer);
        }
        if (building)
        {
            Building(buildTimer);
        }
    }

    private void UpdateState()
    {
        if(state == 0)
        {
            sprRenderer.sprite = sprTrenchBroken;
            for (int i = 0; i < antList.Count; i++)
            {
                antList[i].SetActive(false);
            }
        }
        else {
            for (int i = 0; i < antList.Count; i++)
            {
                if ((state-1) >= i) antList[i].SetActive(true);
                else antList[i].SetActive(false);
            }
        }
    }

    // Enemy steals part
    public bool StealPart()
    {
        if (state == 0) return false;
        state--;

        UpdateState();

        return true;
    }

    // Start repairing
    public void RepairPart()
    {
        if (state == 0) return; // If wall needs to be rebuild

        wallcollider.isTrigger = false;

        // Check if able to repair
        if (Inventory._instance.PayCost(repairCost))
        {
            _timer = 0;
            repairing = true;
            AudioSource.PlayClipAtPoint(Repairclip, transform.position);
        }
    }

    // For buying or recollecting part
    public void BuildPart()
    {
        if (state != 0) return; // If wall needs to be repaired

        wallcollider.isTrigger = false;

        // Check if able to repair
        if (Inventory._instance.PayCost(buildCost))
        {
            _timer = 0;
            building = true;
        }
    }

    // Buy wall
    public void Building(float timer)
    {
        _timer += Time.deltaTime;
        if (_timer > timer)
        {
            Debug.Log("Repaired");
            state = 3;
            UpdateState();
            wallcollider.isTrigger = true;
            repairing = false;
        }
    }

    // Capture mouse
    private void OnMouseDown()
    {
        if (gmc.getMode() == inputMode.repair)
        {
            RepairPart();
        }
        if(gmc.getMode() == inputMode.build)
        {
            BuildPart();
        }
    }
}
