using System;
using System.Collections.Generic;
using DG.Tweening;
using GardenLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameHudController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI _experienceAmmountTMP;
        [Space]
        [Header("Select plant popup")]
        [SerializeField] private RectTransform _popUpRect;
        [SerializeField] private ScrollRect _plantsScrollRect;
        [Space] 
        [SerializeField] private Button _missClickButton; // its black background
        [SerializeField] private PlantUiLine _plantViewPrefab;

        public bool IsAnyPopUpShowing => _isPopUpShowing;
        
        private Action<PlantScriptableData> _callBack;
        private Sequence _tweenSequence;
        private bool _isPopUpShowing = false;
        private Image _popUpBG;
        private Color _visibleColorValue;

        private void Awake()
        {
            _missClickButton.onClick.AddListener(ChangePopUpVisability);
            _popUpBG = _missClickButton.image; // same object
            _visibleColorValue = _popUpBG.color;
        }

        public void InitController(List<PlantScriptableData> plantsData)
        {
            _experienceAmmountTMP.text = "0";
            _popUpBG.gameObject.SetActive(false);
            _popUpRect.localScale = Vector3.zero;
            _plantsScrollRect.verticalNormalizedPosition = 1f;

            foreach (var data in plantsData)
            {
                var line = Instantiate(_plantViewPrefab, _plantsScrollRect.content);
                line.Init(data);
                line.OnLineClicked += OnPlantSelected;
            }
        }

        public void ShowPlantsList(Action<PlantScriptableData> callBack)
        {
            _callBack = callBack;
            ChangePopUpVisability();
        }

        private void OnPlantSelected(PlantScriptableData selected)
        {
            _callBack?.Invoke(selected);
            ChangePopUpVisability();
        }
        private void ChangePopUpVisability()
        {
            _isPopUpShowing = !_isPopUpShowing;

            _popUpBG.gameObject.SetActive(true);
            _tweenSequence?.Kill();
            
            Vector3 targetScale = _isPopUpShowing ? Vector3.one : Vector3.zero;
            float targetAlpha = _isPopUpShowing ? _visibleColorValue.a : 0f;

            _tweenSequence = DOTween.Sequence();
            _tweenSequence.Append(_popUpRect.DOScale(targetScale, 0.3f).SetEase(Ease.Linear));
            _tweenSequence.Join(_popUpBG.DOFade(targetAlpha, 0.3f).SetEase(Ease.Linear));

            _tweenSequence.OnComplete(() =>
            {
                _tweenSequence = null;
                _popUpBG.gameObject.SetActive(_isPopUpShowing);
            });

            _plantsScrollRect.verticalNormalizedPosition = 1f;
        }
        public void UpdateExperienceStats(string stats) =>
            _experienceAmmountTMP.text = stats;
        
    }
}