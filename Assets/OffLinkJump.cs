using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum OffmeshLinkMethod
{
    Teleport,
    NormalSpeed,
    Parabola,
    Curve
}
public class OffLinkJump : MonoBehaviour
{
    NavMeshAgent agent;
    public OffmeshLinkMethod method;
    public AnimationCurve curve;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //Teleport
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            //Detect if agent on link
            if (agent.isOnOffMeshLink)
            {
                if(method == OffmeshLinkMethod.NormalSpeed)
                {
                    yield return StartCoroutine(NormalSpeed(agent));
                }
                else if (method == OffmeshLinkMethod.Parabola)
                {
                    yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                }
                else if (method == OffmeshLinkMethod.Curve)
                {
                    yield return StartCoroutine(Curve(agent, 0.5f));
                }
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Gets the Speed
    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        //Check if the agent is in the OffMeshLink
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        //Check if the agent is in the OffMeshLink
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0)
        {
            float yOffset = height * 5.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime/duration;
            yield return null;
        }
    }
    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        //Check if the agent is in the OffMeshLink
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0)
        {
            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
