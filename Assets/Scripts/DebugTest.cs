using UnityEngine;

public class DebugTest : MonoBehaviour
{
    void Start()
    {
        ExternalApplicationExecutor.DebugRun("Hello World!", false);
    }
}
