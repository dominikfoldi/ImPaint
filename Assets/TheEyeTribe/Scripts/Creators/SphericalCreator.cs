/*
 * Copyright (c) 2013-present, The Eye Tribe. 
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree. 
 *
 */

using UnityEngine;
using VRStandardAssets.Utils;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace EyeTribe.Unity.Interaction
{
    /// <summary>
    /// Utility class the creates GameObjects in a spherical pattern around parent GO
    /// <para/>
    /// Classes wishing to create an arbitrary GameObject should extend this class and
    /// override CreateObject() method.
    /// </summary>
    public class SphericalCreator : MonoBehaviour
    {
        [SerializeField] private float _Radius = 10f;
        [SerializeField] private float _RandomRange;
        [SerializeField] private int _NumLongitude = 1;
        [SerializeField] private int _NumLatitude = 1;
        [SerializeField] private float _DuplicateDistance = .5f;
        [SerializeField] private bool _FaceCenter = true;
        [SerializeField] private bool _RandomRotation = false;

        protected virtual void Awake()
        {
            if (_Radius <= 0f)
                throw new Exception("_Radius must be a positive number");

            if (_RandomRange < 0f)
                throw new Exception("_RandomRange must be a positive number");

            if (_NumLongitude < 1f)
                throw new Exception("_NumLongitude must be a positive and non-zero number");

            if (_NumLatitude < 1f)
                throw new Exception("_NumLatitude must be a positive and non-zero number");

            if (_DuplicateDistance < 0f)
                throw new Exception("_DuplicateDistance must be a positive number");
        }

        void OnEnable() 
        {
            Initialize();
        }

        void OnDisable()
        {
            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }

        protected virtual void Initialize()
        {
            float LongStep = 360f / _NumLongitude;
            float LatStep = 360f / _NumLatitude;

            // Generate transforms while evaluating duplicates
            float rangeHalf = _RandomRange * .5f;
            var range = _Radius + UnityEngine.Random.Range(-rangeHalf, rangeHalf);
            var dir = Vector3.forward;
            var v = (transform.rotation * Quaternion.Euler(LongStep, LatStep, 0f)) * dir;
            var pos = v * range;

            /*
            Debug.DrawLine(transform.position + transform.rotation * pos,
                transform.position + transform.rotation * (pos + (v * -range)), Color.red, 5f);
            */

            Quaternion rotation = Quaternion.identity;

            if (_FaceCenter)
                rotation = transform.rotation * Quaternion.LookRotation(v * -1);

            if (_RandomRotation)
                rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f),
                    UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));

            GameObject go = CreateObject(pos, rotation);
            go.transform.parent = transform;
        }

        protected virtual GameObject CreateObject(Vector3 position, Quaternion rotation)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }
    }
}
