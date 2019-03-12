
/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

using UnityEngine;

namespace Aura2API
{
    /// <summary>
    /// Static class containing extension for Camera type
    /// </summary>
    public static class CameraExtensions
    {
        #region Private Members
        /// <summary>
        /// Clip space corners' position
        /// </summary>
        private static readonly Vector3[] _frustumClipPos =         //                     E_______________F
        {                                                           //                    /|		    _/ |
                new Vector3(-1, 1, -1), // A                        //                   / |		  _/   |
                new Vector3(1, 1, -1), // B                         //                  /  |	    _/	   |	FAR
                new Vector3(1, -1, -1), // C                        //                 /   |     _/	       |
                new Vector3(-1, -1, -1), // D                       //                /	   H____/__________G
                new Vector3(-1, 1, 1), // E                         //               A____/__B        __/
                new Vector3(1, 1, 1), // F                          //               |  _/   |     __/
                new Vector3(1, -1, 1), // G                         //        NEAR   |_/	 |  __/
                new Vector3(-1, -1, 1) // H                         //               D_______C_/
        };
        
        /// <summary>
        /// The automatic spawn distance from the camera for gameObjects creation
        /// </summary>
        private static readonly float _spawnDistanceFromCamera = 50.0f;
        /// <summary>
        /// The automatic spawn downwards search distance if there is no surface to spawn onto at _spawnDistanceFromCamera
        /// </summary>
        private static readonly float _spawnHeightTolerance = 25.0f;
        #endregion

        #region Functions
        /// <summary>
        /// Tells if the current camera is the sceneView camera
        /// </summary>
        public static bool IsCurrentSceneViewCamera
        {
            get
            {
                return Camera.current != null && Camera.current.IsSceneViewCamera();
            }
        }

        /// <summary>
        /// Tells if the reference camera is the SceneView camera
        /// </summary>
        /// <returns>True if the reference camera is the current SceneView camera</returns>
        public static bool IsSceneViewCamera(this Camera camera)
        {
            #if UNITY_EDITOR
            return camera.cameraType == CameraType.SceneView;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Computes the planes corresponding to the sides of frustum (a tapered box) of the camera
        /// </summary>
        /// <param name="nearClipPlaneDistance">The near clip distance</param>
        /// <param name="farClipPlaneDistance">The far clip distance</param>
        /// <returns>An array with the planes corresponding to the sides of frustum, in the following order : left, right, top, bottomm, near, far</returns>
        public static Plane[] GetFrustumPlanes(this Camera camera, float nearClipPlaneDistance, float farClipPlaneDistance)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);            
            // Overwrite near and far planes
            frustumPlanes[4] = new Plane(camera.transform.forward, camera.transform.position + camera.transform.forward * nearClipPlaneDistance);     // near
            frustumPlanes[5] = new Plane(-camera.transform.forward, camera.transform.position + camera.transform.forward * farClipPlaneDistance);     // far

            return frustumPlanes;
        }

        /// <summary>
        /// Computes the size of the frustum at given distance
        /// </summary>
        /// <param name="distance">The distance at which we want to know the size of the frustum</param>
        /// <returns>The size of the frustum at given distance</returns>
        public static Vector2 GetFrustumSizeAtDistance(this Camera camera, float distance)
        {
            float height =  Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f * distance;
            float width = height * camera.aspect;

            return new Vector2(width, height);
        }

        /// <summary>
        /// Returns the viewing eye for Stereo rendering (if not stereo or not in a render loop, returns Left)
        /// </summary>
        /// <returns>The current eye (left if not stereo or not in a render loop)</returns>
        public static Camera.StereoscopicEye GetStereoscopicEye(this Camera camera)
        {
            return (Camera.StereoscopicEye)((int)camera.stereoActiveEye % 2);
        }

