using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GardenLogic
{
    public class GrowingTimer
    {
        private Image _timerBg;
        private TextMeshProUGUI _timerTextTMP;
        private Action _callBack;

        private bool _isShowing = false;
        private Sequence _tweenSequence;

        public GrowingTimer(Image timerBG, TextMeshProUGUI timerText, Action callBack)
        {
            _timerBg = timerBG;
            _timerTextTMP = timerText;
            _callBack = callBack;

            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            var currentCol = _timerBg.color;
            currentCol.a = 0f;
            _timerBg.color = currentCol;
            _timerBg.gameObject.SetActive(false);

            _timerBg.rectTransform.localScale = Vector3.zero;
        }
        public async void StartTimer(int secondsToGrow)
        {
            TweenTimerVisability();

            var currentTime = DateTime.UtcNow;
            var targetTime = DateTime.UtcNow.AddSeconds(secondsToGrow);
            
            while (currentTime < targetTime)
            {
                currentTime = DateTime.UtcNow;
                TimeSpan dif = targetTime - currentTime;
                _timerTextTMP.text = $"{dif.Hours}h-{dif.Minutes}m-{dif.Seconds}s";
                await Task.Delay(1000);
            }
            
            if (!Application.isPlaying)
                return;
            
            TweenTimerVisability();
            _callBack?.Invoke();
        }

        private void TweenTimerVisability()
        {
            _isShowing = !_isShowing;
            _timerBg.gameObject.SetActive(true);

            Vector3 targetScale = _isShowing ? Vector3.one : Vector3.zero;
            float targetFade = _isShowing ? 0.42f : 0f;

            _tweenSequence = DOTween.Sequence();
            _tweenSequence.Append(_timerBg.DOFade(targetFade, 1f));
            _tweenSequence.Join(_timerBg.rectTransform.DOScale(targetScale, 1f));
            
            _tweenSequence.OnComplete(() =>
            {
                _tweenSequence = null;
                _timerBg.gameObject.SetActive(_isShowing);
            });
        }
    }
}