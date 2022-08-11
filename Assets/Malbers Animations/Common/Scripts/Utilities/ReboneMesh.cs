﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    public class ReboneMesh : MonoBehaviour
    {

        [ContextMenuItem("Transfer Bones From Skin", "DuplicateBones")]
        public GameObject _sourceSkinMesh;

        [ContextMenuItem("Transfer Bones From Root", "TransferRootBone")]
        public Transform _sourceBones;



        [ContextMenu("Transfer Bones From Skin")]
        void DuplicateBones()
        {
            if (_sourceSkinMesh != null)
            {
                CopyFromSkinToSkin();
                Debug.Log("Trasfer Ready");
            }
        }

        [ContextMenu("Transfer Bones From Root")]
        void TransferRootBone()
        {
            if (_sourceBones != null)
            {
                CopyBonesSameBones();
                Debug.Log("Trasfer Ready RootBone");
            }
        }



        private void CopyFromSkinToSkin()
        {
            SkinnedMeshRenderer targetRenderer = _sourceSkinMesh.GetComponent<SkinnedMeshRenderer>();

            Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
            foreach (Transform bone in targetRenderer.bones)
            {
                boneMap[bone.name] = bone;
            }

            SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
            Transform[] boneArray = thisRenderer.bones;
            for (int idx = 0; idx < boneArray.Length; ++idx)
            {
                string boneName = boneArray[idx].name;
                if (false == boneMap.TryGetValue(boneName, out boneArray[idx]))
                {
                    Debug.LogError("failed to get bone: " + boneName);
                    Debug.Break();
                }
            }
            thisRenderer.bones = boneArray;
            thisRenderer.rootBone = targetRenderer.rootBone;
        }

        private void CopyBonesSameBones()
        {
            Transform[] rootBone = _sourceBones.GetComponentsInChildren<Transform>();

            Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

            foreach (Transform bone in rootBone)
            {
                boneMap[bone.name] = bone;
            }

            SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
            Transform[] boneArray = thisRenderer.bones;


            for (int idx = 0; idx < boneArray.Length; ++idx)
            {
                string boneName = boneArray[idx].name;
                if (false == boneMap.TryGetValue(boneName, out boneArray[idx]))
                {
                    Debug.LogError("failed to get bone: " + boneName);

                }
            }
            thisRenderer.bones = boneArray;
           // thisRenderer.rootBone = targetRenderer.rootBone;
        }
    }
}