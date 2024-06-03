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

        Debug.LogFormat("[{0}] 병렬 실행 테스트", Time.time - time >= 0.3f ? "통과" : "실패");
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestA());
        cs.Append(CoTestB());
        cs.Append(CoTestC());

        time = Time.time;

        yield return new WaitUntil(() => cs.Completed);

        Debug.LogFormat("[{0}] 순차 실행 테스트", Time.time - time >= 0.6f ? "통과" : "실패");

        var isCompleted = false;
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestD());
        cs.OnComplete(() => isCompleted = true);

        yield return new WaitUntil(() => cs.Completed);

        Debug.LogFormat("[{0}] 완료 콜백 테스트", isCompleted ? "통과" : "실패");
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestD());
        
        yield return null;
        
        time = Time.time;

        cs.Pause();

        yield return new WaitForSeconds(0.5f);

        cs.Resume();

        yield return new WaitUntil(() => cs.Completed);

        Debug.LogFormat("[{0}] 일시정지/계속 테스트", Time.time - time >= 0.5f ? "통과" : "실패");
        
        isCompleted = false;
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestD());
        cs.OnComplete(() => isCompleted = true);

        yield return null;

        cs.Stop();
        
        yield return new WaitUntil(() => cs.Stoped);
        
        Debug.LogFormat("[{0}] 중지 테스트", isCompleted == false ? "통과" : "실패");

        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestA());
        cs.Interval(0.5f);
        cs.Append(CoTestA());
        
        time = Time.time;
        
        yield return new WaitUntil(() => cs.Completed);
        
        Debug.LogFormat("[{0}] 대기 테스트", Time.time - time > 0.7f ? "통과" : "실패");

        var isCalled = false;
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestA());
        cs.Callback(() => isCalled = true);
        cs.Append(CoTestA());
        
        yield return new WaitUntil(() => cs.Completed);
        
        Debug.LogFormat("[{0}] 콜백 테스트", isCalled ? "통과" : "실패");

        var a = 0;
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestD());
        cs.Until(() => a >= 10);
        cs.OnComplete(() =>
        {
            Debug.LogFormat("[{0}] Until 조건 만족 테스트", a >= 10 ? "통과" : "실패");
        });
        
        while (a < 10)
        {
            a++;
            yield return null;
        }
        
        yield return new WaitUntil(() => cs.Completed);
        
        a = 0;
        
        cs = CSPlayer.CoroutineStream();
        
        cs.Append(CoTestD());
        cs.While(() => a < 10);
        cs.OnComplete(() =>
        {
            Debug.LogFormat("[{0}] While 조건 만족 테스트", a >= 10 ? "통과" : "실패");
        });
        
        while (a < 10)
        {
            a++;
            yield return null;
        }
        
        yield return new WaitUntil(() => cs.Completed);
        
        Debug.Log("테스트 완료");
            
        UnityEditor.EditorApplication.isPlaying = false;
    }
    
    private IEnumerator CoTestA()
    {
        yield return new WaitForSeconds(0.1f);
    }
    
    private IEnumerator CoTestB()
    {
        yield return new WaitForSeconds(0.2f);
    }
    
    private IEnumerator CoTestC()
    {
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator CoTestD()
    {
        for (int i = 0; i < 10; i++)
        {
            // Debug.LogFormat("CoTestD - {0}", i);
            yield return null;
        }
    }
}