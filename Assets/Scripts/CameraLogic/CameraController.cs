using System;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

namespace CameraLogic
{
    public class CameraController
    {
        private Camera _camera;
        private Transform _target;
        private Quaternion _rotationOnTarget;
        private Vector3 _defaultPos;
        private Quaternion _defaultRotation;
        private Tween _cameraTween;
        private float _timeOffset = 0f;

        public CameraController(Camera cameraToWorkWith)
        {
            _camera = cameraToWorkWith;
            _defaultPos = _camera.transform.position;
            _defaultRotation = _camera.transform.rotation;
            _rotationOnTarget = _defaultRotation;
        }

        public void MoveCamera(Transform target = null, float tweenTimeOffset = 0f)
        {
            _timeOffset = tweenTimeOffset;
            
            if (target != null)
            {
                _target = target;
                TweenCamera(CameraTarget.TargetPos);
            }
            else
                TweenCamera(CameraTarget.DefaultPos);
        }
        
        private void TweenCamera(CameraTarget typeOfTargetPos)
        {
            _cameraTween?.Kill();
            
            Vector3 targetPos = _defaultPos;

            if (typeOfTargetPos == CameraTarget.TargetPos)
            {
                targetPos = new Vector3(
                    _target.transform.position.x,
                    _target.transform.position.y + 4f,
                    _target.transform.position.z - 2f);
            }
            
            _cameraTween = _camera.transform.DOMove(targetPos, 1f + _timeOffset)
                .OnComplete(() => _cameraTween = null);
        }
        private enum CameraTarget
        {
            DefaultPos = 0,
            TargetPos = 1,
        }
    }
    
}