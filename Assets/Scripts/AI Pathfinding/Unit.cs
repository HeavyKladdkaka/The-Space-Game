using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    public Transform target1;
    public Transform target2;
    public Transform target3;

    public Transform player;

    public float speed = 20;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    private bool isTarget1, isTarget2, isTarget3;
    private Transform target;

    Path path;

    void Start()
    {

        isTarget1 = true;
        isTarget2 = false;
        isTarget3 = false;

        CheckTargets();
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {

            path = new Path(waypoints, transform.position, turnDst, stoppingDst);

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {

        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        //if()

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            //print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {

        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    //CheckTargets();
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {

                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);

                    if (speedPercent < 0.01f)
                    {
                        CheckTargets();
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;

        }
    }

    public void CheckTargets()
    {
        if (isTarget1)
        {
            isTarget1 = false;
            isTarget2 = true;
            target = target2;
        }
        else if (isTarget2)
        {
            isTarget2 = false;
            isTarget3 = true;
            target = target3;
        }
        else if (isTarget3)
        {
            isTarget3 = false;
            isTarget1 = true;
            target = target1;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "Player")
        {
            target = player;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            target = player;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CheckTargets();
        }
    }
}
