using UnityEngine;

public class DebugTest : MonoBehaviour
{
    void Start()
    {
        ExternalApplicationExecutor.DebugRun("Hello Chatty", false);
    }
}