        /// <summary>
        /// Computes the corners of the plane parallel to the camera at given distance
        /// </summary>
        /// <param name="eye">The stereoscopic eye</param>
        /// <param name="planeDistance">The reference distance</param>
        /// <returns>the four corners of the intersecting plane</returns>
        public static Vector4[] GetFrustumPlaneCorners(this Camera camera, Camera.MonoOrStereoscopicEye eye, float planeDistance)
        {
            Vector3[] tmpArray = new Vector3[4];
            camera.CalculateFrustumCorners(/*camera.rect*/new Rect(0,0,1,1), planeDistance, eye, tmpArray);
            for (int i = 0; i < 4; ++i)
            {
                tmpArray[i] = camera.transform.localToWorldMatrix.MultiplyPoint(tmpArray[i]);
            }

            Vector4[] planeCorners = new Vector4[4];
            Vector3 tmp = tmpArray[0];
            planeCorners[0] = new Vector4(tmpArray[1].x, tmpArray[1].y, tmpArray[1].z, 1.0f);
            planeCorners[1] = new Vector4(tmpArray[2].x, tmpArray[2].y, tmpArray[2].z, 1.0f);
            planeCorners[2] = new Vector4(tmpArray[3].x, tmpArray[3].y, tmpArray[3].z, 1.0f);
            planeCorners[3] = new Vector4(tmp.x, tmp.y, tmp.z, 1.0f);

            return planeCorners;
        }

        /// <summary>
        /// Computes the world space frustum's corners' position
        /// </summary>
        /// <param name="nearClipDistance">The desired near plane</param>
        /// <param name="farClipDistance">The desired far plane</param>
        /// <returns>An array containing the positions</returns>
        public static Vector4[] GetFrustumCorners(this Camera camera, Camera.MonoOrStereoscopicEye eye, float nearClipDistance, float farClipDistance)
        {
            Vector4[] nearPlaneCorners = camera.GetFrustumPlaneCorners(eye, nearClipDistance);
            Vector4[] farPlaneCorners = camera.GetFrustumPlaneCorners(eye, farClipDistance);
            return nearPlaneCorners.Append(farPlaneCorners);
        }

