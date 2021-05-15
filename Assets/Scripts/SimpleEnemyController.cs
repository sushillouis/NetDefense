using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PACKET_LIFECYCLE_STATUS {
    UNSPAWNED,
    ENROUTE,
    ROUTER_TAKE_DOWN,
    ARRIVED_AT_DESTINATION,
    ARRIVED_AT_HONEYPOT
}

public class SimpleEnemyController : NetworkBehaviour {

    //For tuning movement and scoring values
    public float movementSpeed = 16;
    public int damage = 1;

    //For tuning pulse behavior
    public Gradient pulseGradient;
    public float pulseDuration;
    public float pulseSpeed;

    //public GameObject dynamicHud;
    //public GameObject badPacketOutline;


    //Trait values
    [SyncVar]
    public int color;
    [SyncVar]
    public int size;
    [SyncVar]
    public int shape;
    [SyncVar]
    public bool malicious;
    [SyncVar]
    public int id;
    [SyncVar]
    public PACKET_LIFECYCLE_STATUS status;

    public Vector3 spawnPos;

    public static int instance_id;

    //Target destination
    public Destination destination;

    //Path for packet to follow
    public Path path;

    //Holds current position in path
    private int waypointIndex = 1;

    //Used to check when waypoint has been reached
    private float lastDistance = Mathf.Infinity;
    private float distance;

    public bool isReadyForRemoval;

    public GameObject selectedChild;

    public void Start() {

        if (EntityManager.inst.isMultiplayer && !EntityManager.inst.isServer) {
            setupBehavior(color, size, shape, malicious);
        }
    }

    /*   IEnumerator Pulsate() {
           if (MainMenu.difficulty == Difficulty.EASY) {
               GradientColorKey[] colorKey = pulseGradient.colorKeys;
               colorKey[0].color = GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
               pulseGradient.SetKeys(colorKey, pulseGradient.alphaKeys);
               while (true) {
                   float value = Mathf.PingPong(Time.time * pulseSpeed, 1);
                   Color color = pulseGradient.Evaluate(value);
                   GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
                   yield return null;
               }
           }
       }*/
    /*
    IEnumerator ClearPulseColor() {
        //Color color = pulseGradient.Evaluate(0);
        // GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        yield return null;
    }*/


    public void Tick() {

        // badPacketOutline.SetActive(status == PACKET_LIFECYCLE_STATUS.ENROUTE && malicious && MainMenu.difficulty == Difficulty.EASY);
        UpdateBadBackProperties();

        if (status == PACKET_LIFECYCLE_STATUS.UNSPAWNED)
            transform.position = new Vector3(spawnPos.x, spawnPos.y - (5 * instance_id), spawnPos.z);


        if (status == PACKET_LIFECYCLE_STATUS.ENROUTE)
            GoEnroute();


        if (status == PACKET_LIFECYCLE_STATUS.ARRIVED_AT_DESTINATION ||
            status == PACKET_LIFECYCLE_STATUS.ROUTER_TAKE_DOWN)
            transform.position = new Vector3(spawnPos.x, spawnPos.y - (5 * instance_id), spawnPos.z);

    }

    /**
     *
     * Do transformations through the transformation pipline to go from world space to scaled canvas space
     *
     */
    //private void UpdateDynamicHud() {

    //    RectTransform rt = dynamicHud.GetComponent<RectTransform>();

    //    Vector2 screen_space = Camera.main.WorldToScreenPoint(transform.position);

    //    float scaleFactor = MainCanvasManager.inst.scaleFactor();

    //    Vector2 scaled = new Vector2(screen_space.x / scaleFactor, screen_space.y / scaleFactor);

    //    Vector2 finalTransform = new Vector2(scaled.x - rt.rect.width / 2, scaled.y - rt.rect.height / 2);

    //    badPacketOutline.GetComponent<RectTransform>().localPosition = finalTransform;


    //}


    public void UpdateBadBackProperties() {
        if (malicious && status == PACKET_LIFECYCLE_STATUS.UNSPAWNED) {
            // bool shouldUpdate = !(Shared.inst.isBadPacket(color, shape, size));


            color = Shared.inst.maliciousPacketProperties.color;
            shape = Shared.inst.maliciousPacketProperties.shape;
            size = Shared.inst.maliciousPacketProperties.size;

            Game_Manager.inst.SetTraits(this);
        }
    }

