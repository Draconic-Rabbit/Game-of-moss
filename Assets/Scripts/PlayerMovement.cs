using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool controlEnabled = true;
    Vector2 rawInput;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject body;

    [SerializeField] float paddingBounds = 0.5f;
    Vector2 minBounds;
    Vector2 maxBounds;

    // Start is called before the first frame update
    void Start()
    {
        initBounds();

    }

    void initBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(Vector2.zero) + new Vector3(paddingBounds, paddingBounds, 0);
        maxBounds = mainCamera.ViewportToWorldPoint(Vector2.one) - new Vector3(paddingBounds, paddingBounds, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LookAt();
    }


    // should be use to only affect sprite, not parent
    private void LookAt()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        body.transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }

    private void Move()
    {
        Vector3 delta = rawInput * speed * Time.deltaTime;
        Vector2 newPos = new Vector2();
        newPos.x = Mathf.Clamp((transform.position.x + delta.x), minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp((transform.position.y + delta.y), minBounds.y, maxBounds.y);
        transform.position = newPos;
    }

    void OnMove(InputValue value)
    {
        if (!controlEnabled) { return; }
        rawInput = value.Get<Vector2>();
    }
}
