using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace DarkNaku.CoroutineStream
{
    public class CSTask : CustomYieldInstruction 
    {
        public bool Stoped => _stoped;
        public bool Paused => _paused;
        public bool Completed => _completeCount == _taskCount;
        public override bool keepWaiting => !Completed;

        private int _taskCount;
        private int _completeCount;
        private bool _stoped;
        private bool _paused;
        private bool _running;
        private Coroutine _coroutine;
        private IEnumerator[] _tasks;
        private MonoBehaviour _player;

        public CSTask(MonoBehaviour player, params IEnumerator[] tasks) 
        {
            if (player == null)
            {
                Debug.LogError("[Task] Constructor : Player is null.");
                return;
            }
            
            if (tasks == null || tasks.Length == 0) 
            {
                Debug.LogError("[Task] Constructor : It must have at least one parameter.");
                return;
            }

            _player = player;
            _tasks = tasks;
            _taskCount = _tasks.Length;
        }

        public Coroutine Play() 
        {
            if (_player == null || _tasks == null || _tasks.Length == 0)
            {
                Debug.LogError("[Task] Play : Player or Tasks is null.");
                return null;
            }
            
            _coroutine = _player.StartCoroutine(CoPlay(_player));
            
            return _coroutine;
        }

        public void Pause() 
        {
            _paused = true;
        }

        public void Resume() 
        {
            _paused = false;
        }
        
        public void Stop() 
        {
            _tasks = null;
            _stoped = true;
            _running = false;
            _player.StopCoroutine(_coroutine);
        }

        private IEnumerator CoPlay(MonoBehaviour player) 
        {
            _running = true;

            for (int i = 0; i < _tasks.Length; i++) 
            {
                player.StartCoroutine(CoWrapper(_tasks[i]));
            }

            yield return new WaitUntil(() => Completed);

            _tasks = null;
            _stoped = true;
            _running = false;
        }

        private IEnumerator CoWrapper(IEnumerator coroutine) 
        {
            if (coroutine == null) 
            {
                _completeCount++;
                yield break;
            }

            while (_running) 
            {
                if (_paused) 
                {
                    yield return null;
                } 
                else if (coroutine.MoveNext()) 
                {
                    yield return coroutine.Current;
                } 
                else 
                {
                    _completeCount++;
                    break;
                }
            }
        }
    }
}