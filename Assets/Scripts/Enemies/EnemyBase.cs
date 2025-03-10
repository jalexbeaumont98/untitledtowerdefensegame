using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class EnemyBase : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Vector3 tileTarget;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected GameState gameState;
    [SerializeField] protected Tilemap towerMap;
    [SerializeField] protected Sprite frontSprite;
    [SerializeField] protected Sprite backSprite;
    [SerializeField] protected Sprite leftSprite;
    [SerializeField] protected Sprite rightSprite;


    [Header("Attributes")]
    [SerializeField] public string enemyName;
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected int armor;
    [SerializeField] protected int shield;
    [SerializeField] protected int reward;
    [SerializeField] protected int damage;
    [SerializeField] protected int pathMode;
    [SerializeField] protected bool stealth;
    [SerializeField] protected bool flying;
    [SerializeField] protected float minimumDistance;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected float minAnimTime;
    [SerializeField] protected string description;

    protected NavMeshPath path;
    protected NavMeshPath oldPath;
    protected float elapsed = 0.0f;

    bool selected;

    protected string newAgentTypeName = "Flying"; // The name of the new agent type (e.g., "Humanoid", "Small")

    protected GameObject selectedHighlightPrefab;

    public delegate void OnEnemySelectedChange(int newHealth);

    public event OnEnemySelectedChange onEnemySelectedChange;


    protected virtual void Start()
    {

        towerMap = FindObjectOfType<TileClickHandler>().GetTowerMap();

        gameState = GameState.Instance;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;

        target = GameObject.FindWithTag("Base").transform;

        path = new NavMeshPath();
        oldPath = path;

        maxHealth = health;

        //if (flying) ChangeAgentType(newAgentTypeName);
        if (stealth) SetStealth();
    }



    protected virtual void ChangeAgentType(string agentTypeName)
    {


        // Get the NavMeshAgent component

        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent component found on this GameObject.");
            return;
        }

        // Find the desired agent type ID
        int agentTypeID = GetAgentTypeID(agentTypeName);
        if (agentTypeID == -1)
        {
            Debug.LogError($"Agent type '{agentTypeName}' not found.");
            return;
        }

        // Cache current agent properties
        Vector3 currentPosition = agent.transform.position;
        Quaternion currentRotation = agent.transform.rotation;

        float speed = agent.speed;
        float acceleration = agent.acceleration;
        float angularSpeed = agent.angularSpeed;
        float stoppingDistance = agent.stoppingDistance;

        // Destroy the current NavMeshAgent
        Destroy(agent);

        // Wait until the current frame ends before adding the new component
        StartCoroutine(AddNewNavMeshAgent(agentTypeID, currentPosition, speed, acceleration, angularSpeed, stoppingDistance));


    }

    private IEnumerator AddNewNavMeshAgent(int agentTypeID, Vector3 position, float speed, float acceleration, float angularSpeed, float stoppingDistance)
    {
        // Wait for the current frame to finish
        yield return null;

        // Add a new NavMeshAgent
        NavMeshAgent newAgent = gameObject.AddComponent<NavMeshAgent>();
        newAgent.agentTypeID = agentTypeID;

        // Reapply cached properties
        newAgent.speed = speed;
        newAgent.acceleration = acceleration;
        newAgent.angularSpeed = angularSpeed;
        newAgent.stoppingDistance = stoppingDistance;

        // Reposition the agent on the NavMesh
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            newAgent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("Failed to reposition agent on the NavMesh. It might not match the new agent type.");
        }

        agent = newAgent;

    }

    private int GetAgentTypeID(string agentTypeName)
    {
        // Search for the agent type in the NavMesh settings
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++)
        {
            var settings = NavMesh.GetSettingsByIndex(i);
            if (NavMesh.GetSettingsNameFromID(settings.agentTypeID) == agentTypeName)
            {
                return settings.agentTypeID;
            }
        }
        return -1; // Agent type not found
    }

    public virtual int GetHP()
    {
        return health;
    }

    public virtual int GetMaxHP()
    {
        return maxHealth;
    }

    public virtual bool GetStealth()
    {
        return stealth;
    }

    public virtual bool GetFlying()
    {
        return flying;
    }

    public void SetVariations(Dictionary<string, object> variations)
    {
        // Modify speed if specified
        if (variations.ContainsKey("speed") && variations["speed"] is double speedValue)
        {
            float vspeed = (float)speedValue; // Convert from double to float

            speed = vspeed;
        }

        // Modify health if specified
        if (variations.ContainsKey("health"))
        {
            int healthValue = Convert.ToInt32(variations["health"]);
            print("is in the block");
            health = healthValue;
        }

        // Modify color if specified
        if (variations.ContainsKey("color") && variations["color"] is string colorString)
        {
            if (UnityEngine.ColorUtility.TryParseHtmlString(colorString, out Color newColor))
            {

                spriteRenderer.color = newColor;

            }
            else
            {
                Debug.LogError($"Invalid color string: {colorString}");
            }
        }

        if (variations.ContainsKey("stealth") && variations["stealth"] is bool isStealth)
        {
            stealth = isStealth;
        }

        if (variations.ContainsKey("flying") && variations["flying"] is bool isFlying)
        {
            flying = isFlying;
        }

        if (variations.ContainsKey("description") && variations["description"] is string description)
        {
            this.description = description;
        }
    }


    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (selected) onEnemySelectedChange?.Invoke(health);

        if (health <= 0)
        {
            DeselectEnemy();
            Die();
        }
    }

    public virtual int GetDamage()
    {
        return damage;
    }

    public virtual Sprite GetSprite()
    {
        return frontSprite;
    }

    public virtual NavMeshAgent GetAgent()
    {
        return agent;
    }

    public virtual void SetDescription(string description)
    {
        this.description = description;
    }

    public GameObject SelectEnemy(GameObject prefab)
    {
        selectedHighlightPrefab = Instantiate(prefab, this.gameObject.transform);
        selected = true;

        return selectedHighlightPrefab;

    }

    public void DeselectEnemy()
    {
        selected = false;
    }



    public virtual Dictionary<string, string> GetInfo()
    {

        Dictionary<string, string> info = new Dictionary<string, string>
        {
            {"name", enemyName},
            {"maxHealth", maxHealth.ToString()},
            {"health", health.ToString()},
            {"speed", speed.ToString()},
            {"reward", reward.ToString()},
            {"description", description}
        };

        string isBoolValue = "";

        if (stealth)
        {
            isBoolValue = name + " is stealth";
            info.Add("stealth", isBoolValue);
        }

        if (flying)
        {
            isBoolValue = name + " is a flying enemy.";
            info.Add("flying", isBoolValue);
        }

        return info;
    }

    public virtual void Die(bool goal = false)
    {
        gameState.RemoveEnemyFromList(this);

        if (!goal) gameState.SetMoney(reward);

        Destroy(gameObject);
    }

    protected virtual void SetStealth()
    {
        Color color = spriteRenderer.color;

        // Set the alpha (transparency) value
        color.a = Mathf.Clamp01(0.8f); // Ensure transparency is between 0 and 1

        // Apply the new color
        spriteRenderer.color = color;
    }

    protected virtual void Move()
    {

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh or is not active.");
            agent = GetComponent<NavMeshAgent>();
        }


    }

    protected virtual void AnimateMovement()
    {

        elapsed += Time.deltaTime;
        if (elapsed > minAnimTime)
        {
            elapsed -= minAnimTime;

        }

        else return;


        Vector2 velocity = agent.velocity;

        // Determine the primary direction based on velocity
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            // Moving horizontally
            if (velocity.x > 0)
            {
                // Moving right
                spriteRenderer.sprite = rightSprite;
            }
            else
            {
                // Moving left
                spriteRenderer.sprite = leftSprite;
            }
        }
        else
        {
            // Moving vertically
            if (velocity.y > 0)
            {
                // Moving up
                spriteRenderer.sprite = backSprite;
            }
            else
            {
                // Moving down
                spriteRenderer.sprite = frontSprite;
            }
        }
    }


    protected virtual void DebugPath()
    {
        oldPath = path;

        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }

    }

    protected virtual void Update()
    {
        Move();

        AnimateMovement();

        //DebugPath();

        // Calculate the angle in degrees
        //float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg + 90f;

        // Set the rotation of the sprite




    }

    public virtual bool CheckForObstructions()
    {
        return CalculateNewPath();
    }

    protected virtual void SetNewTileTarget()
    {


        pathMode = 1;
    }

    protected virtual bool FindPathObstruction()
    {
        Vector2 start;
        Vector2 end;
        Vector2 direction;
        float distance;

        for (int i = 0; i < oldPath.corners.Length - 1; i++)
        {
            start = oldPath.corners[i];
            end = oldPath.corners[i + 1];
            direction = end - start;

            distance = direction.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, layerMask);

            if (hit.collider != null)
            {
                Debug.Log("Hit a collider: " + hit.collider.gameObject.name);

                Vector3 hitWorldPosition = hit.point;
                Vector3Int tilePosition = towerMap.WorldToCell(hitWorldPosition);
                Vector3 tileWorldPosition = towerMap.CellToWorld(tilePosition);

                tileTarget = tileWorldPosition;
                return true;
            }

            else
            {
                Debug.Log("No collider detected between the points.");

            }

        }


        return false;




    }

    protected virtual bool CalculateNewPath()
    {


        agent.CalculatePath(target.position, path);

        print("New path calculated");
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {

            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);


            return true;
        }
    }
}