    public void GoEnroute() {

        //UpdateDynamicHud();


        if (path != null) {
            Vector3 direction = path.waypoints[waypointIndex].position - path.waypoints[waypointIndex - 1].position;
            Vector3 vec_value = (direction.magnitude > 0) ? (direction / direction.magnitude) : Vector3.zero;
            float velocityScalar = MainMenu.difficulty == Difficulty.EASY ? .6f : 1f;
            GetComponent<Rigidbody>().velocity = ((direction * movementSpeed) / direction.magnitude) * velocityScalar;
            distance = Mathf.Abs((transform.position - path.waypoints[waypointIndex].position).magnitude);


            if (distance > lastDistance) {
                transform.position = new Vector3(path.waypoints[waypointIndex].position.x, 0, path.waypoints[waypointIndex].position.z);

                if (waypointIndex + 1 < path.waypoints.Count) {
                    waypointIndex++;
                    transform.LookAt(new Vector3(path.waypoints[waypointIndex].position.x, 0, path.waypoints[waypointIndex].position.z));
                    lastDistance = Mathf.Infinity;
                } else {
                    if (!malicious) {
                        // ScoreManager.inst.OnFriendlyPacketTransfered(id);
                        isReadyForRemoval = true;
                    } else {
                        if (!path.IsHoneypot) {
                            //ScoreManager.inst.OnBadPacketTransfered(id);

                            isReadyForRemoval = true;
                        }
                    }
                    // verify solo player is working correctly
                }
            } else {
                lastDistance = distance;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Destination") {

			// TODO: Rigerously test solution (it appears to be working on every difficulty)
        	Destination dest = gameObject.GetComponent<Destination>(); // TODO: Why would objects tagged as destinations not have a destination component attached?
			if(dest) status = !dest.isHoneypot ? PACKET_LIFECYCLE_STATUS.ARRIVED_AT_HONEYPOT : PACKET_LIFECYCLE_STATUS.ARRIVED_AT_DESTINATION;
			else status = PACKET_LIFECYCLE_STATUS.ARRIVED_AT_DESTINATION;

            if (malicious) {
                ParticleSystem effect = other.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
                effect.Play();
                Camera.main.transform.GetChild(0).GetComponent<AudioSource>().Play();
                ScoreManager.inst.OnBadPacketTransfered(id);
            } else {
                ScoreManager.inst.OnFriendlyPacketTransfered(id);

            }
        }

        if (other.gameObject.tag == "Router") {
            Router r = other.gameObject.GetComponent<Router>();

            if (r.color == color && r.shape == shape && r.size == size) {
                status = PACKET_LIFECYCLE_STATUS.ROUTER_TAKE_DOWN;
                if (malicious) {
                    Camera.main.transform.GetChild(1).GetComponent<AudioSource>().Play();

                }
                if (EntityManager.inst.isMultiplayer && !EntityManager.inst.isServer) {
                    Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.CHANGE_PACKET_LIFECYCLE_STATUS, id + "," + (int)PACKET_LIFECYCLE_STATUS.ROUTER_TAKE_DOWN));
                }
            }
        }
    }

    public void requestTurnOnBadPacketOutline(bool isbad) {
        selectedChild.SetActive(isbad && MainMenu.difficulty == Difficulty.EASY);
    }


    public void setupBehavior(int newColor, int newSize, int newShape, bool newMalicious, bool updateClient = false) {
        this.color = newColor;
        this.size = newSize;
        this.shape = newShape;
        this.malicious = newMalicious;

        this.id = instance_id++;
        status = PACKET_LIFECYCLE_STATUS.UNSPAWNED;
        spawnPos = gameObject.transform.position;

        requestTurnOnBadPacketOutline(newMalicious);

        float selectionScale = Mathf.Max(transform.localScale.x * selectedChild.transform.localScale.x, transform.localScale.z * selectedChild.transform.localScale.z);
        selectionScale *= 1.5f;
        selectedChild.transform.localScale = new Vector3(selectionScale, selectedChild.transform.localScale.y, selectionScale);

        Game_Manager.inst.SetTraits(this);
        //if (newMalicious)
        //    StartCoroutine("Pulsate");
        if (!EntityManager.inst.isMultiplayer || (EntityManager.inst.isMultiplayer && EntityManager.inst.isServer) || updateClient) {
            int index = Random.Range(0, destination.paths.Count);
            path = destination.paths[index];

            SetSpawnRotandPos();
        }

        //dynamicHud = GameObject.FindGameObjectWithTag("DYNAMIC_INFO_HUD");
        //badPacketOutline = Instantiate(badPacketOutline);
        //badPacketOutline.transform.SetParent(dynamicHud.transform);
    }

    public void SetSpawnRotandPos() {
        if (path.waypoints[0] == null) {
            Debug.Log("BIG PROBLEM");
            return;
        }

        transform.position = new Vector3(path.waypoints[0].position.x, 0, path.waypoints[0].position.z); // null and missing reference
        transform.LookAt(new Vector3(path.waypoints[waypointIndex].position.x, 0, path.waypoints[waypointIndex].position.z));
    }

    public void OnDeployed() {
        // update bad packet type
        UpdateBadBackProperties();



        // update transform to spawn pos
        transform.position = new Vector3(path.transform.position.x, 0, path.transform.position.z);

        //UpdateDynamicHud();
    }
}
