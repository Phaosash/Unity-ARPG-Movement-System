using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Controller: MonoBehaviour {
    //  Use this to create a reference to the main camera in the scene
    private Camera camera;

    //  Use this to create a reference to the NavMeshAgent component attached to the GameObject
    private NavMeshAgent navMeshAgent;

    //  Use this to control the distance that the player will stop from the target position when moving
    [Header("Movement")]
    [SerializeField] private float stoppingDistance = 0.15f;

    //  Use this to set the players walking speed
    [Header("Sprint")]
    [SerializeField] private float walkSpeed = 5f;

    //  Use this to set the players sprinting speed
    [SerializeField] private float sprintSpeed = 8f;

    //  This method is called when the script instance is being loaded, and is used to initialize the script.
    //  Currently, it gets the NavMeshAgent component attached to the GameObject and sets its stopping distance.
    private void Awake (){
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistance;
    }

    //  This method is called before the first frame update, and is used to initialize the script.
    //  Currently, it sets the camera variable to the main camera in the scene.
    private void Start (){
        camera = Camera.main;
    }

    //  This method is called once per frame, after all Update methods have been called.
    private void Update (){
        HandlePlayerMovement();
    }

    //  This method handles the player's movement based on mouse input.
    //  Clicking or holding the left mouse button will move the player to the mouse position,
    //  and pressing the middle mouse button will make the player sprint.
    private void HandlePlayerMovement (){
        if (Mouse.current == null) return;

        bool isClicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool isHolding = Mouse.current.leftButton.isPressed;
        bool isSprinting = Mouse.current.middleButton.wasPressedThisFrame;
        bool isHoldingSprint = Mouse.current.middleButton.isPressed;

        if (isClicked || isHolding){
            MoveToMousePosition(false);
        }

        if (isSprinting || isHoldingSprint){
            MoveToMousePosition(true);
        }

    }

    //  This method moves the player to the position of the mouse cursor on the screen.
    //  If sprinting is enabled, the player's speed is set to sprintSpeed; otherwise,
    //  it is set to walkSpeed.
    private void MoveToMousePosition (bool isSprinting){
        if (isSprinting){
            navMeshAgent.speed = sprintSpeed;
        } else {
            navMeshAgent.speed = walkSpeed;
        }

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo)){
            if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas)){
                navMeshAgent.SetDestination(navMeshHit.position);
            }
        }
    }
}