        /// <summary>
        /// Computes a default spawn position in front of the camera. The position will be in this order : a) on the surface in front of the camera (if within 50m). b) if not, 50m in front of the camera, on a surface which would be within 25m under. c) if not, 50m in front of the camera.
        /// </summary>
        /// <returns>The computed spawn position</returns>
        public static Vector3 GetSpawnPosition(this Camera camera)
        {
            Vector3 spawnPosition;

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo, _spawnDistanceFromCamera))
            {
                spawnPosition = hitInfo.point;
            }
            else
            {
                spawnPosition = camera.transform.position + camera.transform.forward * _spawnDistanceFromCamera;

                if (Physics.Raycast(spawnPosition, Vector3.down, out hitInfo, _spawnHeightTolerance))
                {
                    spawnPosition = hitInfo.point;
                }
            }
            
            return spawnPosition;
        }

        /// <summary>
        /// Computes the world position of the frustum corners with custom near/far clip distances
        /// </summary>
        /// <param name="camera">The queried camera</param>
        /// <param name="nearClipPlaneDistance">The near clip distance</param>
        /// <param name="farClipPlaneDistance">The far clip distance</param>
        /// <returns>An array with the world position of the corners in the following order : nearTopLeft, nearTopRight, nearBottomRight, nearBottomLeft, farTopLeft, farTopRight, farBottomRight, farBottomLeft</returns>
        public static Vector4[] GetViewportFrustumCornersWorldPosition(this Camera camera, float nearClipPlaneDistance, float farClipPlaneDistance)
        {
            return new Vector4[]
                   {
                       camera.ViewportToWorldPoint(new Vector3(0, 1, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, farClipPlaneDistance))
                   };
        }

        /// <summary>
        /// Computes the world position of the corners of the frustum
        /// </summary>
        /// <param name="frustumClipToCameraInverseProjMatrix">The clip to camera inverse projection matrix</param>
        /// <returns>An array containing the world position of the corners of the frustum</returns>
        public static Vector4[] GetFrustumCornersWorldPosition(this Camera camera, Matrix4x4 frustumClipToCameraInverseProjMatrix)
        {
            Vector4[] frustumWorldPos = new Vector4[8];

            for (int i = 0; i < 8; ++i)
            {
                frustumWorldPos[i] = frustumClipToCameraInverseProjMatrix.MultiplyPoint(_frustumClipPos[i]);
            }

            return frustumWorldPos;
        }

        /// <summary>
        /// Computes the world position of the corners of the frustum
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="nearClipPlaneDistance">The near plane</param>
        /// <param name="farClipPlaneDistance">The far plane</param>
        /// <returns>An array containing the world position of the corners of the frustum</returns>
        public static Vector3[] GetFrustumCornersWorldPosition(this Camera camera, float nearClipPlaneDistance, float farClipPlaneDistance)
        {
            Vector3[] frustumWorldPos = new Vector3[8];

            Vector2 near = camera.GetFrustumSizeAtDistance(nearClipPlaneDistance);
            Vector2 far = camera.GetFrustumSizeAtDistance(farClipPlaneDistance);

            Matrix4x4 matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, Vector3.one);

            for (int i = 0; i < 8; ++i)
            {
                Vector3 pos = _frustumClipPos[i];
                if (pos.z < 0)
                {
                    pos.x *= near.x;
                    pos.y *= near.y;
                    pos.z = nearClipPlaneDistance;
                }
                else
                {
                    pos.x *= far.x;
                    pos.y *= far.y;
                    pos.z = farClipPlaneDistance;
                }

                frustumWorldPos[i] = matrix.MultiplyPoint3x4(pos);
            }

            return frustumWorldPos;
        }

        /// <summary>
        /// Gets the projection matrix of the camera
        /// </summary>
        /// <param name="eye">The stereoscopic eye</param>
        /// <returns></returns>
        public static Matrix4x4 GetProjectionMatrix(this Camera camera, Camera.MonoOrStereoscopicEye eye)
        {
            if (eye == Camera.MonoOrStereoscopicEye.Mono)
            {
                return camera.projectionMatrix;
            }
            else
            {
                return camera.GetStereoProjectionMatrix((Camera.StereoscopicEye)eye);
            }
        }

        /// <summary>
        /// Returns the camera's World space to Clip space matrix
        /// </summary>
        /// <param name="nearClipPlane">The near clip plane</param>
        /// <param name="farClipPlane">The far clip plane</param>
        /// <returns>The camera's World space to Clip space matrix</returns>
        public static Matrix4x4 GetWorldToClipMatrix(this Camera cameraComponent, Camera.MonoOrStereoscopicEye eye, float nearClipPlane, float farClipPlane)
        {
            float tmpNear = cameraComponent.nearClipPlane;
            cameraComponent.nearClipPlane = nearClipPlane;
            float tmpFar = cameraComponent.farClipPlane;
            cameraComponent.farClipPlane = farClipPlane;

            Matrix4x4 worldToCameraMatrix = cameraComponent.worldToCameraMatrix;
            Matrix4x4 projectionMatrix = cameraComponent.GetProjectionMatrix(eye);
            Matrix4x4 worldToClipMatrix = projectionMatrix * worldToCameraMatrix;

            cameraComponent.nearClipPlane = tmpNear;
            cameraComponent.farClipPlane = tmpFar;

            return worldToClipMatrix;
        }
        
        /// <summary>
        /// Returns the stereo mode of the camera
        /// </summary>
        /// <returns>What stereo mode the camera is running on</returns>
        public static StereoMode GetCameraStereoMode(this Camera camera)
        {
            if (camera.stereoEnabled)
            {
                if(XrHelpers.IsSinglePassStereo)
                {
                    return StereoMode.SinglePass;
                } 
                else
                {
                    return StereoMode.MultiPass;
                }
            }
            else
            {
                return StereoMode.Mono;
            }
        }
        #endregion
    }
}