using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkNaku.CoroutineStream
{
    public class CoroutineStream {
        public bool Stoped => _stoped;
        public bool Paused => _paused;
        public bool Completed => _completed;

        private bool _stoped;
        private bool _paused;
        private CSTask _current;
        private bool _completed;
        private Queue<CSTask> _tasks;
        private Coroutine _coroutine;
        private MonoBehaviour _player;
        private System.Action _onComplete;

        public CoroutineStream(MonoBehaviour player) 
        {
            if (!player)
            {
                Debug.LogError("[CoroutineStream] Constructor : Player is null.");
                return;
            }
            
            _player = player;
            _tasks = new Queue<CSTask>();
        }

        public CoroutineStream Pause() {
            _paused = true;

            if (_current != null) 
            {
                _current.Pause();
            }

            return this;
        }

        public CoroutineStream Resume() 
        {
            _paused = false;

            if (_current != null) 
            {
                _current.Resume();
            }
            
            return this;
        }

        public void Stop() 
        {
            _stoped = true;
            _tasks.Clear();

            if (_current != null) 
            {
                _current.Stop();
                _current = null;
            }

            if (_coroutine != null) 
            {
                _player.StopCoroutine(_coroutine);
            }
        }

        public CoroutineStream Append(params IEnumerator[] coroutines) 
        {
            Enqueue(new CSTask(_player, coroutines));
            return this;
        }

        public CoroutineStream Interval(float seconds) 
        {
            Enqueue(new CSTask(_player, WaitForSeconds(seconds)));
            return this;
        }

        public CoroutineStream Until(System.Func<bool> predicate) 
        {
            Enqueue(new CSTask(_player, WaitUntil(predicate)));
            return this;
        }

        public CoroutineStream While(System.Func<bool> predicate) 
        {
            Enqueue(new CSTask(_player, WaitWhile(predicate)));
            return this;
        }

        public CoroutineStream Callback(System.Action callback) 
        {
            Enqueue(new CSTask(_player, CoCallback(callback)));
            return this;
        }

        public CoroutineStream OnComplete(System.Action callback) 
        {
            _onComplete = callback;
            return this;
        }

        public async Task Async()
        {
            while (_stoped == false && _completed == false)
            {
                await Task.Yield();
            }
        }
        
        private void Enqueue(CSTask task) 
        {
            _tasks.Enqueue(task);
            
            _coroutine ??= _player.StartCoroutine(CoPlay());
        }

        private IEnumerator CoPlay() 
        {
            while (_tasks.Count > 0) 
            {
                while (_paused) 
                {
                    yield return null;
                }

                _current = _tasks.Dequeue();
                
                yield return _current.Play();
            }

            _tasks.Clear();
            _current = null;
            _completed = true;
            _onComplete?.Invoke();
        }

        private IEnumerator WaitForSeconds(float seconds) 
        {
            yield return new CustomWaitForSeconds(seconds);
        }

        private IEnumerator WaitUntil(System.Func<bool> predicate) 
        {
            yield return new WaitUntil(predicate);
        }

        private IEnumerator WaitWhile(System.Func<bool> predicate) 
        {
            yield return new WaitWhile(predicate);
        }

        private IEnumerator CoCallback(System.Action callback) 
        {
            callback?.Invoke();
            yield break;
        }
    }

    public class CustomWaitForSeconds : CustomYieldInstruction 
    {
        public override bool keepWaiting => Time.time - _startTime < _seconds;

        private float _startTime;
        private float _seconds;

        public CustomWaitForSeconds(float seconds)
        {
            _startTime = Time.time;
            _seconds = seconds;
        }
    }
}