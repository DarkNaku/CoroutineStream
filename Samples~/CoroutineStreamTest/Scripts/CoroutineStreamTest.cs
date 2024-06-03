using System.Collections;
using System.Collections.Generic;
using DarkNaku.CoroutineStream;
using UnityEngine;
using UnityEngine.Assertions;

public class CoroutineStreamTest : MonoBehaviour
{
    private IEnumerator Start()
    {
        var cs = CSPlayer.CoroutineStream().Append(CoTestA(), CoTestB(), CoTestC());

        var time = Time.time;

        yield return new WaitUntil(() => cs.Completed);

        Debug.LogFormat("[{0}] 병렬 실행 테스트", Time.time - time >= 1.5f ? "통과" : "실패");
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestA());
        cs.Append(CoTestB());
        cs.Append(CoTestC());

        time = Time.time;

        yield return new WaitUntil(() => cs.Completed);

        Debug.LogFormat("[{0}] 순차 실행 테스트", Time.time - time >= 3f ? "통과" : "실패");

        UnityEditor.EditorApplication.isPlaying = false;
    }

    private IEnumerator CoTestA()
    {
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator CoTestB()
    {
        yield return new WaitForSeconds(1.0f);
    }
    
    private IEnumerator CoTestC()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
